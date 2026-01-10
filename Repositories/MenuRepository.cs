using System.Data;
using Computrition.MenuMicroService.Data;
using Computrition.MenuMicroService.Models;
using Dapper;

namespace Computrition.MenuMicroService.Repositories
{
    public class MenuRepository: IMenuRepository
    {
        private readonly AppDbContext _efContext;
        private readonly IDbConnection _dapperConn;

        public MenuRepository(AppDbContext efContext, IDbConnection dapperConn)
        {
            _efContext = efContext;
            _dapperConn = dapperConn;
        }

        public async Task AddMenuItemAsync(MenuItem item)
        {
            _efContext.MenuItems.Add(item);
            await _efContext.SaveChangesAsync(); 
        }
        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _dapperConn.QueryFirstOrDefaultAsync<Patient>("SELECT * FROM Patients WHERE Id = @Id", new { Id = id });
        }
        public async Task<IEnumerable<MenuItem>> GetFilteredMenuItemsAsync(DietaryRestriction restriction)
        {
            string sql = "SELECT * FROM MenuItems WHERE 1=1";
           if (restriction == DietaryRestriction.GF) sql += " AND IsGlutenFree = 1"; 
           if (restriction == DietaryRestriction.SF) sql += " AND IsSugarFree = 1"; 
           if (restriction == DietaryRestriction.HH) sql += " AND IsHeartHealthy = 1";
            return await _dapperConn.QueryAsync<MenuItem>(sql);
        }
    }
}