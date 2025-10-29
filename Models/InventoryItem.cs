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

        public decimal UnitPrice { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public DateTime? LastRestocked { get; set; }

        // Keep a persisted Status string for DB compatibility with existing migrations / snapshot.
        // This is what older migrations and DbSeeder expect.
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Active";

        // Keep IsActive for compatibility with existing migrations/snapshots that refer to it.
        public bool IsActive { get; set; } = true;

        // New archive fields (mapped)
        public bool IsArchived { get; set; } = false;
        public DateTime? ArchivedDate { get; set; }

        // If you want a computed human-friendly status for the UI, expose it as NotMapped
        // so EF Core does not try to map it to the database.
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
    }
}
    