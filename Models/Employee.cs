using System.ComponentModel.DataAnnotations;

namespace DVTech_ODSS.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string EmployeeCode { get; set; } = string.Empty;

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(50)]
        [Phone]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Position { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;

        public DateTime DateHired { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, On Leave, Inactive

        public bool IsActive { get; set; } = true;

        // Navigation property
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}