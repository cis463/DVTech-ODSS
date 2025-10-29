using DVTech_ODSS.Models;

namespace DVTech_ODSS.Data
{
    public static class DbSeeder
    {
        public static async Task SeedInventoryData(ApplicationDbContext context)
        {
            // Check if data already exists
            if (context.InventoryItems.Any())
                return;

            var items = new List<InventoryItem>
            {
                new InventoryItem
                {
                    ItemName = "Laptop Dell Inspiron 15",
                    Description = "15.6 inch display, Intel i5, 8GB RAM, 512GB SSD",
                    Category = "Electronics",
                    Quantity = 15,
                    MinimumStockLevel = 5,
                    Unit = "pcs",
                    UnitPrice = 35000,
                    DateAdded = DateTime.Now.AddMonths(-3),
                    Status = "Active"
                },
                new InventoryItem
                {
                    ItemName = "Office Chair Ergonomic",
                    Description = "Adjustable height, lumbar support, mesh back",
                    Category = "Furniture",
                    Quantity = 3,
                    MinimumStockLevel = 5,
                    Unit = "pcs",
                    UnitPrice = 8500,
                    DateAdded = DateTime.Now.AddMonths(-2),
                    Status = "Low Stock"
                },
                new InventoryItem
                {
                    ItemName = "A4 Bond Paper",
                    Description = "70gsm, white, 500 sheets per ream",
                    Category = "Office Supplies",
                    Quantity = 50,
                    MinimumStockLevel = 20,
                    Unit = "reams",
                    UnitPrice = 180,
                    DateAdded = DateTime.Now.AddMonths(-1),
                    Status = "Active"
                },
                new InventoryItem
                {
                    ItemName = "USB Flash Drive 32GB",
                    Description = "USB 3.0, high-speed data transfer",
                    Category = "Electronics",
                    Quantity = 8,
                    MinimumStockLevel = 10,
                    Unit = "pcs",
                    UnitPrice = 450,
                    DateAdded = DateTime.Now.AddDays(-15),
                    Status = "Low Stock"
                },
                new InventoryItem
                {
                    ItemName = "Whiteboard Marker",
                    Description = "Assorted colors, dry-erase",
                    Category = "Office Supplies",
                    Quantity = 120,
                    MinimumStockLevel = 30,
                    Unit = "pcs",
                    UnitPrice = 35,
                    DateAdded = DateTime.Now.AddDays(-10),
                    Status = "Active"
                },
                new InventoryItem
                {
                    ItemName = "Network Cable Cat6",
                    Description = "10 meters, RJ45 connectors",
                    Category = "IT Equipment",
                    Quantity = 25,
                    MinimumStockLevel = 15,
                    Unit = "pcs",
                    UnitPrice = 250,
                    DateAdded = DateTime.Now.AddDays(-5),
                    Status = "Active"
                }
            };

            context.InventoryItems.AddRange(items);
            await context.SaveChangesAsync();
        }
    }
}