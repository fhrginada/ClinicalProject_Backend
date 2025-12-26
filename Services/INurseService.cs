using CLINICSYSTEM.Data.DTOs;

namespace CLINICSYSTEM.Services
{
    public interface INurseService
    {
        Task<int?> GetNurseIdByUserIdAsync(int userId);
        Task<NurseDashboardDTO?> GetDashboardAsync(int userId);
        Task<NurseProfileDTO?> GetProfileAsync(int userId);
        Task<List<DoctorScheduleViewDTO>> GetAllDoctorSchedulesAsync();
        Task<List<DoctorScheduleViewDTO>> GetMyDoctorSchedulesAsync(int userId);
        Task<List<DoctorScheduleViewDTO>> GetDoctorScheduleAsync(int doctorId);
        Task<AppointmentDTO?> BookAppointmentForPatientAsync(int nurseId, BookAppointmentForPatientRequest request);
        Task<List<NurseTaskDTO>> GetMyTasksAsync(int userId);
        Task<NurseTaskDTO?> GetTaskDetailsAsync(int taskId, int userId);
        Task<bool> UpdateTaskStatusAsync(int taskId, int userId, UpdateTaskStatusRequest request);
        Task<List<UpcomingAppointmentDTO>> GetUpcomingAppointmentsAsync(int days = 7);
        Task<List<UpcomingAppointmentDTO>> GetUpcomingAppointmentsForNurseAsync(int userId, int days = 7);
    }
}

