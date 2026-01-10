namespace Computrition.MenuService.API.Models
{
    public class Patient
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public DietaryRestriction DietaryRestrictionCode { get; set;} = DietaryRestriction.None;
    }
}
