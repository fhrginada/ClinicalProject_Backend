using System;
using System.ComponentModel.DataAnnotations;
using ClinicalProject_API.Data;

namespace ClinicalProject_API.Models.Entities
{ 
public class Appointment
    {
        [Key]
        public int AppointmentId { get; set; }

        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public DateTime AppointmentDate { get; set; }
        public TimeSpan? AppointmentTime { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;

        public string Reason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    
    public enum AppointmentStatus
    {
        Scheduled,
        Confirmed,
        Completed,
        Cancelled,
        NoShow
    }
}
