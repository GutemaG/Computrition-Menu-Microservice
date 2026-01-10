using System.Net;
using System.Text.Json;
using Computrition.MenuService.API.MultiTenancy;
using Computrition.MenuService.Tests.TestInfrastructure;

namespace Computrition.MenuService.Tests;

public class MultiTenancyControllerTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MultiTenancyControllerTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PatientsEndpoint_WhenMissingTenantHeader_Returns400()
    {
        var response = await _client.GetAsync("/api/patients");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains(TenantHeaderMiddleware.HeaderName, body);
    }

    [Fact]
    public async Task PatientsEndpoint_WhenTenantHeaderUnknown_Returns401()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/patients");
        request.Headers.Add(TenantHeaderMiddleware.HeaderName, "unknown-hospital");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PatientsEndpoint_WhenTenantIsHospitalA_ReturnsOnlyHospitalAPatients()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/patients");
        request.Headers.Add(TenantHeaderMiddleware.HeaderName, "hospital-a");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        var hospitalIds = ExtractHospitalIds(json);

        Assert.NotEmpty(hospitalIds);
        Assert.All(hospitalIds, id => Assert.Equal(1, id));
    }

    [Fact]
    public async Task PatientsEndpoint_WhenTenantIsHospitalB_ReturnsOnlyHospitalBPatients()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/patients");
        request.Headers.Add(TenantHeaderMiddleware.HeaderName, "hospital-b");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        var hospitalIds = ExtractHospitalIds(json);

        Assert.NotEmpty(hospitalIds);
        Assert.All(hospitalIds, id => Assert.Equal(2, id));
    }

    [Fact]
    public async Task GetPatientById_WhenTenantARequestsTenantBPatient_Returns404()
    {
        // Patient Id=2 is seeded with HospitalId=2 in AppDbContext.
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/patients/2");
        request.Headers.Add(TenantHeaderMiddleware.HeaderName, "hospital-a");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task HospitalEndpoints_AreAccessibleWithoutTenantHeader()
    {
        var response = await _client.GetAsync("/api/hospital");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task MenuItemsByRestriction_WhenTenantIsHospitalA_ReturnsOnlyHospitalAMenuItems()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/menu-items/restriction/GF");
        request.Headers.Add(TenantHeaderMiddleware.HeaderName, "hospital-a");

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        var hospitalIds = ExtractHospitalIds(json);

        Assert.NotEmpty(hospitalIds);
        Assert.All(hospitalIds, id => Assert.Equal(1, id));
    }

    private static List<int> ExtractHospitalIds(string json)
    {
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.ValueKind != JsonValueKind.Array)
            return new List<int>();

        var results = new List<int>();

        foreach (var element in doc.RootElement.EnumerateArray())
        {
            if (element.TryGetProperty("hospitalId", out var hospitalIdProp) &&
                hospitalIdProp.ValueKind == JsonValueKind.Number &&
                hospitalIdProp.TryGetInt32(out var id))
            {
                results.Add(id);
            }
        }

        return results;
    }
}
