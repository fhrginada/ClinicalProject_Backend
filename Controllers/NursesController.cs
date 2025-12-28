using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CLINICSYSTEM.Data.DTOs;
using CLINICSYSTEM.Services;

namespace CLINICSYSTEM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Nurse")]
    public class NursesController : ControllerBase
    {
        private readonly INurseService _nurseService;
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<NursesController> _logger;

        public NursesController(
            INurseService nurseService, 
            IAppointmentService appointmentService,
            ILogger<NursesController> logger)
        {
            _nurseService = nurseService;
            _appointmentService = appointmentService;
            _logger = logger;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && int.TryParse(userIdClaim.Value, out var id) ? id : 0;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized(new { message = "Invalid authentication token" });

                var dashboard = await _nurseService.GetDashboardAsync(userId);
                if (dashboard == null)
                    return NotFound(new { message = "Nurse profile not found" });

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving nurse dashboard");
                return StatusCode(500, new { message = "An error occurred while retrieving dashboard" });
            }
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var profile = await _nurseService.GetProfileAsync(userId);
                if (profile == null)
                    return NotFound(new { message = "Nurse profile not found" });

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving nurse profile");
                return StatusCode(500, new { message = "An error occurred while retrieving profile" });
            }
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateNurseProfileRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Request body is required" });
                }

                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var result = await _nurseService.UpdateProfileAsync(userId, request);
                if (!result)
                    return BadRequest(new { message = "Failed to update profile. Please check if the profile exists." });

                _logger.LogInformation("Nurse profile updated successfully for UserId: {UserId}", userId);
                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating nurse profile");
                return StatusCode(500, new { message = "An error occurred while updating profile" });
            }
        }

        [HttpGet("doctor/schedules")]
        public async Task<IActionResult> GetMyDoctorSchedules()
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                // For small clinic: nurse only sees their assigned doctor's schedule
                var schedules = await _nurseService.GetMyDoctorSchedulesAsync(userId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor schedules");
                return StatusCode(500, new { message = "An error occurred while retrieving schedules" });
            }
        }

        [HttpGet("doctors/{doctorId}/schedules")]
        public async Task<IActionResult> GetDoctorSchedule([FromRoute] int doctorId)
        {
            try
            {
                var schedules = await _nurseService.GetDoctorScheduleAsync(doctorId);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor schedule");
                return StatusCode(500, new { message = "An error occurred while retrieving schedule" });
            }
        }

        [HttpGet("doctors/{doctorId}/available-slots")]
        public async Task<IActionResult> GetAvailableSlots(
            [FromRoute] int doctorId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (startDate == DateTime.MinValue) startDate = DateTime.UtcNow.Date;
                if (endDate == DateTime.MinValue) endDate = startDate.AddDays(30);

                var slots = await _appointmentService.GetAvailableSlotsAsync(doctorId, startDate, endDate);
                return Ok(slots);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving available slots");
                return StatusCode(500, new { message = "An error occurred while retrieving available slots" });
            }
        }

        [HttpPost("appointments/book")]
        public async Task<IActionResult> BookAppointmentForPatient([FromBody] BookAppointmentForPatientRequest request)
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var nurseId = await _nurseService.GetNurseIdByUserIdAsync(userId);
                if (nurseId == null)
                    return NotFound(new { message = "Nurse profile not found" });

                var appointment = await _nurseService.BookAppointmentForPatientAsync(nurseId.Value, request);
                if (appointment == null)
                    return BadRequest(new { message = "Failed to book appointment. Please check if the time slot is available." });

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error booking appointment");
                return StatusCode(500, new { message = "An error occurred while booking appointment" });
            }
        }

        [HttpGet("appointments/upcoming")]
        public async Task<IActionResult> GetUpcomingAppointments([FromQuery] int days = 7)
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                // For small clinic: only show appointments for the nurse's assigned doctor
                var appointments = await _nurseService.GetUpcomingAppointmentsForNurseAsync(userId, days);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving upcoming appointments");
                return StatusCode(500, new { message = "An error occurred while retrieving appointments" });
            }
        }

        [HttpGet("tasks")]
        public async Task<IActionResult> GetMyTasks()
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var tasks = await _nurseService.GetMyTasksAsync(userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving tasks");
                return StatusCode(500, new { message = "An error occurred while retrieving tasks" });
            }
        }

        [HttpGet("tasks/{taskId}")]
        public async Task<IActionResult> GetTaskDetails([FromRoute] int taskId)
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var task = await _nurseService.GetTaskDetailsAsync(taskId, userId);
                if (task == null)
                    return NotFound(new { message = "Task not found" });

                return Ok(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving task details");
                return StatusCode(500, new { message = "An error occurred while retrieving task details" });
            }
        }

        [HttpPut("tasks/{taskId}/status")]
        public async Task<IActionResult> UpdateTaskStatus([FromRoute] int taskId, [FromBody] UpdateTaskStatusRequest request)
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                if (taskId != request.TaskId)
                    return BadRequest(new { message = "Task ID mismatch" });

                var result = await _nurseService.UpdateTaskStatusAsync(taskId, userId, request);
                if (!result)
                    return BadRequest(new { message = "Failed to update task status. Please check if the task exists and the status is valid." });

                return Ok(new { message = "Task status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating task status");
                return StatusCode(500, new { message = "An error occurred while updating task status" });
            }
        }
    }
}

