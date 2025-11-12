using System.ComponentModel.DataAnnotations;

namespace DVTech_ODSS.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required]
        [StringLength(100)]
        public string SupplierName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ContactPerson { get; set; }

        [StringLength(50)]
        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Inactive

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        // Archive fields
        public bool IsArchived { get; set; } = false;
        public DateTime? ArchivedDate { get; set; }

        // Navigation properties
        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
        public ICollection<SupplierItem> SupplierItems { get; set; } = new List<SupplierItem>();
    }
}