using System.ComponentModel.DataAnnotations;

namespace ClinicalProject_API.Models.ViewModels
{
    public class AppointmentStatusRequest
    {
        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public string Status { get; set; }  

        [MaxLength(500)]
        public string Reason { get; set; }
    }
}