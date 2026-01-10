using Computrition.MenuService.API.Models;

namespace Computrition.MenuService.API.Services
{
    public interface IMenuService
    {
        Task<IEnumerable<MenuItem>> GetAllowedMenuForPatientAsync(int patientId);
        Task CreateMenuItemAsync(MenuItem item);
        Task<MenuItem?> GetMenuItemByIdAsync(int id);
        Task UpdateMenuAsync(MenuItem menuItem);
        Task<IEnumerable<MenuItem>> GetMenuItemByDietaryCode(DietaryRestriction dietaryRestriction);
        Task DeleteMenuItem(int id);
    }
}