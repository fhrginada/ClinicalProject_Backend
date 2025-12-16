using System;
using System.ComponentModel.DataAnnotations;
using ClinicalProject_API.Data;

namespace ClinicalProject_API.Models.Entities
{
    public class MedicalRecord
    {
        [Key]
        public int MedicalRecordId { get; set; }

        public int PatientId { get; set; }

        public required Patient Patient { get; set; }  // Mark as required

        public MedicalRecordType RecordType { get; set; }

        public DateTime DateRecorded { get; set; } = DateTime.UtcNow;

        public required string Description { get; set; }  // Mark as required

        [StringLength(1024)]
        public string? AttachmentUrl { get; set; }  
    }

    
    public enum MedicalRecordType
    {
        Diagnosis,
        Prescription,
        LabResults,
        Imaging
    }
}

