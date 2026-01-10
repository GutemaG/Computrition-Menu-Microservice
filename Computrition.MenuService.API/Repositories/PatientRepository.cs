using System.Data;
using Computrition.MenuService.API.Data;
using Computrition.MenuService.API.Models;
using Computrition.MenuService.API.MultiTenancy;
using Dapper;

namespace Computrition.MenuService.API.Repositories
{
    class PatientRepository: IPatientRepository
    {
        private readonly AppDbContext _efContext;
        private readonly IDbConnection _dapperConn;
        private readonly ITenantContext _tenant;
        
        public PatientRepository(AppDbContext ef, IDbConnection dapper, ITenantContext tenant)
        {
            _efContext = ef;
            _dapperConn = dapper;
            _tenant = tenant;
        }
        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _dapperConn.QueryFirstOrDefaultAsync<Patient>(
                "SELECT * FROM Patients WHERE Id = @Id AND HospitalId = @HospitalId",
                new { Id = id, HospitalId = _tenant.HospitalId });
        }
        public async Task CreateAsync(Patient patient)
        {
            _efContext.Patients.Add(patient);
            await _efContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            const string sql = "SELECT * FROM Patients WHERE HospitalId = @HospitalId";
            return await _dapperConn.QueryAsync<Patient>(sql, new { HospitalId = _tenant.HospitalId });
        }
        public async Task UpdateAsync(Patient patient)
        {
            _efContext.Patients.Update(patient);
            await _efContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var patient = await _efContext.Patients.FindAsync(id);
            if (patient != null)
            {
                _efContext.Patients.Remove(patient);
                await _efContext.SaveChangesAsync();
            }
        }

    }
}