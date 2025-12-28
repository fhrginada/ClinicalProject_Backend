using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CLINICSYSTEM.Data;
using CLINICSYSTEM.Data.DTOs;
using CLINICSYSTEM.Services;

namespace CLINICSYSTEM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ClinicDbContext _context;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(
            IAppointmentService appointmentService,
            ClinicDbContext context,
            ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService;
            _context = context;
            _logger = logger;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : 0;
        }

        [HttpGet("available-slots")]
        public async Task<IActionResult> GetAvailableSlots(
            [FromQuery] int doctorId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate == DateTime.MinValue) startDate = DateTime.UtcNow.Date;
            if (endDate == DateTime.MinValue) endDate = startDate.AddDays(30);

            var slots = await _appointmentService.GetAvailableSlotsAsync(doctorId, startDate, endDate);
            return Ok(slots);
        }

        [Authorize(Roles = "Patient")]
        [HttpPost("book")]
        public async Task<IActionResult> BookAppointment([FromBody] BookAppointmentRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Request body is required" });
                }

                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                // Get patient ID from user ID
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient == null)
                {
                    return BadRequest(new { message = "Patient profile not found. Please complete your profile registration." });
                }

                var appointment = await _appointmentService.BookAppointmentAsync(patient.PatientId, request);
                if (appointment == null)
                {
                    return BadRequest(new { message = "Failed to book appointment. Please try again." });
                }

                _logger.LogInformation("Appointment booked successfully: AppointmentId={AppointmentId}, PatientId={PatientId}, DoctorId={DoctorId}", 
                    appointment.AppointmentId, patient.PatientId, request.DoctorId);

                return Ok(appointment);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Appointment booking failed: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error booking appointment");
                return StatusCode(500, new { message = "An error occurred while booking the appointment. Please try again." });
            }
        }

        [Authorize(Roles = "Patient")]
        [HttpPut("reschedule")]
        public async Task<IActionResult> RescheduleAppointment([FromBody] RescheduleAppointmentRequest request)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var result = await _appointmentService.RescheduleAppointmentAsync(userId, request);
            if (!result) return BadRequest("Failed to reschedule appointment");

            return Ok(new { message = "Appointment rescheduled successfully" });
        }

        [Authorize(Roles = "Patient")]
        [HttpPut("cancel")]
        public async Task<IActionResult> CancelAppointment([FromBody] CancelAppointmentRequest request)
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var result = await _appointmentService.CancelAppointmentAsync(userId, request);
            if (!result) return BadRequest("Failed to cancel appointment");

            return Ok(new { message = "Appointment cancelled successfully" });
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("my-appointments")]
        public async Task<IActionResult> GetMyAppointments()
        {
            var userId = GetUserId();
            if (userId == 0) return Unauthorized();

            var appointments = await _appointmentService.GetPatientAppointmentsAsync(userId);
            return Ok(appointments);
        }

        [HttpGet("{appointmentId}")]
        public async Task<IActionResult> GetAppointmentDetails([FromRoute] int appointmentId)
        {
            var appointment = await _appointmentService.GetAppointmentDetailsAsync(appointmentId);
            if (appointment == null) return NotFound();

            return Ok(appointment);
        }
    }
}
