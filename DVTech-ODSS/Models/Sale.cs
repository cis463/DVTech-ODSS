using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DVTech_ODSS.Models
{
    public class Sale
    {
        [Key]
        public int SaleId { get; set; }

        [Required]
        [StringLength(50)]
        public string SaleNumber { get; set; } = string.Empty;

        [Required]
        public DateTime SaleDate { get; set; } = DateTime.Now;

        [StringLength(100)]
        public string? CustomerName { get; set; }

        [StringLength(50)]
        public string? CustomerContact { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        [Required]
        public decimal TotalCost { get; set; }

        [Required]
        public decimal TotalProfit { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Cash";

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public bool IsActive { get; set; } = true;

        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}