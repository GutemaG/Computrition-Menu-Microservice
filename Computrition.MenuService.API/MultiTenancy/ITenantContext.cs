namespace Computrition.MenuService.API.MultiTenancy
{
    public interface ITenantContext
    {
        int HospitalId { get; }
        string HospitalSlug { get; }

        void SetTenant(int hospitalId, string hospitalSlug);
    }
}