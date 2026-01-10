using Computrition.MenuService.API.Models;
using Computrition.MenuService.API.Repositories;
using Computrition.MenuService.API.Services;
using Moq;

namespace Computrition.MenuService.Tests
{
    public class PatientServiceTest
    {
        private readonly Mock<IPatientRepository> _mockPatientRepo;
        private readonly PatientService _service;
        public PatientServiceTest()
        {
            _mockPatientRepo = new Mock<IPatientRepository>();
            _service = new PatientService(_mockPatientRepo.Object);
        }
        [Fact]
        public async Task CreatePatientAsync_ValidPatient_CallsRepoCreateAsync()
        {
            var newPatient = new Patient { Name = "Jane Doe", DietaryRestrictionCode = DietaryRestriction.SF };
            await _service.CreatePatientAsync(newPatient);
            _mockPatientRepo.Verify(r => r.CreateAsync(It.Is<Patient>(p => p.Name == "Jane Doe")), Times.Once);
        }
        [Fact]
        public async Task GetPatientByIdAsync_ExistingId_ReturnsPatient()
        {
            int patientId = 1;
            var expectedPatient = new Patient { Id = patientId, Name = "John Smith" };
            _mockPatientRepo.Setup(r => r.GetPatientByIdAsync(patientId)).ReturnsAsync(expectedPatient);
            var result = await _service.GetPatientByIdAsync(patientId);

            Assert.NotNull(result);
            Assert.Equal(patientId, result.Id);
            Assert.Equal("John Smith", result.Name);
        }
        [Fact]
        public async Task GetAllPatientsAsync_NoPatientsExist_ReturnsEmptyCollection()
        {
            _mockPatientRepo.Setup(r => r.GetAllAsync())
                     .ReturnsAsync(new List<Patient>());

            // Act
            var result = await _service.GetAllPatientsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}