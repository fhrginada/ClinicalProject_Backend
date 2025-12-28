using System;

namespace ClinicalProject_API.Models.Entities
{
    public class DigitalSignatureToken
    {
        public int DigitalSignatureTokenId { get; set; }
        public int PrescriptionId { get; set; }
        public Prescription Prescription { get; set; }

        public string Token { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
