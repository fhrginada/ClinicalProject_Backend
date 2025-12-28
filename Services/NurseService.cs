using Microsoft.EntityFrameworkCore;
using CLINICSYSTEM.Data;
using CLINICSYSTEM.Data.DTOs;
using CLINICSYSTEM.Models;

namespace CLINICSYSTEM.Services
{
    public class NurseService : INurseService
    {
        private readonly ClinicDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly ILogger<NurseService> _logger;

        public NurseService(
            ClinicDbContext context, 
            INotificationService notificationService,
            ILogger<NurseService> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<int?> GetNurseIdByUserIdAsync(int userId)
        {
            var nurse = await _context.Nurses.FirstOrDefaultAsync(n => n.UserId == userId);
            return nurse?.NurseId;
        }

        public async Task<NurseDashboardDTO?> GetDashboardAsync(int userId)
        {
            var nurse = await _context.Nurses
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.UserId == userId);

            if (nurse == null) return null;

            var today = DateTime.UtcNow.Date;
            var endDate = today.AddDays(7);

            // For single doctor-nurse setup, automatically use the only doctor
            int? doctorIdFilter;
            var totalDoctors = await _context.Doctors.CountAsync();
            var totalNurses = await _context.Nurses.CountAsync();
            
            if (totalDoctors == 1 && totalNurses == 1)
            {
                // In single doctor-nurse mode, get the only doctor's appointments
                var doctor = await _context.Doctors.FirstOrDefaultAsync();
                doctorIdFilter = doctor?.DoctorId;
                
                // Also automatically link the nurse to the doctor if not already linked
                if (nurse.DoctorId != doctorIdFilter && doctorIdFilter.HasValue)
                {
                    nurse.DoctorId = doctorIdFilter.Value;
                    nurse.UpdatedAt = DateTime.UtcNow;
                    _context.Nurses.Update(nurse);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // For multi-doctor/multi-nurse setups, use the assigned doctor
                doctorIdFilter = nurse.DoctorId;
            }

            // Get upcoming appointments (next 7 days) for assigned doctor
            var upcomingAppointments = await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.TimeSlot)
                .Where(a => (doctorIdFilter == null || a.DoctorId == doctorIdFilter) &&
                           a.TimeSlot.SlotDate >= today && 
                           a.TimeSlot.SlotDate <= endDate &&
                           a.Status != "Cancelled" &&
                           a.Status != "Completed")
                .OrderBy(a => a.TimeSlot.SlotDate)
                .ThenBy(a => a.TimeSlot.StartTime)
                .Take(10)
                .ToListAsync();

            // Get tasks
            var allTasks = await _context.NurseTasks
                .Include(nt => nt.Doctor)
                    .ThenInclude(d => d.User)
                .Where(nt => nt.NurseId == nurse.NurseId)
                .OrderByDescending(nt => nt.CreatedAt)
                .Take(10)
                .ToListAsync();

            var pendingTasks = allTasks.Where(t => t.Status == "Pending").ToList();
            var inProgressTasks = allTasks.Where(t => t.Status == "InProgress").ToList();

            // Count doctors with schedules today
            var todayDayOfWeek = today.DayOfWeek.ToString();
            var doctorsToday = await _context.DoctorSchedules
                .Include(ds => ds.Doctor)
                .Where(ds => ds.DayOfWeek == todayDayOfWeek)
                .Select(ds => ds.DoctorId)
                .Distinct()
                .CountAsync();

            return new NurseDashboardDTO
            {
                TotalUpcomingAppointments = upcomingAppointments.Count,
                TotalPendingTasks = pendingTasks.Count,
                TotalInProgressTasks = inProgressTasks.Count,
                TotalDoctorsToday = doctorsToday,
                UpcomingAppointments = upcomingAppointments.Select(a => new UpcomingAppointmentDTO
                {
                    AppointmentId = a.AppointmentId,
                    DoctorName = $"{a.Doctor.User.FirstName} {a.Doctor.User.LastName}",
                    PatientName = $"{a.Patient.User.FirstName} {a.Patient.User.LastName}",
                    AppointmentDate = a.TimeSlot.SlotDate,
                    StartTime = a.TimeSlot.StartTime,
                    EndTime = a.TimeSlot.EndTime,
                    Status = a.Status,
                    ReasonForVisit = a.ReasonForVisit
                }).ToList(),
                RecentTasks = allTasks.Select(t => new NurseTaskDTO
                {
                    TaskId = t.TaskId,
                    DoctorName = $"{t.Doctor.User.FirstName} {t.Doctor.User.LastName}",
                    Title = t.Title,
                    Description = t.Description,
                    Status = t.Status,
                    DueDate = t.DueDate,
                    CreatedAt = t.CreatedAt,
                    CompletedAt = t.CompletedAt,
                    RelatedAppointmentId = t.RelatedAppointmentId,
                    Notes = t.Notes
                }).ToList()
            };
        }

        public async Task<NurseProfileDTO?> GetProfileAsync(int userId)
        {
            var nurse = await _context.Nurses
                .Include(n => n.User)
                .FirstOrDefaultAsync(n => n.UserId == userId);

            if (nurse?.User == null) return null;

            return new NurseProfileDTO
            {
                NurseId = nurse.NurseId,
                UserId = nurse.UserId,
                FirstName = nurse.User.FirstName,
                LastName = nurse.User.LastName,
                Email = nurse.User.Email,
                PhoneNumber = nurse.User.PhoneNumber,
                LicenseNumber = nurse.LicenseNumber,
                Department = nurse.Department
            };
        }

        public async Task<List<DoctorScheduleViewDTO>> GetAllDoctorSchedulesAsync()
        {
            // For small clinic: get all schedules (there's only one doctor)
            return await _context.DoctorSchedules
                .Include(ds => ds.Doctor)
                    .ThenInclude(d => d.User)
                .Select(ds => new DoctorScheduleViewDTO
                {
                    ScheduleId = ds.ScheduleId,
                    DoctorId = ds.DoctorId,
                    DoctorName = ds.Doctor.User.FirstName + " " + ds.Doctor.User.LastName,
                    Specialization = ds.Doctor.Specialization,
                    DayOfWeek = ds.DayOfWeek,
                    StartTime = ds.StartTime,
                    EndTime = ds.EndTime,
                    SlotDurationMinutes = ds.SlotDurationMinutes
                })
                .OrderBy(ds => ds.DayOfWeek)
                .ThenBy(ds => ds.StartTime)
                .ToListAsync();
        }

        public async Task<List<DoctorScheduleViewDTO>> GetMyDoctorSchedulesAsync(int userId)
        {
            // For single doctor-nurse setup, get the only doctor's schedules
            var nurse = await _context.Nurses
                .Include(n => n.Doctor)
                .FirstOrDefaultAsync(n => n.UserId == userId);

            if (nurse == null) return new List<DoctorScheduleViewDTO>();
            
            var totalDoctors = await _context.Doctors.CountAsync();
            var totalNurses = await _context.Nurses.CountAsync();
            
            if (totalDoctors == 1 && totalNurses == 1)
            {
                // In single doctor-nurse mode, get the only doctor's schedules
                var singleDoctor = await _context.Doctors.FirstOrDefaultAsync();
                if (singleDoctor == null) return new List<DoctorScheduleViewDTO>();
                
                // Also automatically link the nurse to the doctor if not already linked
                if (nurse.DoctorId != singleDoctor.DoctorId)
                {
                    nurse.DoctorId = singleDoctor.DoctorId;
                    nurse.UpdatedAt = DateTime.UtcNow;
                    _context.Nurses.Update(nurse);
                    await _context.SaveChangesAsync();
                }
                
                return await GetDoctorScheduleAsync(singleDoctor.DoctorId);
            }
            else
            {
                // For multi-doctor/multi-nurse setups, use the assigned doctor
                if (nurse?.DoctorId == null) return new List<DoctorScheduleViewDTO>();
                
                return await GetDoctorScheduleAsync(nurse.DoctorId.Value);
            }
        }

        public async Task<List<DoctorScheduleViewDTO>> GetDoctorScheduleAsync(int doctorId)
        {
            return await _context.DoctorSchedules
                .Include(ds => ds.Doctor)
                    .ThenInclude(d => d.User)
                .Where(ds => ds.DoctorId == doctorId)
                .Select(ds => new DoctorScheduleViewDTO
                {
                    ScheduleId = ds.ScheduleId,
                    DoctorId = ds.DoctorId,
                    DoctorName = ds.Doctor.User.FirstName + " " + ds.Doctor.User.LastName,
                    Specialization = ds.Doctor.Specialization,
                    DayOfWeek = ds.DayOfWeek,
                    StartTime = ds.StartTime,
                    EndTime = ds.EndTime,
                    SlotDurationMinutes = ds.SlotDurationMinutes
                })
                .OrderBy(ds => ds.DayOfWeek)
                .ThenBy(ds => ds.StartTime)
                .ToListAsync();
        }

        public async Task<AppointmentDTO?> BookAppointmentForPatientAsync(int nurseId, BookAppointmentForPatientRequest request)
        {
            // Verify nurse exists and get their assigned doctor
            var nurse = await _context.Nurses.FindAsync(nurseId);
            if (nurse == null) return null;
            
            // For single doctor-nurse setup, automatically use the only doctor
            var totalDoctors = await _context.Doctors.CountAsync();
            var totalNurses = await _context.Nurses.CountAsync();
            
            int doctorId;
            if (totalDoctors == 1 && totalNurses == 1)
            {
                // In single doctor-nurse mode, automatically use the only doctor
                var singleDoctor = await _context.Doctors.FirstOrDefaultAsync();
                if (singleDoctor == null) return null;
                doctorId = singleDoctor.DoctorId;
                
                // Automatically link the nurse to the doctor if not already linked
                if (nurse.DoctorId != doctorId)
                {
                    nurse.DoctorId = doctorId;
                    nurse.UpdatedAt = DateTime.UtcNow;
                    _context.Nurses.Update(nurse);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // For multi-doctor/multi-nurse setups, verify nurse is assigned to the requested doctor
                if (nurse.DoctorId != request.DoctorId)
                    return null; // Nurse not assigned to this doctor
                doctorId = request.DoctorId;
            }

            // Verify patient exists
            var patient = await _context.Patients.FindAsync(request.PatientId);
            if (patient == null) return null;

            // Verify doctor exists
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null) return null;

            // Check if time slot is available
            var timeSlot = await _context.TimeSlots.FindAsync(request.TimeSlotId);
            if (timeSlot == null || timeSlot.Status != "Available")
                return null;

            // Verify time slot belongs to the doctor
            var timeSlotWithSchedule = await _context.TimeSlots
                .Include(ts => ts.Schedule)
                .FirstOrDefaultAsync(ts => ts.TimeSlotId == request.TimeSlotId);
            
            if (timeSlotWithSchedule == null || timeSlotWithSchedule.Schedule == null)
                return null;
            
            if (timeSlotWithSchedule.Schedule.DoctorId != doctorId)
                return null;

            // Create appointment
            var appointment = new AppointmentModel
            {
                DoctorId = doctorId, // Use the correct doctorId
                PatientId = request.PatientId,
                TimeSlotId = request.TimeSlotId,
                Status = "Scheduled",
                ReasonForVisit = request.ReasonForVisit,
                BookedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            timeSlot.Status = "Booked";

            _context.Appointments.Add(appointment);
            _context.TimeSlots.Update(timeSlot);
            await _context.SaveChangesAsync();

            // Send notification to doctor
            if (doctor.UserId != null)
            {
                await _notificationService.CreateNotificationAsync(doctor.UserId, new CreateNotificationRequest
                {
                    Title = "New Appointment",
                    Message = $"Nurse booked an appointment for patient on {timeSlot.SlotDate:MMM dd, yyyy}",
                    Type = "Appointment"
                });
            }

            // Return appointment details
            var savedAppointment = await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.TimeSlot)
                .FirstOrDefaultAsync(a => a.AppointmentId == appointment.AppointmentId);

            if (savedAppointment == null) return null;

            return new AppointmentDTO
            {
                AppointmentId = savedAppointment.AppointmentId,
                DoctorName = $"{savedAppointment.Doctor.User.FirstName} {savedAppointment.Doctor.User.LastName}",
                PatientName = $"{savedAppointment.Patient.User.FirstName} {savedAppointment.Patient.User.LastName}",
                AppointmentDate = savedAppointment.TimeSlot.SlotDate,
                StartTime = savedAppointment.TimeSlot.StartTime,
                EndTime = savedAppointment.TimeSlot.EndTime,
                Status = savedAppointment.Status,
                ReasonForVisit = savedAppointment.ReasonForVisit
            };
        }

        public async Task<List<NurseTaskDTO>> GetMyTasksAsync(int userId)
        {
            var nurse = await _context.Nurses.FirstOrDefaultAsync(n => n.UserId == userId);
            if (nurse == null) return new List<NurseTaskDTO>();

            var tasks = await _context.NurseTasks
                .Include(nt => nt.Doctor)
                    .ThenInclude(d => d.User)
                .Where(nt => nt.NurseId == nurse.NurseId)
                .OrderByDescending(nt => nt.CreatedAt)
                .ToListAsync();

            return tasks.Select(t => new NurseTaskDTO
            {
                TaskId = t.TaskId,
                DoctorName = $"{t.Doctor.User.FirstName} {t.Doctor.User.LastName}",
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                CompletedAt = t.CompletedAt,
                RelatedAppointmentId = t.RelatedAppointmentId,
                Notes = t.Notes
            }).ToList();
        }

        public async Task<NurseTaskDTO?> GetTaskDetailsAsync(int taskId, int userId)
        {
            var nurse = await _context.Nurses.FirstOrDefaultAsync(n => n.UserId == userId);
            if (nurse == null) return null;

            var task = await _context.NurseTasks
                .Include(nt => nt.Doctor)
                    .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(nt => nt.TaskId == taskId && nt.NurseId == nurse.NurseId);

            if (task == null) return null;

            return new NurseTaskDTO
            {
                TaskId = task.TaskId,
                DoctorName = $"{task.Doctor.User.FirstName} {task.Doctor.User.LastName}",
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt,
                CompletedAt = task.CompletedAt,
                RelatedAppointmentId = task.RelatedAppointmentId,
                Notes = task.Notes
            };
        }

        public async Task<bool> UpdateTaskStatusAsync(int taskId, int userId, UpdateTaskStatusRequest request)
        {
            var nurse = await _context.Nurses.FirstOrDefaultAsync(n => n.UserId == userId);
            if (nurse == null) return false;

            var task = await _context.NurseTasks
                .FirstOrDefaultAsync(nt => nt.TaskId == taskId && nt.NurseId == nurse.NurseId);

            if (task == null) return false;

            // Validate status
            var validStatuses = new[] { "Pending", "InProgress", "Completed", "Cancelled" };
            if (!validStatuses.Contains(request.Status))
                return false;

            task.Status = request.Status;
            task.UpdatedAt = DateTime.UtcNow;
            if (request.Notes != null)
                task.Notes = request.Notes;

            if (request.Status == "Completed")
                task.CompletedAt = DateTime.UtcNow;
            else if (request.Status != "Completed" && task.CompletedAt.HasValue)
                task.CompletedAt = null;

            _context.NurseTasks.Update(task);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<UpcomingAppointmentDTO>> GetUpcomingAppointmentsAsync(int days = 7)
        {
            var today = DateTime.UtcNow.Date;
            var endDate = today.AddDays(days);

            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.TimeSlot)
                .Where(a => a.TimeSlot.SlotDate >= today && 
                           a.TimeSlot.SlotDate <= endDate &&
                           a.Status != "Cancelled")
                .OrderBy(a => a.TimeSlot.SlotDate)
                .ThenBy(a => a.TimeSlot.StartTime)
                .ToListAsync();

            return appointments.Select(a => new UpcomingAppointmentDTO
            {
                AppointmentId = a.AppointmentId,
                DoctorName = $"{a.Doctor.User.FirstName} {a.Doctor.User.LastName}",
                PatientName = $"{a.Patient.User.FirstName} {a.Patient.User.LastName}",
                AppointmentDate = a.TimeSlot.SlotDate,
                StartTime = a.TimeSlot.StartTime,
                EndTime = a.TimeSlot.EndTime,
                Status = a.Status,
                ReasonForVisit = a.ReasonForVisit
            }).ToList();
        }

        public async Task<List<UpcomingAppointmentDTO>> GetUpcomingAppointmentsForNurseAsync(int userId, int days = 7)
        {
            var nurse = await _context.Nurses.FirstOrDefaultAsync(n => n.UserId == userId);
            if (nurse == null) return new List<UpcomingAppointmentDTO>();

            var today = DateTime.UtcNow.Date;
            var endDate = today.AddDays(days);
            
            // For single doctor-nurse setup, automatically get appointments for the only doctor
            var totalDoctors = await _context.Doctors.CountAsync();
            var totalNurses = await _context.Nurses.CountAsync();
            int? doctorIdFilter;
            
            if (totalDoctors == 1 && totalNurses == 1)
            {
                // In single doctor-nurse mode, get appointments for the only doctor
                var doctor = await _context.Doctors.FirstOrDefaultAsync();
                doctorIdFilter = doctor?.DoctorId;
                
                // Also automatically link the nurse to the doctor if not already linked
                if (nurse.DoctorId != doctorIdFilter && doctorIdFilter.HasValue)
                {
                    nurse.DoctorId = doctorIdFilter.Value;
                    nurse.UpdatedAt = DateTime.UtcNow;
                    _context.Nurses.Update(nurse);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                // For multi-doctor/multi-nurse setups, use the assigned doctor
                doctorIdFilter = nurse.DoctorId;
            }

            // Only show appointments for the nurse's assigned doctor
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.User)
                .Include(a => a.Patient)
                    .ThenInclude(p => p.User)
                .Include(a => a.TimeSlot)
                .Where(a => doctorIdFilter != null && a.DoctorId == doctorIdFilter &&
                           a.TimeSlot.SlotDate >= today && 
                           a.TimeSlot.SlotDate <= endDate &&
                           a.Status != "Cancelled")
                .OrderBy(a => a.TimeSlot.SlotDate)
                .ThenBy(a => a.TimeSlot.StartTime)
                .ToListAsync();

            return appointments.Select(a => new UpcomingAppointmentDTO
            {
                AppointmentId = a.AppointmentId,
                DoctorName = $"{a.Doctor.User.FirstName} {a.Doctor.User.LastName}",
                PatientName = $"{a.Patient.User.FirstName} {a.Patient.User.LastName}",
                AppointmentDate = a.TimeSlot.SlotDate,
                StartTime = a.TimeSlot.StartTime,
                EndTime = a.TimeSlot.EndTime,
                Status = a.Status,
                ReasonForVisit = a.ReasonForVisit
            }).ToList();
        }
    }
}

