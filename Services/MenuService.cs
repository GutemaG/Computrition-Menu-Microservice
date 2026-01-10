using Computrition.MenuMicroService.Models;
using Computrition.MenuMicroService.Repositories;

namespace Computrition.MenuMicroService.Services
{
    public class MenuService:IMenuService
    {
        private readonly IMenuRepository _repo;
        public MenuService(IMenuRepository repo)
        {
            _repo = repo;
        }
        public async Task<IEnumerable<MenuItem>> GetAllowedMenuForPatientAsync(int patientId)
        {
            var patient = await _repo.GetPatientByIdAsync(patientId); 
            if(patient == null)
            {
                return Enumerable.Empty<MenuItem>();
            }
            return await _repo.GetFilteredMenuItemsAsync(patient.DietaryRestrictionCode);
        }
    }
}