using ClinicalProject_API.Models.Entities;
using ClinicalProject_API.Repositories.Interfaces;
using ClinicalProject_API.Services.Interfaces;

namespace ClinicalProject_API.Services.Implementations
{
    public class NurseScheduleService : INurseScheduleService
    {
        private readonly INurseScheduleRepository _repo;

        public NurseScheduleService(INurseScheduleRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<NurseSchedule>> GetByNurseIdAsync(int nurseId)
        {
            return await _repo.GetByNurseIdAsync(nurseId);
        }

        public async Task AddAsync(NurseSchedule schedule)
        {
            await _repo.AddAsync(schedule);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }
    }
}
