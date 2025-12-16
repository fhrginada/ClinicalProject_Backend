using System.ComponentModel.DataAnnotations;
using ClinicalProject_API.Data;

namespace ClinicalProject_API.Models.Entities
{
    public class PrescriptionDetail
    {
        [Key]
        public int DetailID { get; set; }
        public int PrescriptionId { get; set; }
        public int? MedicationId { get; set; }
        public string DrugName { get; set; } = null!;
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public string? Duration { get; set; }
        public string? Instructions { get; set; }

        // Navigation properties
        public Prescription? Prescription { get; set; }
        public Medication? Medication { get; set; }
    }
}
