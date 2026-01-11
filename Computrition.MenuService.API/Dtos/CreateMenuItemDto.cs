using System.ComponentModel.DataAnnotations;

namespace Computrition.MenuService.API.Dtos
{
    public class CreateMenuItemDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsGlutenFree {get; set;}
        public bool IsSugarFree {get; set;}
        public bool IsHeartHealthy {get; set;}
    }
}