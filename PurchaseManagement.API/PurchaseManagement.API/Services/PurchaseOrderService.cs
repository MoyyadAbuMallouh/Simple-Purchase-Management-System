using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PurchaseManagement.API.Data;
using PurchaseManagement.API.DTOs;
using PurchaseManagement.API.Models;
using PurchaseManagement.API.Services.Interfaces;

namespace PurchaseManagement.API.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMappingService _mapper;
        private readonly ILogger<PurchaseOrderService> _logger;
        private readonly IProductService _productService;

        public PurchaseOrderService(ApplicationDbContext context, IMappingService mapper, ILogger<PurchaseOrderService> logger, IProductService productService)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _productService = productService;
        }

        public async Task<PurchaseOrderDto> CreatePurchaseOrderAsync(CreatePurchaseOrderDto dto)
        {
            _logger.LogInformation("Creating purchase order for vendor ID {VendorId}", dto.VendorId);

            // Check if vendor exists
            var vendor = await _context.Vendors.FindAsync(dto.VendorId);
            if (vendor == null)
                throw new InvalidOperationException($"Vendor with ID {dto.VendorId} not found");

            // Validate products existence
            var productIds = dto.Items.Select(i => i.ProductId).Distinct();
            var missingProducts = new List<int>();

            foreach (var productId in productIds)
            {
                bool exists = await _productService.ProductExistsAsync(productId);
                if (!exists)
                    missingProducts.Add(productId);
            }

            if (missingProducts.Any())
                throw new InvalidOperationException($"Product(s) not found: {string.Join(", ", missingProducts)}");

            // Fetch actual product prices
            var productPrices = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.UnitPrice);

            // Build order items with correct prices
            var orderItems = dto.Items.Select(i => new PurchaseOrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = productPrices[i.ProductId]
            }).ToList();

            // Build purchase order entity
            var purchaseOrder = new PurchaseOrder
            {
                VendorId = dto.VendorId,
                Items = orderItems,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                TotalAmount = orderItems.Sum(i => i.Quantity * i.UnitPrice)
            };

            _context.PurchaseOrders.Add(purchaseOrder);
            await _context.SaveChangesAsync();

            return _mapper.MapToPurchaseOrderDto(purchaseOrder);
        }

        public async Task<PurchaseOrderDto> GetPurchaseOrderAsync(int id)
        {
            var purchaseOrder = await GetPurchaseOrdersWithIncludes()
                .FirstOrDefaultAsync(po => po.Id == id);

            if (purchaseOrder == null)
                throw new KeyNotFoundException($"Purchase order with ID {id} not found");

            return _mapper.MapToPurchaseOrderDto(purchaseOrder);
        }

        public async Task<List<PurchaseOrderDto>> GetAllPurchaseOrdersAsync()
        {
            var orders = await GetPurchaseOrdersWithIncludes().ToListAsync();
            return _mapper.MapToPurchaseOrderDtos(orders).ToList();
        }

        public async Task UpdatePurchaseOrderAsync(int id, UpdatePurchaseOrderDto dto)
        {
            var existingOrder = await _context.PurchaseOrders
                .Include(po => po.Items)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (existingOrder == null)
                throw new KeyNotFoundException($"Purchase order with ID {id} not found");

            existingOrder.VendorId = dto.VendorId;

            // Remove old items
            _context.PurchaseOrderItems.RemoveRange(existingOrder.Items);

            // Fetch product prices for updated items
            var productIds = dto.Items.Select(i => i.ProductId).Distinct();
            var productPrices = await _context.Products
                .Where(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.UnitPrice);

            // Add new items with correct unit prices
            var newItems = dto.Items.Select(i => new PurchaseOrderItem
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();

            existingOrder.Items = newItems;
            existingOrder.TotalAmount = newItems.Sum(i => i.Quantity * i.UnitPrice);
            existingOrder.Status = dto.Status ?? existingOrder.Status; // Keep existing status if not provided

            await _context.SaveChangesAsync();
        }

        public async Task DeletePurchaseOrderAsync(int id)
        {
            var order = await _context.PurchaseOrders
                .Include(po => po.Items)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (order == null)
                throw new KeyNotFoundException($"Purchase order with ID {id} not found");

            _context.PurchaseOrderItems.RemoveRange(order.Items);
            _context.PurchaseOrders.Remove(order);

            await _context.SaveChangesAsync();
        }

        public async Task<PurchaseOrder> GetPurchaseOrderByIdAsync(int id)
        {
            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.Items)
                .FirstOrDefaultAsync(po => po.Id == id);

            if (purchaseOrder == null)
                throw new KeyNotFoundException($"Purchase order with ID {id} not found");

            return purchaseOrder;
        }

        private IQueryable<PurchaseOrder> GetPurchaseOrdersWithIncludes()
        {
            return _context.PurchaseOrders
                .Include(po => po.Vendor)
                .Include(po => po.Items)
                .ThenInclude(i => i.Product);
        }
    }
}
