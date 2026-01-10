using Computrition.MenuService.API.Models;

namespace Computrition.MenuService.API.Repositories
{
    public interface IMenuRepository
    {
        Task AddMenuItemAsync(MenuItem item);

        Task<IEnumerable<MenuItem>> GetFilteredMenuItemsAsync(DietaryRestriction restriction);
        Task UpdateAsync(MenuItem menu);

        Task<MenuItem?> GetMenuItemByIdAsync(int id);
        Task DeleteMenuItem(int id);
    }
}