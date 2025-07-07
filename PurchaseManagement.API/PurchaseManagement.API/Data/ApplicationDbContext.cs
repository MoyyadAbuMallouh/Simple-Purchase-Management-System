using Microsoft.EntityFrameworkCore;
using PurchaseManagement.API.Models;

namespace PurchaseManagement.API.Data
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions <ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Users> Users { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        // OnModelCreating: Where you EXPLICITLY configure your database schema
        // This is where you define relationships, constraints, and validations
        // Instead of letting EF "guess" what you want, you tell it exactly what to do
        // This ensures data integrity and proper relationships at the database level
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure relationships and constraints here
            // Examples: unique indexes, foreign keys, data validation rules, etc.
            modelBuilder.Entity<Product>()
            .HasIndex(p => p.Code)
            .IsUnique();

            // Configure relationships and constraints
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.Vendor)
                .WithMany()
                .HasForeignKey(po => po.VendorId);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(poi => poi.PurchaseOrder)
                .WithMany(po => po.Items)
                .HasForeignKey(poi => poi.PurchaseOrderId);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(poi => poi.Product)
                .WithMany()
                .HasForeignKey(poi => poi.ProductId);

            base.OnModelCreating(modelBuilder);
        }

    }
}
