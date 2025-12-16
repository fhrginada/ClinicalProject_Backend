using System.ComponentModel.DataAnnotations;

namespace Clinical_project.API.Models.ViewModels

{
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
