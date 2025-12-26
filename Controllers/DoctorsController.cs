using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CLINICSYSTEM.Data.DTOs;
using CLINICSYSTEM.Services;

namespace CLINICSYSTEM.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Doctor")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly ILogger<DoctorsController> _logger;

        public DoctorsController(IDoctorService doctorService, ILogger<DoctorsController> logger)
        {
            _doctorService = doctorService;
            _logger = logger;
        }

        private int GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var id))
            {
                return id;
            }
            return 0;
        }

        [HttpGet]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetAllDoctors()
        {
            try
            {
                var doctors = await _doctorService.GetAllDoctorsAsync();
                return Ok(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all doctors");
                return StatusCode(500, new { message = "An error occurred while retrieving doctors" });
            }
        }

        [HttpGet("profile")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0)
                {
                    _logger.LogWarning("Unauthorized access attempt - no valid user ID in token");
                    return Unauthorized(new { message = "Invalid authentication token" });
                }

                var profile = await _doctorService.GetProfileAsync(userId);
                if (profile == null)
                {
                    _logger.LogWarning("Doctor profile not found for UserId: {UserId}", userId);
                    return NotFound(new { message = "Doctor profile not found. Please complete registration." });
                }

                return Ok(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving doctor profile");
                return StatusCode(500, new { message = "An error occurred while retrieving profile" });
            }
        }

        [HttpPut("profile")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateDoctorProfileRequest request)
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var result = await _doctorService.UpdateProfileAsync(userId, request);
                if (!result)
                {
                    _logger.LogWarning("Failed to update profile for UserId: {UserId}", userId);
                    return BadRequest(new { message = "Failed to update profile" });
                }

                return Ok(new { message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor profile");
                return StatusCode(500, new { message = "An error occurred while updating profile" });
            }
        }

        [HttpGet("appointments/today")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetTodayAppointments()
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
                if (doctorId == null) return NotFound(new { message = "Doctor profile not found" });

                var appointments = await _doctorService.GetTodayAppointmentsAsync(doctorId.Value);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving today's appointments");
                return StatusCode(500, new { message = "An error occurred while retrieving appointments" });
            }
        }

        [HttpGet("appointments")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetAppointments([FromQuery] DateTime date)
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
                if (doctorId == null) return NotFound(new { message = "Doctor profile not found" });

                if (date == DateTime.MinValue) date = DateTime.UtcNow.Date;

                var appointments = await _doctorService.GetAppointmentsAsync(doctorId.Value, date);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments");
                return StatusCode(500, new { message = "An error occurred while retrieving appointments" });
            }
        }

        [HttpGet("patients/{patientId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetPatientRecord([FromRoute] int patientId)
        {
            try
            {
                var record = await _doctorService.GetPatientRecordAsync(patientId);
                if (record == null) return NotFound(new { message = "Patient record not found" });

                return Ok(record);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient record");
                return StatusCode(500, new { message = "An error occurred while retrieving patient record" });
            }
        }

        [HttpGet("patients/search")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> SearchPatients([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrEmpty(query))
                    return BadRequest(new { message = "Search query is required" });

                var patients = await _doctorService.SearchPatientsAsync(query);
                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching patients");
                return StatusCode(500, new { message = "An error occurred while searching patients" });
            }
        }

        [HttpGet("patients/{patientId}/images")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetPatientMedicalImages([FromRoute] int patientId)
        {
            try
            {
                var images = await _doctorService.GetPatientMedicalImagesAsync(patientId);
                return Ok(images);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patient medical images");
                return StatusCode(500, new { message = "An error occurred while retrieving images" });
            }
        }

        [HttpPost("schedule")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleRequest request)
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                // Get doctorId from userId
                var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
                if (doctorId == null)
                {
                    return NotFound(new { message = "Doctor profile not found" });
                }

                var result = await _doctorService.CreateScheduleAsync(doctorId.Value, request);
                if (!result) return BadRequest(new { message = "Failed to create schedule" });

                return Ok(new { message = "Schedule created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating schedule");
                return StatusCode(500, new { message = $"An error occurred while creating schedule: {ex.Message}" });
            }
        }

        [HttpGet("schedule")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetSchedules()
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
                if (doctorId == null) return NotFound(new { message = "Doctor profile not found" });

                var schedules = await _doctorService.GetSchedulesAsync(doctorId.Value);
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving schedules");
                return StatusCode(500, new { message = "An error occurred while retrieving schedules" });
            }
        }

        [HttpDelete("schedule/{scheduleId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteSchedule([FromRoute] int scheduleId)
        {
            try
            {
                var result = await _doctorService.DeleteScheduleAsync(scheduleId);
                if (!result) return NotFound(new { message = "Schedule not found" });

                return Ok(new { message = "Schedule deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting schedule");
                return StatusCode(500, new { message = "An error occurred while deleting schedule" });
            }
        }

        [HttpGet("nurse")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetAssignedNurse()
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
                if (doctorId == null) return NotFound(new { message = "Doctor profile not found" });

                var nurse = await _doctorService.GetAssignedNurseAsync(doctorId.Value);
                if (nurse == null) return NotFound(new { message = "No nurse assigned to this doctor" });

                return Ok(nurse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving assigned nurse");
                return StatusCode(500, new { message = "An error occurred while retrieving nurse" });
            }
        }

        [HttpPost("nurse/assign/{nurseId}")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AssignNurseToDoctor([FromRoute] int nurseId)
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
                if (doctorId == null) return NotFound(new { message = "Doctor profile not found" });

                var result = await _doctorService.AssignNurseToDoctorAsync(doctorId.Value, nurseId);
                if (!result) return BadRequest(new { message = "Failed to assign nurse. The nurse may already be assigned to another doctor." });

                return Ok(new { message = "Nurse assigned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning nurse to doctor");
                return StatusCode(500, new { message = "An error occurred while assigning nurse" });
            }
        }

        [HttpPost("nurse/tasks/assign")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> AssignTaskToNurse([FromBody] CreateNurseTaskRequest request)
        {
            try
            {
                var userId = GetUserId();
                if (userId == 0) return Unauthorized();

                var doctorId = await _doctorService.GetDoctorIdByUserIdAsync(userId);
                if (doctorId == null) return NotFound(new { message = "Doctor profile not found" });

                // For single doctor-nurse setup, automatically use the only nurse
                var totalDoctors = await _doctorService.GetTotalDoctorCountAsync();
                var totalNurses = await _doctorService.GetTotalNurseCountAsync();
                
                if (totalDoctors == 1 && totalNurses == 1)
                {
                    // In single doctor-nurse mode, automatically assign to the only nurse
                    // We don't need to verify nurse assignment since it's automatic
                    // The service will automatically determine the nurse ID
                }
                else
                {
                    // For multi-doctor/multi-nurse setups, verify the nurse is assigned to this doctor
                    if (!request.NurseId.HasValue)
                        return BadRequest(new { message = "Nurse ID is required in multi-doctor/nurse setups" });
                    
                    var nurse = await _doctorService.GetAssignedNurseAsync(doctorId.Value);
                    if (nurse == null || nurse.NurseId != request.NurseId.Value)
                        return BadRequest(new { message = "Nurse is not assigned to this doctor" });
                }

                var result = await _doctorService.AssignTaskToNurseAsync(doctorId.Value, request);
                if (!result) return BadRequest(new { message = "Failed to assign task. Please verify nurse ID and appointment ID if provided." });

                return Ok(new { message = "Task assigned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning task to nurse");
                return StatusCode(500, new { message = "An error occurred while assigning task" });
            }
        }
    }
}
