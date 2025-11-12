using DVTech_ODSS.Models;
using System.Security.Cryptography;
using System.Text;

namespace DVTech_ODSS.Data
{
    public static class DbSeeder
    {
        public static async Task SeedInventoryData(ApplicationDbContext context)
        {
            // No longer seed inventory items
            // Items will be added automatically when purchase orders are received

            // You can optionally seed some sample suppliers and purchase orders for testing
            await SeedSuppliersAndOrders(context);
        }

        private static async Task SeedSuppliersAndOrders(ApplicationDbContext context)
        {
            // Check if suppliers already exist
            if (context.Suppliers.Any())
                return;

            var suppliers = new List<Supplier>
            {
                new Supplier
                {
                    SupplierName = "Cool Air Philippines Inc.",
                    ContactPerson = "Juan Dela Cruz",
                    PhoneNumber = "02-1234-5678",
                    Email = "sales@coolair.ph",
                    Address = "123 EDSA, Quezon City",
                    Status = "Active",
                    DateAdded = DateTime.Now
                },
                new Supplier
                {
                    SupplierName = "AC Parts Depot",
                    ContactPerson = "Maria Santos",
                    PhoneNumber = "02-9876-5432",
                    Email = "orders@acpartsdepot.com",
                    Address = "456 Makati Ave, Makati City",
                    Status = "Active",
                    DateAdded = DateTime.Now
                },
                new Supplier
                {
                    SupplierName = "TechCool Supply Co.",
                    ContactPerson = "Pedro Reyes",
                    PhoneNumber = "02-5555-1234",
                    Email = "info@techcool.ph",
                    Address = "789 Ortigas Center, Pasig City",
                    Status = "Active",
                    DateAdded = DateTime.Now
                }
            };

            context.Suppliers.AddRange(suppliers);
            await context.SaveChangesAsync();
        }

        public static async Task SeedDefaultUsers(ApplicationDbContext context)
        {
            // Check if users already exist
            if (context.Users.Any())
                return;

            var users = new List<User>
            {
                new User
                {
                    Username = "admin",
                    PasswordHash = HashPassword("admin123"),
                    FullName = "System Administrator",
                    Role = "Admin",
                    Email = "admin@dvtech.com",
                    DateCreated = DateTime.Now,
                    IsActive = true
                },
                new User
                {
                    Username = "secretary",
                    PasswordHash = HashPassword("secretary123"),
                    FullName = "Office Secretary",
                    Role = "Secretary",
                    Email = "secretary@dvtech.com",
                    DateCreated = DateTime.Now,
                    IsActive = true
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }

        private static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
