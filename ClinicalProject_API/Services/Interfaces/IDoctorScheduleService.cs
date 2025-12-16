using ClinicalProject_API.Models.Entities;

namespace ClinicalProject_API.Services.Interfaces
{
    public interface IDoctorScheduleService
    {
        Task<IEnumerable<DoctorSchedule>> GetByDoctorIdAsync(int doctorId);
        Task AddAsync(DoctorSchedule schedule);
        Task<bool> DeleteAsync(int id);
    }
}
