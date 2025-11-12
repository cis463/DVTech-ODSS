using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVTech_ODSS.Models
{
    public class InventoryItem
    {
        [Key]
        public int ItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Brand { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int MinimumStockLevel { get; set; }

        [Required]
        public int HighStockLevel { get; set; } = 100;

        [Required]
        [StringLength(20)]
        public string Unit { get; set; } = string.Empty;

        // PRICING FIELDS - PHASE 1 MARKUP FEATURE
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }  // Cost price (what you paid supplier)

        [Column(TypeName = "decimal(18,2)")]
        public decimal MarkupPercentage { get; set; } = 25;  // Default 25% markup

        [Column(TypeName = "decimal(18,2)")]
        public decimal SellingPrice { get; set; }  // Price you sell to customers

        // Timestamps
        public DateTime DateAdded { get; set; } = DateTime.Now;

        public DateTime? LastRestocked { get; set; }

        // Supplier information for tracking
        public int? LastSupplierId { get; set; }

        [StringLength(100)]
        public string? LastSupplierName { get; set; }

        // Keep these for DB compatibility
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active";

        public bool IsActive { get; set; } = true;

        // Archive fields
        public bool IsArchived { get; set; } = false;
        public DateTime? ArchivedDate { get; set; }

        // Computed properties for UI
        [NotMapped]
        public string StockStatus
        {
            get
            {
                if (Quantity >= HighStockLevel)
                    return "High Stock";
                else if (Quantity > MinimumStockLevel)
                    return "Good Stock";
                else if (Quantity > 0)
                    return "Low Stock";
                else
                    return "Out of Stock";
            }
        }

        [NotMapped]
        public decimal ProfitPerUnit => SellingPrice - UnitPrice;

        [NotMapped]
        public decimal TotalCostValue => Quantity * UnitPrice;

        [NotMapped]
        public decimal TotalSellingValue => Quantity * SellingPrice;

        [NotMapped]
        public decimal TotalProfitPotential => Quantity * ProfitPerUnit;

        // Method to calculate selling price based on markup
        public void CalculateSellingPrice()
        {
            SellingPrice = UnitPrice * (1 + (MarkupPercentage / 100));
        }

        // Method to calculate markup from cost and selling price
        public void CalculateMarkupFromPrices()
        {
            if (UnitPrice > 0)
            {
                MarkupPercentage = ((SellingPrice - UnitPrice) / UnitPrice) * 100;
            }
        }
    }
}