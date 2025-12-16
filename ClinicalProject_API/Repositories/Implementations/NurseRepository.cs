using ClinicalProject_API.Data;
using ClinicalProject_API.Models.Entities;
using ClinicalProject_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClinicalProject_API.Repositories.Implementations
{
    public class NurseRepository : INurseRepository
    {
        private readonly ClinicalDbContext _context;

        public NurseRepository(ClinicalDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Nurse>> GetAllAsync()
        {
            return await _context.Nurses
                .Include(n => n.User)
                .Include(n => n.Schedules)
                .ToListAsync();
        }

        public async Task<Nurse> GetByIdAsync(int id)
        {
            return await _context.Nurses
                .Include(n => n.User)
                .Include(n => n.Schedules)
                .FirstOrDefaultAsync(n => n.NurseId == id);
        }

        public async Task AddAsync(Nurse nurse)
        {
            await _context.Nurses.AddAsync(nurse);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Nurse nurse)
        {
            _context.Nurses.Update(nurse);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var nurse = await GetByIdAsync(id);
            if (nurse == null) return false;

            _context.Nurses.Remove(nurse);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
