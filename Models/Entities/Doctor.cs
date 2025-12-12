using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClinicalProject_API.Data;

namespace ClinicalProject_API.Models.Entities

{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; }
        public string Specialty { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<DoctorSchedule> Schedules { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<Prescription> PrescriptionsIssued { get; set; } // FIX: Change type from object to ICollection<Prescription>
    }
}
