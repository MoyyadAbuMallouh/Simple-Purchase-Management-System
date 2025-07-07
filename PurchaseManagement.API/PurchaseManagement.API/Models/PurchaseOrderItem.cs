namespace PurchaseManagement.API.Models
{
    public class PurchaseOrderItem
    {
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Navigation properties
        public PurchaseOrder PurchaseOrder { get; set; } = null!; // This creates the foreign key relationship
        public Product Product { get; set; } = null!; // This creates the foreign key relationship
    }
}
