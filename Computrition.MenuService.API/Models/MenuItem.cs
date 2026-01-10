using System.ComponentModel.DataAnnotations;

namespace Computrition.MenuService.API.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;
        public bool IsGlutenFree {get; set;}
        public bool IsSugarFree {get; set;}
        public bool IsHeartHealthy {get; set;}

        public int HospitalId { get; set; }
        public virtual Hospital Hospital { get; set; } = default!;

    }
}