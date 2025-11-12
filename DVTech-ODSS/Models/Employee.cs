using System.ComponentModel.DataAnnotations;

namespace DVTech_ODSS.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        // NEW: Separate name fields
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [StringLength(20)]
        public string? Suffix { get; set; }

        // Computed full name (for backward compatibility)
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

        // NEW: Photo path for employee pictures
        [StringLength(500)]
        public string? PhotoPath { get; set; }

        // Navigation property
        public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();

        // Helper method to update full name
        public void UpdateFullName()
        {
            var nameParts = new List<string>();

            // Add first name
            if (!string.IsNullOrWhiteSpace(FirstName))
                nameParts.Add(FirstName);

            // Add middle name
            if (!string.IsNullOrWhiteSpace(MiddleName))
                nameParts.Add(MiddleName);

            // Add last name
            if (!string.IsNullOrWhiteSpace(LastName))
                nameParts.Add(LastName);

            // Combine name parts
            FullName = string.Join(" ", nameParts);

            // Add suffix if present
            if (!string.IsNullOrWhiteSpace(Suffix))
                FullName += $", {Suffix}";
        }

        // Helper method to get formatted name
        public string GetFormattedName(bool lastNameFirst = false)
        {
            if (lastNameFirst)
            {
                var formatted = string.IsNullOrWhiteSpace(MiddleName)
                    ? $"{LastName}, {FirstName}"
                    : $"{LastName}, {FirstName} {MiddleName[0]}.";

                if (!string.IsNullOrWhiteSpace(Suffix))
                    formatted += $", {Suffix}";

                return formatted;
            }
            return FullName;
        }
    }
}