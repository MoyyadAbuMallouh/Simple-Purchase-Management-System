using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PurchaseManagement.API.DTOs
{
    public class PurchaseOrderDto
    {
        public int Id { get; set; }

        public int VendorId { get; set; }

        public string VendorName { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending";

        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }

    public class CreatePurchaseOrderDto
    {
        [Required(ErrorMessage = "Vendor is required")]
        public int VendorId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one product must be added")]
        public List<CreatePurchaseOrderItemDto> Items { get; set; } = new();
    }
    public class UpdatePurchaseOrderDto
    {
        [Required(ErrorMessage = "Vendor is required")]
        public int VendorId { get; set; }

        public string Status { get; set; } = "Pending";

        [Required]
        [MinLength(1, ErrorMessage = "At least one product must be added")]
        public List<CreatePurchaseOrderItemDto> Items { get; set; } = new();
    }


}
