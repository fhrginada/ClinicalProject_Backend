using ClinicalProject_API.Models.Entities;

namespace ClinicalProject_API.Repositories.Interfaces
{
    public interface INurseScheduleRepository
    {
        Task<IEnumerable<NurseSchedule>> GetByNurseIdAsync(int nurseId);
        Task AddAsync(NurseSchedule schedule);
        Task<bool> DeleteAsync(int id);
    }
}
