using System.ComponentModel.DataAnnotations;

namespace PurchaseManagement.API.DTOs
{
    // DTO for creating a new product
    public class CreateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product code is required")]
        [StringLength(50, ErrorMessage = "Product code cannot exceed 50 characters")]
        public string Code { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Unit cannot exceed 20 characters")]
        public string? Unit { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Unit price must be between 0.01 and 999999.99")]
        public decimal UnitPrice { get; set; }
    }

    // DTO for updating an existing product
    public class UpdateProductDto
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product code is required")]
        [StringLength(50, ErrorMessage = "Product code cannot exceed 50 characters")]
        public string Code { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Unit cannot exceed 20 characters")]
        public string? Unit { get; set; }

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Unit price must be between 0.01 and 999999.99")]
        public decimal UnitPrice { get; set; }
    }

    // DTO for returning product data
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Unit { get; set; }
        public decimal UnitPrice { get; set; }
    }

    //// DTO for product summary (for dropdowns, etc.)
    //public class ProductSummaryDto
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; } = string.Empty;
    //    public string Code { get; set; } = string.Empty;
    //    public decimal UnitPrice { get; set; }
    //}
}