using ClinicalProject_API.Models.Entities;
using ClinicalProject_API.Models.ViewModels;
using ClinicalProject_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicalProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _service;

        public PrescriptionController(IPrescriptionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var prescription = await _service.GetByIdAsync(id);
            if (prescription == null) return NotFound();
            return Ok(prescription);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PrescriptionRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var prescription = new Prescription
            {
                DoctorId = request.DoctorId,
                PatientId = request.PatientId,
                Doctor = null!, // áæ ÚÇíÒÉ ÊÍáí CS9035
                Patient = null!,
                Details = request.Lines.Select(l => new PrescriptionDetail
                {
                    MedicationId = l.MedicationId,
                    Dose = l.Dosage,
                    Frequency = l.Frequency,
                    Notes = l.Instructions
                }).ToList()
            };

            await _service.AddAsync(prescription);
            return CreatedAtAction(nameof(GetById), new { id = prescription.PrescriptionId }, prescription);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PrescriptionRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existing = await _service.GetByIdAsync(id);
            if (existing == null) return NotFound();

            existing.DoctorId = request.DoctorId;
            existing.PatientId = request.PatientId;
            existing.Details = request.Lines.Select(l => new PrescriptionDetail
            {
                MedicationId = l.MedicationId,
                Dose = l.Dosage,
                Frequency = l.Frequency,
                Notes = l.Instructions
            }).ToList();

            await _service.UpdateAsync(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
