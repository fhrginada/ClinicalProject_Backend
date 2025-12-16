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
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // Create appointment (handles new or existing patient)
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] AppointmentRequest request)
        {
            var userId = User?.Identity?.Name ?? "system";
            var result = await _bookingService.CreateAppointmentAsync(request, userId);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var result = await _bookingService.GetAppointmentByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { success = false, message = "Appointment not found" });
            }
            return Ok(new { success = true, data = result });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Doctor,Nurse")]
        public async Task<IActionResult> GetAllAppointments()
        {
            var result = await _bookingService.GetAllAppointmentsAsync();
            return Ok(new { success = true, data = result });
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetAppointmentsByPatientId(int patientId)
        {
            var result = await _bookingService.GetAppointmentsByPatientIdAsync(patientId);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<IActionResult> GetAppointmentsByDoctorId(int doctorId)
        {
            var result = await _bookingService.GetAppointmentsByDoctorIdAsync(doctorId);
            return Ok(new { success = true, data = result });
        }

        [HttpGet("date/{date}")]
        [Authorize(Roles = "Admin,Doctor,Nurse")]
        public async Task<IActionResult> GetAppointmentsByDate(DateTime date)
        {
            var result = await _bookingService.GetAppointmentsByDateAsync(date);
            return Ok(new { success = true, data = result });
        }

        [HttpPut("status")]
        [Authorize(Roles = "Admin,Doctor,Nurse")]
        public async Task<IActionResult> UpdateAppointmentStatus([FromBody] AppointmentStatusRequest request)
        {
            var userId = User?.Identity?.Name ?? "system";
            var safeReason = request.Reason ?? string.Empty;

            var result = await _bookingService.UpdateAppointmentStatusAsync(
                request.AppointmentId,
                request.Status,
                safeReason,
                userId
            );

            if (!result)
            {
                return NotFound(new { success = false, message = "Appointment not found" });
            }

            return Ok(new { success = true, message = "Appointment status updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var userId = User?.Identity?.Name ?? "system";
            var result = await _bookingService.DeleteAppointmentAsync(id, userId);

            if (!result)
            {
                return NotFound(new { success = false, message = "Appointment not found" });
            }

            return Ok(new { success = true, message = "Appointment deleted successfully" });
        }

        [HttpGet("check-availability")]
        public async Task<IActionResult> CheckAvailability([FromQuery] int doctorId, [FromQuery] DateTime date, [FromQuery] string timeSlot)
        {
            var isAvailable = await _bookingService.IsTimeSlotAvailableAsync(doctorId, date, timeSlot);
            return Ok(new { success = true, isAvailable = isAvailable });
        }
    }
}
