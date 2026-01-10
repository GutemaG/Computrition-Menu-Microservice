using Computrition.MenuService.API.Models;
using Computrition.MenuService.API.Repositories;
using MenuServiceClass = Computrition.MenuService.API.Services.MenuService;
using Moq;

namespace Computrition.MenuService.Tests;

public class MenuServiceTests
{
    private readonly Mock<IMenuRepository> _mockMenuRepo;
    private readonly Mock<IPatientRepository> _mockPatientRepo;
    private readonly MenuServiceClass _service;

    public MenuServiceTests()
    {
        _mockMenuRepo = new Mock<IMenuRepository>();
        _mockPatientRepo = new Mock<IPatientRepository>();
        _service = new MenuServiceClass(_mockMenuRepo.Object, _mockPatientRepo.Object);
    }

    [Fact]
    public async Task GetAllowedMenuForPatientAsync_WhenPatientIsGF_ReturnsOnlyGlutenFreeItems()
    {
        int patientId = 1;
        var patient = new Patient
        {
            Id = patientId,
            Name = "John Doe",
            DietaryRestrictionCode = DietaryRestriction.GF
        };

        var mockMenu = new List<MenuItem>
        {
            new MenuItem { Id = 101, Name = "GF Bread", IsGlutenFree = true, IsSugarFree = false },
            new MenuItem { Id = 102, Name = "Regular Pasta", IsGlutenFree = false, IsSugarFree = false }
        };
        _mockPatientRepo.Setup(r => r.GetPatientByIdAsync(patientId)).ReturnsAsync(patient);
        _mockMenuRepo.Setup(r => r.GetFilteredMenuItemsAsync(DietaryRestriction.GF))
             .ReturnsAsync(mockMenu.Where(m => m.IsGlutenFree));
        var result = await _service.GetAllowedMenuForPatientAsync(patientId);
        var resultList = result.ToList();
        Assert.NotNull(resultList);
        Assert.Single(resultList);
        Assert.Equal("GF Bread", resultList[0].Name);
        Assert.True(resultList[0].IsGlutenFree);
        _mockPatientRepo.Verify(r => r.GetPatientByIdAsync(patientId), Times.Once);
        _mockMenuRepo.Verify(r => r.GetFilteredMenuItemsAsync(DietaryRestriction.GF), Times.Once);
    }

    [Fact]
    public async Task CreateMenuItem_WhenValid_CallsRepoOnce()
    {
        // Arrange
        var item = new MenuItem { Name = "Fresh Salad" };

        // Act
        await _service.CreateMenuItemAsync(item);

        // Assert
        _mockMenuRepo.Verify(r => r.AddMenuItemAsync(It.IsAny<MenuItem>()), Times.Once);
    }
    [Fact]
    public async Task CreateMenuItem_WhenNameIsEmpty_ThrowsArgumentException()
    {
        // Arrange
        var item = new MenuItem { Name = "" };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateMenuItemAsync(item));
    }

    [Fact]
    public async Task CreateMenuItem_Always_CapitalizesFirstLetter()
    {
        // Arrange
        var item = new MenuItem { Name = "low sodium soup" };

        // Act
        await _service.CreateMenuItemAsync(item);

        // Assert
        Assert.Equal("Low sodium soup", item.Name);
    }

}