using System.ComponentModel.DataAnnotations;
using Clinical_project.API.Models;

namespace Clinical_project.API.Models.ViewModels

{
    public class RegisterRequest
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

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
