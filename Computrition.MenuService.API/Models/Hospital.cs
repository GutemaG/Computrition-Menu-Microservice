using System.ComponentModel.DataAnnotations;

namespace Computrition.MenuService.API.Models
{
    public class Hospital
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = default!;

        public string Slug { get; set; } = string.Empty; // it will be used for X-tenant-id in header
    }
}