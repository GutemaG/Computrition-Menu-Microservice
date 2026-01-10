using Computrition.MenuService.API.Models;
using Computrition.MenuService.API.Repositories;

namespace Computrition.MenuService.API.Services
{
    public class PatientService:IPatientService
    {
        private readonly IPatientRepository _patientRepo;

        public PatientService(IPatientRepository patientRepo)
        {
            _patientRepo = patientRepo;
        }

        public async Task<Patient?> GetPatientByIdAsync(int id)
        {
            return await _patientRepo.GetPatientByIdAsync(id);
        }
        public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
        {
            return await _patientRepo.GetAllAsync();
        }
        public async Task CreatePatientAsync(Patient patient)
        {
            await _patientRepo.CreateAsync(patient);
        }
        public async Task UpdatePatientAsync(Patient patient)
        {
            await _patientRepo.UpdateAsync(patient);
        }
        public async Task DeletePatientAsync(int id)
        {
            await _patientRepo.DeleteAsync(id);
        }
    }
}