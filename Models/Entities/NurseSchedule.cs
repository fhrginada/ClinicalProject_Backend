using System;
using System.ComponentModel.DataAnnotations;
using ClinicalProject_API.Data;

namespace ClinicalProject_API.Models.Entities


{
    public class NurseSchedule
    {
        [Key]
        public int NurseScheduleId { get; set; }

        public int NurseId { get; set; }
        public Nurse Nurse { get; set; }

        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
