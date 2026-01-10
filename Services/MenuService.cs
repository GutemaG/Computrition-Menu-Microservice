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
            if (patient == null)
            {
                return Enumerable.Empty<MenuItem>();
            }
            return await _repo.GetFilteredMenuItemsAsync(patient.DietaryRestrictionCode);
        }
        public async Task CreateMenuItemAsync(MenuItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                throw new ArgumentException("Name is required.");
            }

            item.Name = char.ToUpper(item.Name[0]) + item.Name.Substring(1);
            await _repo.AddMenuItemAsync(item);
        }
    }
}