
using Computrition.MenuMicroService.Models;
using Computrition.MenuMicroService.Services;
using Microsoft.AspNetCore.Mvc;

namespace Computrition.MenuMicroService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuItemController: ControllerBase
    {
        private readonly IMenuService _menuService;
        public MenuItemController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenuItem([FromBody] MenuItem item)
        {
            await _menuService.CreateMenuItemAsync(item);
            return CreatedAtAction(nameof(CreateMenuItem), new { id = item.Id }, item);
        }

    }
}