using System;
using System.ComponentModel.DataAnnotations;
using ClinicalProject_API.Data;

namespace ClinicalProject_API.Models.Entities


{
    public class DoctorSchedule
    {
        [Key]
        public int DoctorScheduleId { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        // slot length minutes
        public int SlotDuration { get; set; } = 30;
    }
}
