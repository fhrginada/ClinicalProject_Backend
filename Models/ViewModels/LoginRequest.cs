using System.ComponentModel.DataAnnotations;

namespace ClinicalProject_API.Models.ViewModels

{
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
