using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClinicalProject_API.Data;

namespace ClinicalProject_API.Models.Entities
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required, StringLength(150)]
        public required string FullName { get; set; }   

        [Required, MaxLength(100)]
        public required string Email { get; set; }

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }

        public int? UserId { get; set; }
        public User? User { get; set; }

        public int? Age { get; set; }
        public string? Gender { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
