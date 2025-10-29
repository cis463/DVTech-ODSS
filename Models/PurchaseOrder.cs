using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVTech_ODSS.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public Supplier? Supplier { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public DateTime? ExpectedDeliveryDate { get; set; }

        public DateTime? ActualDeliveryDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Ordered, Delivered, Cancelled

        public decimal TotalAmount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string CreatedBy { get; set; } = "Admin";

        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<PurchaseOrderItem> OrderItems { get; set; } = new List<PurchaseOrderItem>();
    }
}