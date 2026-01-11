using Computrition.MenuService.API.Dtos;
using Computrition.MenuService.API.Models;
using Computrition.MenuService.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Computrition.MenuService.API.Controllers
{
    [ApiController]
    [Route("api/patients")]
    public class PatientsController: ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IPatientService _patientService;
        public PatientsController(IMenuService menuService, IPatientService patientService)
        {
            _menuService = menuService;
            _patientService = patientService;
        }

        [HttpGet("{patientId}/allowed-menu")]
        public async Task<IActionResult> GetAllowedMenu(int patientId)
        {
            var menuItems = await _menuService.GetAllowedMenuForPatientAsync(patientId);
            if(menuItems == null || !menuItems.Any())
            {
                return NotFound($"No allowed menu items found for Patient ID {patientId}.");
            }
            return Ok(menuItems);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var patients = await _patientService.GetAllPatientsAsync();
            return Ok(patients);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null) return NotFound("Patient with the Id not found");
            return Ok(patient);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] CreatePatientDto patient)
        {
            if (string.IsNullOrWhiteSpace(patient.Name))
            {
                throw new ArgumentException("Name is required.");
            }
            var newPatient = new Patient
            {
                Name = patient.Name,
                DietaryRestrictionCode = patient.DietaryRestrictionCode
            };

            await _patientService.CreatePatientAsync(newPatient);
            return CreatedAtAction(nameof(CreatePatient), new { id = newPatient.Id },newPatient);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Patient patient)
        {
            if (id != patient.Id) return BadRequest("Ids doesn't match");
            var existingPatient = await _patientService.GetPatientByIdAsync(id);
            if (existingPatient == null) return NotFound("Patient with the Id not found");
            await _patientService.UpdatePatientAsync(patient);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _patientService.DeletePatientAsync(id);
            return NoContent();
        }

    }
}