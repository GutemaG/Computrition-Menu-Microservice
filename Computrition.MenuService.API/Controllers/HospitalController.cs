using Computrition.MenuService.API.Models;
using Computrition.MenuService.API.MultiTenancy;
using Computrition.MenuService.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Computrition.MenuService.API.Controllers
{
    [SkipTenantHeader]
    [ApiController]
    [Route("/api/hospital")]
    public class HospitalsController : ControllerBase
    {
        public readonly IHospitalService _hospitalService;
        public HospitalsController(IHospitalService hospitalService)
        {
            _hospitalService = hospitalService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateHospital([FromBody] Hospital hospital)
        {
            await _hospitalService.CreateHospitalAsync(hospital);
            return CreatedAtAction(nameof(CreateHospital), new { id = hospital.Id }, hospital);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetHospitalById(int id)
        {
            var hospital = await _hospitalService.GetHospitalById(id);
            if (hospital == null) return NotFound("Hospital with the Id not found");
            return Ok(hospital);
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetHospitalBySlug(string slug)
        {
            var hospitals = await _hospitalService.GetHospitalBySlug(slug);
            return Ok(hospitals);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var hospitals = await _hospitalService.GetAllHospitalAsync();
            return Ok(hospitals);
        }


    }
}