using Computrition.MenuService.API.Models;

namespace Computrition.MenuService.API.Services
{
    public interface IPatientService
    {
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<IEnumerable<Patient>> GetAllPatientsAsync();
        Task CreatePatientAsync(Patient patient);
        Task UpdatePatientAsync(Patient patient);
        Task DeletePatientAsync(int id);
    }
}