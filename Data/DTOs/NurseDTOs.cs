using System.ComponentModel.DataAnnotations;

namespace CLINICSYSTEM.Data.DTOs
{
    public class NurseDashboardDTO
    {
        public int TotalUpcomingAppointments { get; set; }
        public int TotalPendingTasks { get; set; }
        public int TotalInProgressTasks { get; set; }
        public int TotalDoctorsToday { get; set; }
        public List<UpcomingAppointmentDTO> UpcomingAppointments { get; set; } = new();
        public List<NurseTaskDTO> RecentTasks { get; set; } = new();
    }

    public class UpcomingAppointmentDTO
    {
        public int AppointmentId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? ReasonForVisit { get; set; }
    }

    public class NurseTaskDTO
    {
        public int TaskId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? RelatedAppointmentId { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateNurseTaskRequest
    {
        public int? NurseId { get; set; } 

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime DueDate { get; set; }

        public int? RelatedAppointmentId { get; set; }
    }

    public class UpdateTaskStatusRequest
    {
        [Required]
        public int TaskId { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty; 

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class BookAppointmentForPatientRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Valid patient ID is required")]
        public int PatientId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Valid doctor ID is required")]
        public int DoctorId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Valid time slot ID is required")]
        public int TimeSlotId { get; set; }

        [StringLength(500)]
        public string? ReasonForVisit { get; set; }
    }

    public class DoctorScheduleViewDTO
    {
        public int ScheduleId { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public string Specialization { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; }
    }

    public class NurseProfileDTO
    {
        public int NurseId { get; set; }
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? LicenseNumber { get; set; }
        public string? Department { get; set; }
    }

    public class NurseListDTO
    {
        public int NurseId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Department { get; set; }
    }
}

