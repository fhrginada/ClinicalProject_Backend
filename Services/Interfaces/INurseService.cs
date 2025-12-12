using ClinicalProject_API.Models.ViewModels.NurseVM;

namespace ClinicalProject_API.Services.Interfaces
{
    public interface INurseService
    {
        Task<NurseResponse> CreateAsync(CreateNurseRequest request);
        Task<IEnumerable<NurseResponse>> GetAllAsync();
        Task<NurseResponse?> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateNurseRequest request);
        Task<bool> DeleteAsync(int id);
    }
}
