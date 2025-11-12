using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVTech_ODSS.Models
{
    public class PurchaseOrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public PurchaseOrder? PurchaseOrder { get; set; }

        // CHANGED: Reference SupplierItem instead of InventoryItem
        [Required]
        public int SupplierItemId { get; set; }

        [ForeignKey("SupplierItemId")]
        public SupplierItem? SupplierItem { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;

        [StringLength(200)]
        public string? Notes { get; set; }

        // For display purposes - store item info at time of order
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Brand { get; set; }

        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [StringLength(20)]
        public string Unit { get; set; } = string.Empty;
    }
}
