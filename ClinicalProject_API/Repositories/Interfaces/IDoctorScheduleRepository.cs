using ClinicalProject_API.Models.Entities;

namespace ClinicalProject_API.Repositories.Interfaces
{
    public interface IDoctorScheduleRepository
    {
        Task<IEnumerable<DoctorSchedule>> GetByDoctorIdAsync(int doctorId);
        Task<DoctorSchedule?> GetByIdAsync(int id);
        Task AddAsync(DoctorSchedule schedule);
        Task<bool> DeleteAsync(int id);
    }
}

