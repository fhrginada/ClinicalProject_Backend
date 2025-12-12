using ClinicalProject_API.Models.Entities;
using ClinicalProject_API.Models.ViewModels.NurseVM;
using ClinicalProject_API.Repositories.Interfaces;
using ClinicalProject_API.Services.Interfaces;

namespace ClinicalProject_API.Services.Implementations
{
    public class NurseService : INurseService
    {
        private readonly INurseRepository _repository;

        public NurseService(INurseRepository repository)
        {
            _repository = repository;
        }

        public async Task<NurseResponse> CreateAsync(CreateNurseRequest request)
        {
            var nurse = new Nurse
            {
                FullName = request.FullName,
                Specialty = request.Specialty,
                UserId = request.UserId
            };

            await _repository.AddAsync(nurse);

            return new NurseResponse
            {
                NurseId = nurse.NurseId,
                FullName = nurse.FullName,
                Specialty = nurse.Specialty,
                UserId = nurse.UserId
            };
        }

        public async Task<IEnumerable<NurseResponse>> GetAllAsync()
        {
            var nurses = await _repository.GetAllAsync();

            return nurses.Select(n => new NurseResponse
            {
                NurseId = n.NurseId,
                FullName = n.FullName,
                Specialty = n.Specialty,
                UserId = n.UserId
            });
        }

        public async Task<NurseResponse?> GetByIdAsync(int id)
        {
            var nurse = await _repository.GetByIdAsync(id);
            if (nurse == null) return null;

            return new NurseResponse
            {
                NurseId = nurse.NurseId,
                FullName = nurse.FullName,
                Specialty = nurse.Specialty,
                UserId = nurse.UserId
            };
        }

        public async Task<bool> UpdateAsync(int id, UpdateNurseRequest request)
        {
            var nurse = await _repository.GetByIdAsync(id);
            if (nurse == null) return false;

            nurse.FullName = request.FullName;
            nurse.Specialty = request.Specialty;
            

            return await _repository.UpdateAsync(nurse);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
