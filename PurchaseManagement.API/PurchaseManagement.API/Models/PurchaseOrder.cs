namespace PurchaseManagement.API.Models
{
    public class PurchaseOrder
    {
        public int Id { get; set; }
        public int VendorId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending";

        // Navigation properties
        public Vendor Vendor { get; set; } = null!; // This creates the foreign key relationship (Each PurchaseOrder belongs to one Vendor)
        public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>(); // This is the one-to-many relationship (Each PurchaseOrder can have many PurchaseOrderItems)

    }
}
