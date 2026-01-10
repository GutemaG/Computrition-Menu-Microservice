using Computrition.MenuMicroService.Models;

namespace Computrition.MenuMicroService.Repositories
{
    public interface IMenuRepository
    {
        Task AddMenuItemAsync(MenuItem item);
        Task<Patient?> GetPatientByIdAsync(int id);

        Task<IEnumerable<MenuItem>> GetFilteredMenuItemsAsync(DietaryRestriction restriction);
    }
}