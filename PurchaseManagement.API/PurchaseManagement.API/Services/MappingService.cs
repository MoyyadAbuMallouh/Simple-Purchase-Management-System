using PurchaseManagement.API.DTOs;
using PurchaseManagement.API.Models;
using PurchaseManagement.API.Services.Interfaces;

namespace PurchaseManagement.API.Services
{
    public class MappingService : IMappingService
    {
        // Constructor no longer needs IMapper
        public MappingService() { }

        // Product mappings
        public ProductDto MapToProductDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Code = product.Code,
                Unit = product.Unit,
                UnitPrice = product.UnitPrice
            };
        }

        public IEnumerable<ProductDto> MapToProductDtos(IEnumerable<Product> products)
        {
            return products.Select(MapToProductDto);
        }

        public Product MapToProduct(CreateProductDto dto)
        {
            return new Product
            {
                Name = dto.Name,
                Code = dto.Code,
                Unit = dto.Unit,
                UnitPrice = dto.UnitPrice
            };
        }

        public Product MapToProduct(UpdateProductDto dto, int id)
        {
            return new Product
            {
                Id = id,
                Name = dto.Name,
                Code = dto.Code,
                Unit = dto.Unit,
                UnitPrice = dto.UnitPrice
            };
        }

        // Vendor mappings
        public VendorDto MapToVendorDto(Vendor vendor)
        {
            return new VendorDto
            {
                Id = vendor.Id,
                Name = vendor.Name,
                Address = vendor.Address,
                ContactPerson = vendor.ContactPerson,
                Phone = vendor.Phone,
                Email = vendor.Email
            };
        }

        public IEnumerable<VendorDto> MapToVendorDtos(IEnumerable<Vendor> vendors)
        {
            return vendors.Select(MapToVendorDto);
        }

        public Vendor MapToVendor(CreateVendorDto dto)
        {
            return new Vendor
            {
                Name = dto.Name,
                Address = dto.Address,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Email = dto.Email
            };
        }

        public Vendor MapToVendor(UpdateVendorDto dto, int id)
        {
            return new Vendor
            {
                Id = id,
                Name = dto.Name,
                Address = dto.Address,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Email = dto.Email
            };
        }

        // PurchaseOrder mappings
        public PurchaseOrderDto MapToPurchaseOrderDto(PurchaseOrder order)
        {
            return new PurchaseOrderDto
            {
                Id = order.Id,
                VendorId = order.VendorId,
                VendorName = order.Vendor?.Name ?? "",
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                Items = order.Items.Select(i => new PurchaseOrderItemDto
                {
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name ?? "",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }

        public IEnumerable<PurchaseOrderDto> MapToPurchaseOrderDtos(IEnumerable<PurchaseOrder> orders)
        {
            return orders.Select(MapToPurchaseOrderDto);
        }

        public PurchaseOrder MapToPurchaseOrder(CreatePurchaseOrderDto dto)
        {
            return new PurchaseOrder
            {
                VendorId = dto.VendorId,
                Items = dto.Items.Select(i => new PurchaseOrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public PurchaseOrder MapToPurchaseOrder(CreatePurchaseOrderDto dto, int id)
        {
            var order = MapToPurchaseOrder(dto);
            order.Id = id;
            return order;
        }

        public PurchaseOrder MapToPurchaseOrder(UpdatePurchaseOrderDto dto, int id)
        {
            return new PurchaseOrder
            {
                Id = id,
                VendorId = dto.VendorId,
                OrderDate = DateTime.UtcNow,
                Status = "Pending",
                Items = dto.Items.Select(i => new PurchaseOrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        //user mapping
        public UserDto MapToUserDto(Users user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public IEnumerable<UserDto> MapToUserDtos(IEnumerable<Users> users)
        {
            return users.Select(MapToUserDto);
        }

        public Users MapToUser(CreateUserDto dto)
        {
            return new Users
            {
                Username = dto.Username,
                Role = "User",
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void ApplyUpdate(Users user, UpdateUserDto dto)
        {
            if (!string.IsNullOrEmpty(dto.Username))
                user.Username = dto.Username;
            if (!string.IsNullOrEmpty(dto.Role))
                user.Role = dto.Role;
            // Fix: Removed HasValue check since IsActive is not nullable
            user.IsActive = dto.IsActive;
        }
    }
}
