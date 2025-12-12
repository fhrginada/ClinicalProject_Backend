using System.ComponentModel.DataAnnotations;
using ClinicalProject_API.Data;

namespace ClinicalProject_API.Models.Entities


{
    public class Medication
    {
        [Key]
        public int MedicationId { get; set; }

        [Required, StringLength(150)]
        public string DrugName { get; set; }

        [StringLength(100)]
        public string Category { get; set; }

        [StringLength(50)]
        public string CommonDosage { get; set; }
    }
}
