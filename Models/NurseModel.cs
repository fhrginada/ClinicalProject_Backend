namespace CLINICSYSTEM.Models
{
    public class NurseModel
    {
        public int NurseId { get; set; }
        public int UserId { get; set; }
        public int? DoctorId { get; set; } 
        public string? LicenseNumber { get; set; }
        public string? Department { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public UserModel? User { get; set; }
        public DoctorModel? Doctor { get; set; }
        public ICollection<NurseTaskModel>? AssignedTasks { get; set; }
    }
}

