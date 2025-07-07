namespace PurchaseManagement.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string? Unit { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
