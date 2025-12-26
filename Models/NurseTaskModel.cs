namespace CLINICSYSTEM.Models
{
    public class NurseTaskModel
    {
        public int TaskId { get; set; }
        public int NurseId { get; set; }
        public int DoctorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending"; 
        public int? RelatedAppointmentId { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? Notes { get; set; }

        public NurseModel? Nurse { get; set; }
        public DoctorModel? Doctor { get; set; }
        public AppointmentModel? RelatedAppointment { get; set; }
    }
}

