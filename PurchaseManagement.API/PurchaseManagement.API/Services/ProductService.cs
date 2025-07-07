using Microsoft.EntityFrameworkCore;
using PurchaseManagement.API.Data;
using PurchaseManagement.API.Models;
using PurchaseManagement.API.Services.Interfaces;

namespace PurchaseManagement.API.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ApplicationDbContext context, ILogger<ProductService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            _logger.LogInformation("Service: Getting all products");

            try
            {
                var products = await _context.Products.ToListAsync();
                _logger.LogInformation("Service: Successfully retrieved {Count} products", products.Count);
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error retrieving all products");
                throw;
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            _logger.LogInformation("Service: Getting product with ID: {ProductId}", id);

            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("Service: Product with ID {ProductId} not found", id);
                    
                }
                else
                {
                    _logger.LogInformation("Service: Successfully retrieved product {ProductName} (ID: {ProductId})",
                        product.Name, id);
                }

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error retrieving product with ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            _logger.LogInformation("Service: Creating new product: {ProductName}", product.Name);

            try
            {
                // Validate product code uniqueness
                var existingProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Code == product.Code);

                if (existingProduct != null)
                {
                    _logger.LogWarning("Service: Product with code {ProductCode} already exists", product.Code);
                    throw new InvalidOperationException($"Product with code '{product.Code}' already exists");
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Service: Product created successfully with ID: {ProductId}", product.Id);
                return product;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw validation errors
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error creating product: {ProductName}", product.Name);
                throw;
            }
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            _logger.LogInformation("Service: Updating product with ID: {ProductId}", product.Id);

            try
            {
                // Check if product exists
                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null)
                {
                    _logger.LogWarning("Service: Attempted to update non-existent product with ID: {ProductId}", product.Id);
                    throw new InvalidOperationException($"Product with ID {product.Id} not found");
                }

                // Validate product code uniqueness (excluding current product)
                var duplicateProduct = await _context.Products
                    .FirstOrDefaultAsync(p => p.Code == product.Code && p.Id != product.Id);

                if (duplicateProduct != null)
                {
                    _logger.LogWarning("Service: Product code {ProductCode} already exists for another product", product.Code);
                    throw new InvalidOperationException($"Product with code '{product.Code}' already exists");
                }

                // Update properties
                existingProduct.Name = product.Name;
                existingProduct.Code = product.Code;
                existingProduct.Unit = product.Unit;
                existingProduct.UnitPrice = product.UnitPrice;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Service: Product updated successfully. ID: {ProductId}", product.Id);
                return existingProduct;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw validation errors
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error updating product with ID: {ProductId}", product.Id);
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            _logger.LogInformation("Service: Deleting product with ID: {ProductId}", id);

            try
            {
                var product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("Service: Attempted to delete non-existent product with ID: {ProductId}", id);
                    return false;
                }

                // Check if product is used in any purchase order items
                var isUsedInOrders = await _context.PurchaseOrderItems
                    .AnyAsync(poi => poi.ProductId == id);

                if (isUsedInOrders)
                {
                    _logger.LogWarning("Service: Cannot delete product {ProductId} - it's used in purchase orders", id);
                    throw new InvalidOperationException("Cannot delete product that is used in purchase orders");
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Service: Product deleted successfully. ID: {ProductId}, Name: {ProductName}",
                    id, product.Name);

                return true;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw validation errors
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error deleting product with ID: {ProductId}", id);
                throw;
            }
        }

        public async Task<bool> ProductExistsAsync(int id)
        {
            _logger.LogInformation("Service: Checking if product exists with ID: {ProductId}", id);

            try
            {
                var exists = await _context.Products.AnyAsync(p => p.Id == id);
                _logger.LogInformation("Service: Product {ProductId} exists: {Exists}", id, exists);
                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Service: Error checking product existence with ID: {ProductId}", id);
                throw;
            }
        }
    }
}