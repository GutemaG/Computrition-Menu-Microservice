using System.ComponentModel.DataAnnotations;
using Computrition.MenuService.API.Models;

namespace Computrition.MenuService.API.Dtos
{
    public class CreatePatientDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public DietaryRestriction DietaryRestrictionCode { get; set;} = DietaryRestriction.None;

    }
}