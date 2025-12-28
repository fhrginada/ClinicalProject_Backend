using System;
using System.Collections.Generic;

namespace ClinicalProject_API.Models.Entities
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public int? ConsultationID { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; } // ممكن تعمل null! لو تحبي
        public int PatientId { get; set; }
        public Patient Patient { get; set; } // ممكن تعمل null!
        public DateTime DateIssued { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Active";
        public string? Notes { get; set; }
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<PrescriptionDetail> Details { get; set; } = new List<PrescriptionDetail>();
        public ICollection<DigitalSignatureToken> Tokens { get; set; } = new List<DigitalSignatureToken>();
    }
}
