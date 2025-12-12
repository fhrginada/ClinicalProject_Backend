using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ClinicalProject_API.Data;

namespace ClinicalProject_API.Models.Entities


{
    public class Nurse
    {
        [Key]
        public int NurseId { get; set; }

        [Required, StringLength(150)]
        public string FullName { get; set; }

        [StringLength(100)]
        public string Specialty { get; set; }

        public int? UserId { get; set; } // optional link to auth user
        public User User { get; set; }

        public ICollection<NurseSchedule> Schedules { get; set; } = new List<NurseSchedule>();
    }
}
