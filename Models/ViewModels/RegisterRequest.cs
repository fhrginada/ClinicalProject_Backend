using System.ComponentModel.DataAnnotations;
using Clinical_project.API.Models;

namespace Clinical_project.API.Models.ViewModels

{
    public class RegisterRequest
    {
        [Required, StringLength(100)]
        public string UserName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public UserRole Role { get; set; } = UserRole.Patient;
    }

    public enum UserRole
    {
        Admin,
        Doctor,
        Nurse,
        Patient
    }
}
