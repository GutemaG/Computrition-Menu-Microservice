using System.Data;
using Computrition.MenuService.API.Data;
using Computrition.MenuService.API.Models;
using Computrition.MenuService.API.MultiTenancy;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Computrition.MenuService.API.Repositories
{
    public class MenuRepository: IMenuRepository
    {
        private readonly AppDbContext _efContext;
        private readonly IDbConnection _dapperConn;
        private readonly ITenantContext _tenant;

        public MenuRepository(AppDbContext efContext, IDbConnection dapperConn, ITenantContext tenant)
        {
            _efContext = efContext;
            _dapperConn = dapperConn;
            _tenant = tenant;
        }

        public async Task AddMenuItemAsync(MenuItem item)
        {
            item.HospitalId = _tenant.HospitalId;
            _efContext.MenuItems.Add(item);
            await _efContext.SaveChangesAsync(); 
        }
     
        public async Task<IEnumerable<MenuItem>> GetFilteredMenuItemsAsync(DietaryRestriction restriction)
        {
            string sql = "SELECT * FROM MenuItems WHERE HospitalId = @HospitalId";
            if (restriction == DietaryRestriction.GF) sql += " AND IsGlutenFree = 1";
            if (restriction == DietaryRestriction.SF) sql += " AND IsSugarFree = 1";
            if (restriction == DietaryRestriction.HH) sql += " AND IsHeartHealthy = 1";

            return await _dapperConn.QueryAsync<MenuItem>(sql, new { HospitalId = _tenant.HospitalId });
        }
        public async Task<MenuItem?> GetMenuItemByIdAsync(int id)
        {
            var menus = await _efContext.MenuItems.Include(x => x.Hospital).AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            return menus;
        }

        public async Task UpdateAsync(MenuItem menu)
        {
            var existing = await GetMenuItemByIdAsync(menu.Id);
             if (existing is null)
                throw new KeyNotFoundException($"MenuItem with Id {menu.Id} not found.");
            _efContext.MenuItems.Update(menu);
            await _efContext.SaveChangesAsync();
        }

        public async Task DeleteMenuItem(int id)
        {
            var menu = await _efContext.MenuItems.FindAsync(id);
            if (menu != null)
            {
                _efContext.MenuItems.Remove(menu);
                await _efContext.SaveChangesAsync();
            }
        }
    }
}