using System;
using System.ComponentModel.DataAnnotations;

namespace Clinical_project.API.Models.ViewModels

{
    public class AppointmentRequest
    {
        [Required]
        public int PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        public TimeSpan? AppointmentTime { get; set; }
        public string Reason { get; set; }
    }
}
