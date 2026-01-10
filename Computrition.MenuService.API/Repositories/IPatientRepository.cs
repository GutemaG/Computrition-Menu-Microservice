using Computrition.MenuService.API.Models;

namespace Computrition.MenuService.API.Repositories
{
    public interface IPatientRepository
    {

        Task CreateAsync(Patient patient);
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<IEnumerable<Patient>> GetAllAsync();
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(int id);

    }
}