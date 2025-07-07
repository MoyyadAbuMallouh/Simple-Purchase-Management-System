using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PurchaseManagement.API.DTOs;
using PurchaseManagement.API.Models;
using PurchaseManagement.API.Services.Interfaces;

namespace PurchaseManagement.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMappingService _mappingService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, IMappingService mappingService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _mappingService = mappingService;
            _logger = logger;
        }

        // GET: api/products
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            _logger.LogInformation("Controller: Getting all products");

            try
            {
                var products = await _productService.GetAllProductsAsync();
                var productDtos = _mappingService.MapToProductDtos(products);
                _logger.LogInformation("Controller: Successfully retrieved products");
                return Ok(productDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error retrieving products");
                return BadRequest(new { message = "Error retrieving products" });
            }
        }


        // GET: api/products/summary (for dropdowns, etc.)
        //[HttpGet("summary")]
        //public async Task<ActionResult<IEnumerable<ProductSummaryDto>>> GetProductsSummary()
        //{
        //    _logger.LogInformation("Controller: Getting products summary");

        //    try
        //    {
        //        var products = await _productService.GetAllProductsAsync();
        //        var productSummaryDtos = _mappingService.MapToProductSummaryDtos(products);

        //        _logger.LogInformation("Controller: Successfully retrieved {Count} products summary", productSummaryDtos.Count());
        //        return Ok(productSummaryDtos);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Controller: Error retrieving products summary");
        //        return BadRequest(new { message = "Error retrieving products summary" });
        //    }
        //}


        // GET: api/products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            _logger.LogInformation("Controller: Getting product with ID: {ProductId}", id);

            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    _logger.LogWarning("Controller: Product with ID {ProductId} not found", id);
                    return NotFound(new { message = $"Product with ID {id} not found" });
                }

                var productDto = _mappingService.MapToProductDto(product);
                _logger.LogInformation("Controller: Successfully retrieved product {ProductId}", id);
                return Ok(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error retrieving product with ID: {ProductId}", id);
                return BadRequest(new { message = "Error retrieving product" });
            }
        }

        // POST: api/products
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            _logger.LogInformation("Controller: Creating new product: {ProductName}", createProductDto.Name);

            // Model validation is handled by data annotations
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var product = _mappingService.MapToProduct(createProductDto);
                var createdProduct = await _productService.CreateProductAsync(product);
                var productDto = _mappingService.MapToProductDto(createdProduct);

                _logger.LogInformation("Controller: Product created successfully with ID: {ProductId}", createdProduct.Id);

                return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, productDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Controller: Validation error creating product: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error creating product: {ProductName}", createProductDto.Name);
                return BadRequest(new { message = "Error creating product" });
            }
        }

        // PUT: api/products/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            _logger.LogInformation("Controller: Updating product with ID: {ProductId}", id);

            // Model validation is handled by data annotations
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var product = _mappingService.MapToProduct(updateProductDto, id);
                var updatedProduct = await _productService.UpdateProductAsync(product);
                var productDto = _mappingService.MapToProductDto(updatedProduct);

                _logger.LogInformation("Controller: Product updated successfully. ID: {ProductId}", id);
                return Ok(productDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Controller: Validation error updating product: {Message}", ex.Message);

                // Check if it's a not found error
                if (ex.Message.Contains("not found"))
                {
                    return NotFound(new { message = ex.Message });
                }

                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error updating product. ID: {ProductId}", id);
                return BadRequest(new { message = "Error updating product" });
            }
        }

        // DELETE: api/products/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            _logger.LogInformation("Controller: Deleting product with ID: {ProductId}", id);

            try
            {
                var result = await _productService.DeleteProductAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Controller: Product with ID {ProductId} not found for deletion", id);
                    return NotFound(new { message = $"Product with ID {id} not found" });
                }

                _logger.LogInformation("Controller: Product deleted successfully. ID: {ProductId}", id);
                return Ok(new { message = "Product deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Controller: Cannot delete product: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error deleting product. ID: {ProductId}", id);
                return BadRequest(new { message = "Error deleting product" });
            }
        }

        // GET: api/products/exists/5
        [HttpGet("exists/{id}")]
        public async Task<ActionResult<bool>> ProductExists(int id)
        {
            _logger.LogInformation("Controller: Checking if product exists with ID: {ProductId}", id);

            try
            {
                var exists = await _productService.ProductExistsAsync(id);
                return Ok(new { exists = exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Controller: Error checking product existence. ID: {ProductId}", id);
                return BadRequest(new { message = "Error checking product existence" });
            }
        }

        //[HttpGet("test-error")]
        //public ActionResult TestError()
        //{
        //    _logger.LogError("Controller: Testing error logging functionality");
        //    throw new Exception("This is a test error for logging");
        //}

        //[HttpGet("test-service-error")]
        //public async Task<ActionResult> TestServiceError()
        //{
        //    _logger.LogInformation("Controller: Testing service layer error handling");

        //    try
        //    {
        //        // This will cause a service error
        //        await _productService.GetProductByIdAsync(-1);
        //        return Ok();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Controller: Caught service error in test endpoint");
        //        return BadRequest(new { message = "Service error occurred", details = ex.Message });
        //    }
        //}
    }
}