using DVTech_ODSS.Data;
using DVTech_ODSS.Models;
using Microsoft.EntityFrameworkCore;

namespace DVTech_ODSS.Services
{
    public class ProcurementService
    {
        private readonly ApplicationDbContext _context;

        public ProcurementService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ===== SUPPLIER METHODS =====

        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers
                .Where(s => s.IsActive)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers.FindAsync(id);
        }

        public async Task<bool> AddSupplierAsync(Supplier supplier)
        {
            try
            {
                supplier.DateAdded = DateTime.Now;
                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateSupplierAsync(Supplier supplier)
        {
            try
            {
                _context.Suppliers.Update(supplier);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteSupplierAsync(int id)
        {
            try
            {
                var supplier = await GetSupplierByIdAsync(id);
                if (supplier != null)
                {
                    supplier.IsActive = false;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        // ===== PURCHASE ORDER METHODS =====

        public async Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync()
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.OrderItems)
                    .ThenInclude(oi => oi.InventoryItem)
                .Where(po => po.IsActive)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.OrderItems)
                    .ThenInclude(oi => oi.InventoryItem)
                .FirstOrDefaultAsync(po => po.OrderId == id);
        }

        public async Task<bool> AddPurchaseOrderAsync(PurchaseOrder order)
        {
            try
            {
                // Generate order number
                var lastOrder = await _context.PurchaseOrders
                    .OrderByDescending(po => po.OrderId)
                    .FirstOrDefaultAsync();

                var orderCount = lastOrder != null ? lastOrder.OrderId + 1 : 1;
                order.OrderNumber = $"PO-{DateTime.Now:yyyyMMdd}-{orderCount:D4}";
                order.OrderDate = DateTime.Now;

                _context.PurchaseOrders.Add(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdatePurchaseOrderAsync(PurchaseOrder order)
        {
            try
            {
                _context.PurchaseOrders.Update(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                var order = await GetPurchaseOrderByIdAsync(orderId);
                if (order != null)
                {
                    order.Status = status;
                    if (status == "Delivered")
                    {
                        order.ActualDeliveryDate = DateTime.Now;
                    }
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(string status)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Where(po => po.IsActive && po.Status == status)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }
    }
}