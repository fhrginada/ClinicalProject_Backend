using System.ComponentModel.DataAnnotations;

namespace ClinicalProject_API.Models.ViewModels
{
    public class PatientRequest
    {
        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        // Optional if patient is linked to a user
        public int? UserId { get; set; }
    }
}
