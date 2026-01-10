using Computrition.MenuMicroService.Models;

namespace Computrition.MenuMicroService.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuItem>> GetAllowedMenuForPatientAsync(int patientId);
    }
}