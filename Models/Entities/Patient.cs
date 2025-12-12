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
        public string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [StringLength(30)]
        public string Phone { get; set; }

        public int? UserId { get; set; } // optional link to auth user
        public User User { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}
