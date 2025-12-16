using ClinicalProject_API.Data;
using ClinicalProject_API.Models.Entities;
using ClinicalProject_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClinicalProject_API.Repositories.Implementations
{
    public class NurseScheduleRepository : INurseScheduleRepository
    {
        private readonly ClinicalDbContext _context;

        public NurseScheduleRepository(ClinicalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NurseSchedule>> GetByNurseIdAsync(int nurseId)
        {
            return await _context.NurseSchedules
                .Where(s => s.NurseId == nurseId)
                .ToListAsync();
        }

        public async Task AddAsync(NurseSchedule schedule)
        {
            await _context.NurseSchedules.AddAsync(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var schedule = await _context.NurseSchedules.FindAsync(id);
            if (schedule == null) return false;

            _context.NurseSchedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
