using System.Data;
using Computrition.MenuService.API.Data;
using Computrition.MenuService.API.Models;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Computrition.MenuService.API.Repositories
{
    public class HospitalRepository : IHospitalRepository
    {
        public readonly AppDbContext _efContext;
        public readonly IDbConnection _dapperConn;
        public HospitalRepository(AppDbContext ef, IDbConnection dapper)
        {
            _efContext = ef;
            _dapperConn = dapper;
        }

    
        public async Task CreateHospitalAsync(Hospital hospital)
        {
            _efContext.Hospitals.Add(hospital);
            await _efContext.SaveChangesAsync();
            
        }
        public async Task<IEnumerable<Hospital>> GetAllHospitalAsync()
        {
            var query = "SELECT * FROM Hospitals";
            return await _dapperConn.QueryAsync<Hospital>(query);
        }
        
        public async Task<Hospital?> GetHospitalById(int id)
        {
            return await _efContext.Hospitals.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Hospital?> GetHospitalBySlug(string slug)
        {
            return await _efContext.Hospitals.AsNoTracking().FirstOrDefaultAsync(x => x.Slug == slug);
        }
    }
}