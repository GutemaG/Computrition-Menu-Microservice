namespace Computrition.MenuService.API.Models
{
    public class Patient
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public DietaryRestriction DietaryRestrictionCode { get; set; } = DietaryRestriction.None;

        public int HospitalId { get; set; }
        public Hospital Hospital { get; set; } = default!;
    }
}
