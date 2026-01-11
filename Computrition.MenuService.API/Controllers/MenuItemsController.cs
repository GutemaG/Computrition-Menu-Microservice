
using Computrition.MenuService.API.Dtos;
using Computrition.MenuService.API.Models;
using Computrition.MenuService.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Computrition.MenuService.API.Controllers
{
    [ApiController]
    [Route("api/menu-items/")]
    public class MenuItemController: ControllerBase
    {
        private readonly IMenuService _menuService;
        public MenuItemController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpPost("/api/menu-item/")]
        public async Task<IActionResult> CreateMenuItem([FromBody] CreateMenuItemDto item)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var newItem = new MenuItem
            {
                Name = item.Name,
                Category = item.Category,
                IsGlutenFree = item.IsGlutenFree,
                IsHeartHealthy = item.IsHeartHealthy,
                IsSugarFree = item.IsSugarFree
            };
            await _menuService.CreateMenuItemAsync(newItem);
            return CreatedAtAction(nameof(CreateMenuItem), new { id = newItem.Id }, item);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMenuItemById(int id)
        {
            var menu = await _menuService.GetMenuItemByIdAsync(id);
            if (menu == null) return NotFound("Patient with the Id not found");
            return Ok(menu);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MenuItem menuItem)
        {
            if (id != menuItem.Id) return BadRequest("Id doesn't match");
            var existingMenu = await _menuService.GetMenuItemByIdAsync(id);
            if (existingMenu == null) return NotFound("Menu Item with the Id not found");
            await _menuService.UpdateMenuAsync(menuItem);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            await _menuService.DeleteMenuItem(id);
            return NoContent();
        }
        [HttpGet("restriction/{restriction}")]
        public async Task<IActionResult> GetMenuItemByRestriction(DietaryRestriction restriction)
        {
            var menu = await _menuService.GetMenuItemByDietaryCode(restriction);
            return Ok(menu);
        }

    }
}