using Computrition.MenuService.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Computrition.MenuService.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientsController: ControllerBase
    {
        private readonly IMenuService _menuService;
        public PatientsController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet("{patientId}/allowed-menu")]
        public async Task<IActionResult> GetAllowedMenu(int patientId)
        {
            var menuItems = await _menuService.GetAllowedMenuForPatientAsync(patientId);
            if(menuItems == null || !menuItems.Any())
            {
                return NotFound($"No allowed menu items found for Patient ID {patientId}.");
            }
            return Ok(menuItems);
        }


    }
}