using Computrition.MenuService.API.Models;
using Computrition.MenuService.API.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Computrition.MenuService.API.Services
{
    public class MenuService:IMenuService
    {
        private readonly IMenuRepository _menuRepo;
        private readonly IPatientRepository _patientRepo;
        public MenuService(IMenuRepository repo, IPatientRepository patientRepository)
        {
            _menuRepo = repo;
            _patientRepo = patientRepository;
        }
        public async Task<IEnumerable<MenuItem>> GetAllowedMenuForPatientAsync(int patientId)
        {
            var patient = await _patientRepo.GetPatientByIdAsync(patientId);
            if (patient == null)
            {
                return Enumerable.Empty<MenuItem>();
            }
            return await _menuRepo.GetFilteredMenuItemsAsync(patient.DietaryRestrictionCode);
        }
        public async Task CreateMenuItemAsync(MenuItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
            {
                throw new ArgumentException("Name is required.");
            }

            item.Name = char.ToUpper(item.Name[0]) + item.Name.Substring(1);
            await _menuRepo.AddMenuItemAsync(item);
        }
        public async Task<MenuItem?> GetMenuItemByIdAsync(int id)
        {
            return await _menuRepo.GetMenuItemByIdAsync(id);
        }
        public async Task DeleteMenuItem(int id)
        {
            await _menuRepo.DeleteMenuItem(id);
        }
        public async Task<IEnumerable<MenuItem>> GetMenuItemByDietaryCode(DietaryRestriction dietaryRestriction)
        {
            return await _menuRepo.GetFilteredMenuItemsAsync(dietaryRestriction);
        }
        public async Task UpdateMenuAsync(MenuItem menuItem)
        {
            await _menuRepo.UpdateAsync(menuItem);
        }
    }
}