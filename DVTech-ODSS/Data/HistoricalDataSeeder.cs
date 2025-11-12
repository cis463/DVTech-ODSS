using DVTech_ODSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DVTech_ODSS.Data
{
    public static class HistoricalDataSeeder
    {
        public static async Task SeedHistoricalSalesData(ApplicationDbContext context)
        {
            // Check if historical data already exists
            var existingSales = await context.Sales.CountAsync();
            if (existingSales > 50) // Already has substantial data
            {
                Console.WriteLine("Historical sales data already exists. Skipping seeding.");
                return;
            }

            Console.WriteLine("Seeding 5 years of historical sales data...");

            var random = new Random(12345); // Fixed seed for consistency
            var startDate = new DateTime(2020, 1, 1);
            var endDate = new DateTime(2024, 12, 31);

            // Get all inventory items
            var inventoryItems = await context.InventoryItems
                .Where(i => i.IsActive && !i.IsArchived)
                .ToListAsync();

            if (!inventoryItems.Any())
            {
                Console.WriteLine("No inventory items found. Cannot seed sales data.");
                return;
            }

            // Category seasonal multipliers
            var seasonalMultipliers = new Dictionary<string, Dictionary<int, double>>
            {
                ["AC Units"] = new() { [1] = 0.6, [2] = 1.2, [3] = 1.6, [4] = 0.8 },
                ["AC Parts"] = new() { [1] = 0.7, [2] = 1.3, [3] = 1.5, [4] = 0.9 },
                ["Tools"] = new() { [1] = 0.9, [2] = 1.1, [3] = 1.2, [4] = 0.95 },
                ["Consumables"] = new() { [1] = 0.8, [2] = 1.2, [3] = 1.4, [4] = 1.0 },
                ["Hardware"] = new() { [1] = 0.9, [2] = 1.0, [3] = 1.1, [4] = 1.0 }
            };

            var salesCreated = 0;
            var currentDate = startDate;

            while (currentDate <= endDate)
            {
                var quarter = (currentDate.Month - 1) / 3 + 1;

                // Generate 1-3 sales per day (more in peak season)
                var salesPerDay = random.Next(1, 4);

                for (int sale = 0; sale < salesPerDay; sale++)
                {
                    // Select 1-5 random items for this sale
                    var itemCount = random.Next(1, 6);
                    var saleItems = new List<SaleItem>();
                    var totalCost = 0m;
                    var totalAmount = 0m;

                    for (int i = 0; i < itemCount; i++)
                    {
                        var item = inventoryItems[random.Next(inventoryItems.Count)];

                        // Get seasonal multiplier
                        var categoryKey = seasonalMultipliers.ContainsKey(item.Category)
                            ? item.Category
                            : "Hardware";
                        var multiplier = seasonalMultipliers[categoryKey][quarter];

                        // Calculate quantity (seasonal adjustment)
                        var baseQty = random.Next(1, 6);
                        var adjustedQty = Math.Max(1, (int)(baseQty * multiplier));

                        var saleItem = new SaleItem
                        {
                            ItemId = item.ItemId,
                            ItemName = item.ItemName,
                            Brand = item.Brand,
                            Category = item.Category,
                            Unit = item.Unit,
                            Quantity = adjustedQty,
                            UnitCost = item.UnitPrice,
                            UnitPrice = item.SellingPrice,
                            TotalCost = adjustedQty * item.UnitPrice,
                            TotalPrice = adjustedQty * item.SellingPrice,
                            Profit = adjustedQty * (item.SellingPrice - item.UnitPrice)
                        };

                        saleItems.Add(saleItem);
                        totalCost += saleItem.TotalCost;
                        totalAmount += saleItem.TotalPrice;
                    }

                    // Create the sale
                    var saleNumber = $"HIST-{currentDate:yyyyMMdd}-{sale + 1:D4}";
                    var newSale = new Sale
                    {
                        SaleNumber = saleNumber,
                        SaleDate = currentDate.AddHours(random.Next(8, 18)).AddMinutes(random.Next(0, 60)),
                        CustomerName = GetRandomCustomerName(random),
                        CustomerContact = null,
                        TotalAmount = totalAmount,
                        TotalCost = totalCost,
                        TotalProfit = totalAmount - totalCost,
                        PaymentMethod = GetRandomPaymentMethod(random),
                        Notes = null,
                        CreatedBy = "System (Historical Data)",
                        CreatedDate = currentDate,
                        IsActive = true,
                        SaleItems = saleItems
                    };

                    context.Sales.Add(newSale);
                    salesCreated++;

                    // Save in batches of 100 to avoid memory issues
                    if (salesCreated % 100 == 0)
                    {
                        await context.SaveChangesAsync();
                        Console.WriteLine($"Seeded {salesCreated} historical sales...");
                    }
                }

                currentDate = currentDate.AddDays(1);
            }

            // Final save
            await context.SaveChangesAsync();
            Console.WriteLine($"✅ Successfully seeded {salesCreated} historical sales records!");
        }

        private static string GetRandomCustomerName(Random random)
        {
            var names = new[]
            {
                "Walk-in Customer",
                "Corporate Client",
                "Retail Customer",
                "Government Contract",
                "Regular Customer",
                "New Customer",
                null // Some sales without customer name
            };
            return names[random.Next(names.Length)];
        }

        private static string GetRandomPaymentMethod(Random random)
        {
            var methods = new[] { "Cash", "GCash", "Bank Transfer", "Credit Card" };
            return methods[random.Next(methods.Length)];
        }
    }
}