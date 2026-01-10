using System.Text.RegularExpressions;

namespace Computrition.MenuService.API.Utility
{
    public static class Slug
    {
        public static string FromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;

            var lower = name.Trim().ToLowerInvariant();

            // Replace non-alphanumeric with hyphen
            lower = Regex.Replace(lower, @"[^a-z0-9]+", "-");

            // Collapse multiple hyphens + trim
            lower = Regex.Replace(lower, @"-+", "-").Trim('-');

            return lower;
        }
    }
}