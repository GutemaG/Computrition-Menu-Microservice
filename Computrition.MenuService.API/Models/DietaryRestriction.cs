using System.Text.Json.Serialization;

namespace Computrition.MenuService.API.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DietaryRestriction
    {
        None,
        GF, // Gluten Free
        SF, // Sugar Free
        HH // Heart Healthy
    }
}