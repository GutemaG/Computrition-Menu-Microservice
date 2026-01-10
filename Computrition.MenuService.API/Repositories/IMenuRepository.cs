using Computrition.MenuService.API.Models;

namespace Computrition.MenuService.API.Repositories
{
    public interface IMenuRepository
    {
        Task AddMenuItemAsync(MenuItem item);
        Task<Patient?> GetPatientByIdAsync(int id);

        Task<IEnumerable<MenuItem>> GetFilteredMenuItemsAsync(DietaryRestriction restriction);
    }
}