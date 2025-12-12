using ClinicalProject_API.Data;

namespace ClinicalProject_API.Models.Entities
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public int? ConsultationID { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public Patient Patient { get; set; }
        public int PatientId { get; set; }
        public DateTime DateIssued { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active";
        public string? SignaturePath { get; set; }
        public string? Notes { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<PrescriptionDetail> Details { get; set; } = new List<PrescriptionDetail>();
    }
}
