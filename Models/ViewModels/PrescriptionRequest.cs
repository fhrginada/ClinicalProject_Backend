using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Clinical_project.API.Models.ViewModels

{
    public class PrescriptionRequest
    {
        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int PatientId { get; set; }

        public List<PrescriptionLine> Lines { get; set; } = new List<PrescriptionLine>();

        public class PrescriptionLine
        {
            [Required]
            public int MedicationId { get; set; }

            public string Dosage { get; set; }
            public string Frequency { get; set; }
            public string Instructions { get; set; }
        }
    }
}
