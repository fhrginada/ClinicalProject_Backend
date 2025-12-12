using ClinicalProject_API.Models.Entities;

namespace ClinicalProject_API.Repositories.Interfaces
{
    public interface INurseRepository
    {
        Task<IEnumerable<Nurse>> GetAllAsync();
        Task<Nurse> GetByIdAsync(int id);
        Task AddAsync(Nurse nurse);
        Task<bool> UpdateAsync(Nurse nurse);
        Task<bool> DeleteAsync(int id);
    }
}
