using System.Data;
using Computrition.MenuService.API.Data;
using Computrition.MenuService.API.Models;
using Dapper;

namespace Computrition.MenuService.API.Repositories
{
    class PatientRepository: IPatientRepository
    {
        private readonly AppDbContext _efContext;
        private readonly IDbConnection _dapperConn;
        
        public PatientRepository(AppDbContext ef, IDbConnection dapper)
        {
            _efContext = ef;
            _dapperConn = dapper;
        }
        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _dapperConn.QueryFirstOrDefaultAsync<Patient>("SELECT * FROM Patients WHERE Id = @Id", new { Id = id });
        }
        public async Task CreateAsync(Patient patient)
        {
            _efContext.Patients.Add(patient);
            await _efContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Patient>> GetAllAsync()
        {
            string sql = "SELECT * FROM Patients";
            return await _dapperConn.QueryAsync<Patient>(sql);
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