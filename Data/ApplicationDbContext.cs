using Microsoft.EntityFrameworkCore;
using DVTech_ODSS.Models;


namespace DVTech_ODSS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for each module
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Schedule> Schedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for prices
            modelBuilder.Entity<InventoryItem>()
                .Property(i => i.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseOrder>()
                .Property(po => po.TotalAmount)
                .HasPrecision(18, 2);

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
                .HasOne(poi => poi.InventoryItem)
                .WithMany()
                .HasForeignKey(poi => poi.ItemId);

            // Configure Employee-Schedule relationship
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Employee)
                .WithMany(e => e.Schedules)
                .HasForeignKey(s => s.EmployeeId);
        }
    }
}