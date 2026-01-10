namespace Computrition.MenuService.API.MultiTenancy
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class SkipTenantHeaderAttribute : Attribute
    {
    }
}