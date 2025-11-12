using DVTech_ODSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DVTech_ODSS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<SupplierItem> SupplierItems { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleItem> SaleItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for Sale
            modelBuilder.Entity<Sale>()
                .Property(s => s.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sale>()
                .Property(s => s.TotalCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Sale>()
                .Property(s => s.TotalProfit)
                .HasPrecision(18, 2);

            // Configure decimal precision for SaleItem
            modelBuilder.Entity<SaleItem>()
                .Property(si => si.UnitCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SaleItem>()
                .Property(si => si.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SaleItem>()
                .Property(si => si.TotalCost)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SaleItem>()
                .Property(si => si.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SaleItem>()
                .Property(si => si.Profit)
                .HasPrecision(18, 2);

            // Configure relationships
            modelBuilder.Entity<Sale>()
                .HasMany(s => s.SaleItems)
                .WithOne(si => si.Sale)
                .HasForeignKey(si => si.SaleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SaleItem>()
                .HasOne(si => si.InventoryItem)
                .WithMany()
                .HasForeignKey(si => si.ItemId);

            // Configure decimal precision for InventoryItem
            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.MarkupPercentage)
                .HasPrecision(5, 2);

            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.SellingPrice)
                .HasPrecision(18, 2);

            // Configure decimal precision for SupplierItem
            modelBuilder.Entity<SupplierItem>()
                .Property(si => si.PricePerUnit)
                .HasPrecision(18, 2);

            // Configure decimal precision for PurchaseOrder
            modelBuilder.Entity<PurchaseOrder>()
                .Property(po => po.TotalAmount)
                .HasPrecision(18, 2);

            // Configure decimal precision for PurchaseOrderItem
            modelBuilder.Entity<PurchaseOrderItem>()
                .Property(poi => poi.UnitPrice)
                .HasPrecision(18, 2);

            // Configure relationships
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.SupplierId);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(poi => poi.PurchaseOrder)
                .WithMany(po => po.OrderItems)
                .HasForeignKey(poi => poi.OrderId);

            modelBuilder.Entity<PurchaseOrderItem>()
                .HasOne(poi => poi.SupplierItem)
                .WithMany()
                .HasForeignKey(poi => poi.SupplierItemId);

            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Employee)
                .WithMany(e => e.Schedules)
                .HasForeignKey(s => s.EmployeeId);

            modelBuilder.Entity<SupplierItem>()
                .HasOne(si => si.Supplier)
                .WithMany(s => s.SupplierItems)
                .HasForeignKey(si => si.SupplierId);
        }
    }
}