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
                .Include(s => s.SupplierItems.Where(si => si.IsActive))
                .Where(s => s.IsActive && !s.IsArchived)
                .OrderBy(s => s.SupplierName)
                .ToListAsync();
        }

        public async Task<Supplier?> GetSupplierByIdAsync(int id)
        {
            return await _context.Suppliers
                .Include(s => s.SupplierItems.Where(si => si.IsActive))
                .FirstOrDefaultAsync(s => s.SupplierId == id);
        }

        public async Task<bool> AddSupplierAsync(Supplier supplier)
        {
            try
            {
                supplier.DateAdded = DateTime.Now;

                foreach (var item in supplier.SupplierItems)
                {
                    item.DateAdded = DateTime.Now;
                    item.IsActive = true;
                }

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
                var existingSupplier = await _context.Suppliers
                    .Include(s => s.SupplierItems)
                    .FirstOrDefaultAsync(s => s.SupplierId == supplier.SupplierId);

                if (existingSupplier == null)
                    return false;

                existingSupplier.SupplierName = supplier.SupplierName;
                existingSupplier.ContactPerson = supplier.ContactPerson;
                existingSupplier.PhoneNumber = supplier.PhoneNumber;
                existingSupplier.Email = supplier.Email;
                existingSupplier.Address = supplier.Address;
                existingSupplier.Status = supplier.Status;

                var itemsToRemove = existingSupplier.SupplierItems
                    .Where(ei => !supplier.SupplierItems.Any(si => si.SupplierItemId == ei.SupplierItemId))
                    .ToList();

                foreach (var item in itemsToRemove)
                {
                    _context.SupplierItems.Remove(item);
                }

                foreach (var newItem in supplier.SupplierItems)
                {
                    var existingItem = existingSupplier.SupplierItems
                        .FirstOrDefault(ei => ei.SupplierItemId == newItem.SupplierItemId);

                    if (existingItem != null)
                    {
                        existingItem.ItemName = newItem.ItemName;
                        existingItem.Brand = newItem.Brand;
                        existingItem.Category = newItem.Category;
                        existingItem.Description = newItem.Description;
                        existingItem.PricePerUnit = newItem.PricePerUnit;
                        existingItem.Unit = newItem.Unit;
                        existingItem.IsActive = newItem.IsActive;
                    }
                    else
                    {
                        newItem.SupplierId = existingSupplier.SupplierId;
                        newItem.DateAdded = DateTime.Now;
                        newItem.IsActive = true;
                        existingSupplier.SupplierItems.Add(newItem);
                    }
                }

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

        // NEW: Get archived suppliers
        public async Task<List<Supplier>> GetArchivedSuppliersAsync()
        {
            return await _context.Suppliers
                .Include(s => s.SupplierItems.Where(si => si.IsActive))
                .Where(s => s.IsArchived)
                .OrderByDescending(s => s.ArchivedDate)
                .ToListAsync();
        }

        // NEW: Archive supplier (soft delete)
        public async Task<bool> ArchiveSupplierAsync(int id)
        {
            try
            {
                var supplier = await GetSupplierByIdAsync(id);
                if (supplier != null)
                {
                    supplier.IsArchived = true;
                    supplier.ArchivedDate = DateTime.Now;
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

        // NEW: Restore archived supplier
        public async Task<bool> RestoreSupplierAsync(int id)
        {
            try
            {
                var supplier = await _context.Suppliers
                    .Include(s => s.SupplierItems)
                    .FirstOrDefaultAsync(s => s.SupplierId == id);

                if (supplier == null || !supplier.IsArchived)
                    return false;

                supplier.IsArchived = false;
                supplier.ArchivedDate = null;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ===== SUPPLIER ITEM METHODS =====

        public async Task<List<SupplierItem>> GetSupplierItemsAsync(int supplierId)
        {
            return await _context.SupplierItems
                .Where(si => si.SupplierId == supplierId && si.IsActive)
                .OrderBy(si => si.ItemName)
                .ToListAsync();
        }

        public async Task<bool> AddSupplierItemAsync(SupplierItem item)
        {
            try
            {
                item.DateAdded = DateTime.Now;
                _context.SupplierItems.Add(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateSupplierItemAsync(SupplierItem item)
        {
            try
            {
                _context.SupplierItems.Update(item);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteSupplierItemAsync(int id)
        {
            try
            {
                var item = await _context.SupplierItems.FindAsync(id);
                if (item != null)
                {
                    item.IsActive = false;
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
                    .ThenInclude(oi => oi.SupplierItem)
                .Where(po => po.IsActive)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.OrderItems)
                    .ThenInclude(oi => oi.SupplierItem)
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

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, string updatedBy = "Admin")
        {
            try
            {
                var order = await GetPurchaseOrderByIdAsync(orderId);
                if (order != null)
                {
                    order.Status = status;

                    // Track status change timestamps
                    switch (status)
                    {
                        case "Approved":
                            order.ApprovedDate = DateTime.Now;
                            order.ApprovedBy = updatedBy;
                            break;
                        case "Ordered":
                            order.OrderedDate = DateTime.Now;
                            order.OrderedBy = updatedBy;
                            break;
                        case "Delivered":
                            order.DeliveredDate = DateTime.Now;
                            order.ActualDeliveryDate = DateTime.Now;
                            order.DeliveredBy = updatedBy;
                            await UpdateInventoryFromPurchaseOrder(order);
                            break;
                        case "Cancelled":
                            // Just update status - no inventory changes
                            break;
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

        private async Task UpdateInventoryFromPurchaseOrder(PurchaseOrder order)
        {
            foreach (var orderItem in order.OrderItems)
            {
                if (orderItem.SupplierItem == null)
                    continue;

                // Check if this exact item from this supplier already exists in inventory
                var existingInventoryItem = await _context.InventoryItems
                    .FirstOrDefaultAsync(i =>
                        i.ItemName == orderItem.ItemName &&
                        i.Brand == orderItem.Brand &&
                        i.Category == orderItem.Category &&
                        !i.IsArchived);

                if (existingInventoryItem != null)
                {
                    // Update existing inventory item
                    var oldQuantity = existingInventoryItem.Quantity;
                    existingInventoryItem.Quantity += orderItem.Quantity;
                    existingInventoryItem.LastRestocked = DateTime.Now;
                    existingInventoryItem.LastSupplierId = order.SupplierId;
                    existingInventoryItem.LastSupplierName = order.Supplier?.SupplierName;

                    // Update weighted average unit price (cost)
                    var totalValue = (oldQuantity * existingInventoryItem.UnitPrice) +
                                   (orderItem.Quantity * orderItem.UnitPrice);
                    existingInventoryItem.UnitPrice = totalValue / existingInventoryItem.Quantity;

                    // AUTOMATICALLY RECALCULATE SELLING PRICE BASED ON MARKUP
                    existingInventoryItem.CalculateSellingPrice();
                }
                else
                {
                    // Create NEW inventory item from supplier item
                    var newInventoryItem = new InventoryItem
                    {
                        ItemName = orderItem.ItemName,
                        Brand = orderItem.Brand,
                        Description = orderItem.SupplierItem.Description,
                        Category = orderItem.Category,
                        Quantity = orderItem.Quantity,
                        MinimumStockLevel = 10,  // Default values
                        HighStockLevel = 100,
                        Unit = orderItem.Unit,
                        UnitPrice = orderItem.UnitPrice,  // Cost price from supplier
                        MarkupPercentage = 25,  // Default 25% markup
                        DateAdded = DateTime.Now,
                        LastRestocked = DateTime.Now,
                        LastSupplierId = order.SupplierId,
                        LastSupplierName = order.Supplier?.SupplierName,
                        IsActive = true,
                        IsArchived = false,
                        Status = "Active"
                    };

                    // Calculate selling price based on cost + markup
                    newInventoryItem.CalculateSellingPrice();

                    _context.InventoryItems.Add(newInventoryItem);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(string status)
        {
            return await _context.PurchaseOrders
                .Include(po => po.Supplier)
                .Include(po => po.OrderItems)
                    .ThenInclude(oi => oi.SupplierItem)
                .Where(po => po.IsActive && po.Status == status)
                .OrderByDescending(po => po.OrderDate)
                .ToListAsync();
        }
    }
}