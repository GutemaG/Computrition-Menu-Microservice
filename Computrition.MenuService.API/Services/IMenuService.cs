using Computrition.MenuService.API.Models;

namespace Computrition.MenuService.API.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuItem>> GetAllowedMenuForPatientAsync(int patientId);
        Task CreateMenuItemAsync(MenuItem item);
    }
}