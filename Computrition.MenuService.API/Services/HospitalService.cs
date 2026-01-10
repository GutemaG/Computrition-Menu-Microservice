using Computrition.MenuService.API.Models;
using Computrition.MenuService.API.Repositories;
using Computrition.MenuService.API.Utility;

namespace Computrition.MenuService.API.Services
{
    public class HospitalService : IHospitalService
    {
        public readonly IHospitalRepository _hospitalRepo;
        public HospitalService(IHospitalRepository repo)
        {
            _hospitalRepo = repo;
        }

        public async Task CreateHospitalAsync(Hospital hospital)
        {
            if (string.IsNullOrWhiteSpace(hospital.Name))
            {
                throw new ArgumentException("Name is required.");
            }
            // check existence by slug
            var existing = await _hospitalRepo.GetHospitalBySlug(Slug.FromName(hospital.Name));
            if(existing != null)
            {
                throw new Exception($"Hospital with similar Name(slug) already existed");
            }
            await _hospitalRepo.CreateHospitalAsync(hospital);

        }
        public async Task<IEnumerable<Hospital>> GetAllHospitalAsync()
        {
            return await _hospitalRepo.GetAllHospitalAsync();
        }
        public async Task<Hospital?> GetHospitalById(int id)
        {
            var hospital = await _hospitalRepo.GetHospitalById(id);
            return hospital;
        }
        public async Task<Hospital?> GetHospitalBySlug(string slug)
        {
            var hospital = await _hospitalRepo.GetHospitalBySlug(slug);
            return hospital;

        }
    }
}