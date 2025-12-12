using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ClinicalProject_API.Models.ViewModels;
using ClinicalProject_API.Services;

namespace ClinicalProject_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConsultationController : ControllerBase
    {
        private readonly IConsultationService _consultationService;

        public ConsultationController(IConsultationService consultationService)
        {
            _consultationService = consultationService;
        }

        // CREATE Consultation (handles new/existing patient)
        [HttpPost]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> CreateConsultation([FromBody] ConsultationRequest request)
        {
            try
            {
                var userId = User?.Identity?.Name ?? "system";

                // Pass request to service, which will handle patient creation if needed
                var result = await _consultationService.CreateConsultationAsync(request, userId);
                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetConsultationById(int id)
        {
            var result = await _consultationService.GetConsultationByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { success = false, message = "Consultation not found" });
            }
            return Ok(new { success = true, data = result });
        }

        [HttpGet("appointment/{appointmentId}")]
        public async Task<IActionResult> GetConsultationByAppointmentId(int appointmentId)
        {
            var result = await _consultationService.GetConsultationByAppointmentIdAsync(appointmentId);
            if (result == null)
            {
                return NotFound(new { success = false, message = "Consultation not found for this appointment" });
            }
            return Ok(new { success = true, data = result });
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetConsultationsByPatientId(int patientId)
        {
            var result = await _consultationService.GetConsultationsByPatientIdAsync(patientId);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("doctor/{doctorId}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> GetConsultationsByDoctorId(int doctorId)
        {
            var result = await _consultationService.GetConsultationsByDoctorIdAsync(doctorId);
            return Ok(new { success = true, data = result });
        }

        // UPDATE Consultation
        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        public async Task<IActionResult> UpdateConsultation(int id, [FromBody] ConsultationRequest request)
        {
            try
            {
                var userId = User?.Identity?.Name ?? "system";

                var result = await _consultationService.UpdateConsultationAsync(id, request, userId);
                return Ok(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        // MARK Consultation AS PAID
        [HttpPut("{id}/mark-paid")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkConsultationAsPaid(int id)
        {
            var userId = User?.Identity?.Name ?? "system";

            var result = await _consultationService.MarkConsultationAsPaidAsync(id, userId);

            if (!result)
            {
                return NotFound(new { success = false, message = "Consultation not found" });
            }

            return Ok(new { success = true, message = "Consultation marked as paid" });
        }
    }
}
