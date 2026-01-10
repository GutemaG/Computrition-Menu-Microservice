using Computrition.MenuService.API.Models;

namespace Computrition.MenuService.API.Services
{
    public interface IHospitalService
    {

        Task CreateHospitalAsync(Hospital hospital);
        Task<IEnumerable<Hospital>> GetAllHospitalAsync();
        Task<Hospital?> GetHospitalById(int id);
        Task<Hospital?> GetHospitalBySlug(string slug);
    }
}