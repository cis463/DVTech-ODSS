using DVTech_ODSS.Data;
using DVTech_ODSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DVTech_ODSS.Services
{
    public class SalesService
    {
        private readonly ApplicationDbContext _context;
        private readonly InventoryService _inventoryService;

        public SalesService(ApplicationDbContext context, InventoryService inventoryService)
        {
            _context = context;
            _inventoryService = inventoryService;
        }

        public async Task<List<Sale>> GetAllSalesAsync()
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.InventoryItem)
                .Where(s => s.IsActive)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<Sale?> GetSaleByIdAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                    .ThenInclude(si => si.InventoryItem)
                .FirstOrDefaultAsync(s => s.SaleId == id);
        }

        public async Task<(bool success, string message)> CreateSaleAsync(Sale sale)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Generate sale number
                var lastSale = await _context.Sales
                    .OrderByDescending(s => s.SaleId)
                    .FirstOrDefaultAsync();

                var saleCount = lastSale != null ? lastSale.SaleId + 1 : 1;
                sale.SaleNumber = $"SALE-{DateTime.Now:yyyyMMdd}-{saleCount:D4}";
                sale.SaleDate = DateTime.Now;
                sale.CreatedDate = DateTime.Now;

                // Validate and deduct inventory
                foreach (var item in sale.SaleItems)
                {
                    var inventoryItem = await _inventoryService.GetItemByIdAsync(item.ItemId);

                    if (inventoryItem == null)
                    {
                        await transaction.RollbackAsync();
                        return (false, $"Item {item.ItemName} not found in inventory.");
                    }

                    if (inventoryItem.Quantity < item.Quantity)
                    {
                        await transaction.RollbackAsync();
                        return (false, $"Insufficient stock for {item.ItemName}. Available: {inventoryItem.Quantity}, Required: {item.Quantity}");
                    }

                    // Deduct from inventory
                    inventoryItem.Quantity -= item.Quantity;
                    await _inventoryService.UpdateItemAsync(inventoryItem);

                    // Calculate costs and profit
                    item.UnitCost = inventoryItem.UnitPrice;
                    item.UnitPrice = inventoryItem.SellingPrice;
                    item.TotalCost = item.Quantity * item.UnitCost;
                    item.TotalPrice = item.Quantity * item.UnitPrice;
                    item.Profit = item.TotalPrice - item.TotalCost;
                }

                // Calculate sale totals
                sale.TotalCost = sale.SaleItems.Sum(si => si.TotalCost);
                sale.TotalAmount = sale.SaleItems.Sum(si => si.TotalPrice);
                sale.TotalProfit = sale.TotalAmount - sale.TotalCost;

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Sale completed successfully!");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Error processing sale: {ex.Message}");
            }
        }

        public async Task<List<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Sales
                .Include(s => s.SaleItems)
                .Where(s => s.IsActive && s.SaleDate >= startDate && s.SaleDate <= endDate)
                .OrderByDescending(s => s.SaleDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalSalesTodayAsync()
        {
            var today = DateTime.Today;
            return await _context.Sales
                .Where(s => s.IsActive && s.SaleDate.Date == today)
                .SumAsync(s => s.TotalAmount);
        }

        public async Task<decimal> GetTotalProfitTodayAsync()
        {
            var today = DateTime.Today;
            return await _context.Sales
                .Where(s => s.IsActive && s.SaleDate.Date == today)
                .SumAsync(s => s.TotalProfit);
        }
    }
}