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
        public Patient Patient { get; set; }

        public MedicalRecordType RecordType { get; set; }

        public DateTime DateRecorded { get; set; } = DateTime.UtcNow;

        public string Description { get; set; }

        [StringLength(1024)]
        public string AttachmentUrl { get; set; }
    }

    // ⭐ Enum inside same file
    public enum MedicalRecordType
    {
        Diagnosis,
        Prescription,
        LabResults,
        Imaging
    }
}
