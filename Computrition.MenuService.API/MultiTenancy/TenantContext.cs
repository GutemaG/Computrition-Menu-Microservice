
namespace Computrition.MenuService.API.MultiTenancy
{
    public sealed class TenantContext:ITenantContext
    {

        public int HospitalId { get; private set; }
        public string HospitalSlug { get; private set; } = string.Empty;

        public void SetTenant(int hospitalId, string hospitalSlug)
        {
            HospitalId = hospitalId;
            HospitalSlug = hospitalSlug ?? string.Empty;
        }
    }
}