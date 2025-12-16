using ClinicalProject_API.Models.Entities;

namespace ClinicalProject_API.Services.Interfaces
{
    public interface INurseScheduleService
    {
        Task<IEnumerable<NurseSchedule>> GetByNurseIdAsync(int nurseId);
        Task AddAsync(NurseSchedule schedule);
        Task<bool> DeleteAsync(int id);
    }
}
