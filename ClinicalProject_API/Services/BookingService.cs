using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClinicalProject_API.Data;
using ClinicalProject_API.Models.Entities;
using ClinicalProject_API.Models.ViewModels;

namespace ClinicalProject_API.Services
{
    public interface IBookingService
    {
        Task<AppointmentResponse> CreateAppointmentAsync(AppointmentRequest request, string userId);
        Task<AppointmentResponse?> GetAppointmentByIdAsync(int id);
        Task<IEnumerable<AppointmentResponse>> GetAllAppointmentsAsync();
        Task<IEnumerable<AppointmentResponse>> GetAppointmentsByPatientIdAsync(int patientId);
        Task<IEnumerable<AppointmentResponse>> GetAppointmentsByDoctorIdAsync(int doctorId);
        Task<IEnumerable<AppointmentResponse>> GetAppointmentsByDateAsync(DateTime date);
        Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status, string reason, string userId);
        Task<bool> DeleteAppointmentAsync(int id, string userId);
        Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, string timeSlot);
    }

    public class BookingService : IBookingService
    {
        private readonly ClinicalDbContext _context;

        public BookingService(ClinicalDbContext context)
        {
            _context = context;
        }

        public async Task<AppointmentResponse> CreateAppointmentAsync(AppointmentRequest request, string userId)
        {
            var isAvailable = await IsTimeSlotAvailableAsync(request.DoctorId, request.AppointmentDate, request.TimeSlot);
            if (!isAvailable)
                throw new InvalidOperationException("This time slot is already booked.");

            var patient = await _context.Patients.FindAsync(request.PatientId);
            if (patient == null)
                throw new InvalidOperationException("Patient not found.");

            var doctor = await _context.Doctors.FindAsync(request.DoctorId);
            if (doctor == null)
                throw new InvalidOperationException("Doctor not found.");

            var appointment = new Appointment
            {
                PatientId = request.PatientId,
                DoctorId = request.DoctorId,
                AppointmentDate = request.AppointmentDate,
                TimeSlot = request.TimeSlot,
                ReasonForVisit = request.ReasonForVisit,
                Status = AppointmentStatus.Scheduled,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                IsDeleted = false
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return await GetAppointmentByIdAsync(appointment.Id) ?? throw new InvalidOperationException("Failed to create appointment.");
        }

        public async Task<AppointmentResponse?> GetAppointmentByIdAsync(int id)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Consultation)
                .Where(a => a.Id == id && !a.IsDeleted)
                .Select(a => new AppointmentResponse
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient != null ? a.Patient.FullName : string.Empty,
                    PatientEmail = a.Patient != null ? a.Patient.Email : string.Empty,
                    PatientPhone = a.Patient != null ? a.Patient.PhoneNumber : string.Empty,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor != null ? a.Doctor.FullName : string.Empty,
                    DoctorSpecialization = a.Doctor != null ? a.Doctor.Specialty : string.Empty,
                    AppointmentDate = a.AppointmentDate,
                    TimeSlot = a.TimeSlot ?? string.Empty,
                    Status = a.Status.ToString(),
                    ReasonForVisit = a.ReasonForVisit ?? string.Empty,
                    Notes = a.Notes ?? string.Empty,
                    CreatedAt = a.CreatedAt,
                    HasConsultation = a.Consultation != null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AppointmentResponse>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Consultation)
                .Where(a => !a.IsDeleted)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentResponse
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.FullName,
                    PatientEmail = a.Patient.Email,
                    PatientPhone = a.Patient.PhoneNumber,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    DoctorSpecialization = a.Doctor.Specialty,
                    AppointmentDate = a.AppointmentDate,
                    TimeSlot = a.TimeSlot,
                    Status = a.Status.ToString(),
                    ReasonForVisit = a.ReasonForVisit,
                    Notes = a.Notes,
                    CreatedAt = a.CreatedAt,
                    HasConsultation = a.Consultation != null
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AppointmentResponse>> GetAppointmentsByPatientIdAsync(int patientId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Consultation)
                .Where(a => a.PatientId == patientId && !a.IsDeleted)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentResponse
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.FullName,
                    PatientEmail = a.Patient.Email,
                    PatientPhone = a.Patient.PhoneNumber,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    DoctorSpecialization = a.Doctor.Specialty,
                    AppointmentDate = a.AppointmentDate,
                    TimeSlot = a.TimeSlot,
                    Status = a.Status.ToString(),
                    ReasonForVisit = a.ReasonForVisit,
                    Notes = a.Notes,
                    CreatedAt = a.CreatedAt,
                    HasConsultation = a.Consultation != null
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AppointmentResponse>> GetAppointmentsByDoctorIdAsync(int doctorId)
        {
            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Consultation)
                .Where(a => a.DoctorId == doctorId && !a.IsDeleted)
                .OrderByDescending(a => a.AppointmentDate)
                .Select(a => new AppointmentResponse
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.FullName,
                    PatientEmail = a.Patient.Email,
                    PatientPhone = a.Patient.PhoneNumber,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    DoctorSpecialization = a.Doctor.Specialty,
                    AppointmentDate = a.AppointmentDate,
                    TimeSlot = a.TimeSlot,
                    Status = a.Status.ToString(),
                    ReasonForVisit = a.ReasonForVisit,
                    Notes = a.Notes,
                    CreatedAt = a.CreatedAt,
                    HasConsultation = a.Consultation != null
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AppointmentResponse>> GetAppointmentsByDateAsync(DateTime date)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            return await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Consultation)
                .Where(a => a.AppointmentDate >= startDate &&
                           a.AppointmentDate < endDate &&
                           !a.IsDeleted)
                .OrderBy(a => a.AppointmentDate)
                .Select(a => new AppointmentResponse
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = a.Patient.FullName,
                    PatientEmail = a.Patient.Email,
                    PatientPhone = a.Patient.PhoneNumber,
                    DoctorId = a.DoctorId,
                    DoctorName = a.Doctor.FullName,
                    DoctorSpecialization = a.Doctor.Specialty,
                    AppointmentDate = a.AppointmentDate,
                    TimeSlot = a.TimeSlot,
                    Status = a.Status.ToString(),
                    ReasonForVisit = a.ReasonForVisit,
                    Notes = a.Notes,
                    CreatedAt = a.CreatedAt,
                    HasConsultation = a.Consultation != null
                })
                .ToListAsync();
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int appointmentId, string status, string reason, string userId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment == null || appointment.IsDeleted)
                return false;

            if (Enum.TryParse<AppointmentStatus>(status, out var statusEnum))
                appointment.Status = statusEnum;
            else
                throw new ArgumentException("Invalid status value");

            appointment.UpdatedAt = DateTime.UtcNow;
            appointment.UpdatedBy = userId;

            if (!string.IsNullOrEmpty(reason))
                appointment.Notes = string.IsNullOrEmpty(appointment.Notes)
                    ? reason
                    : $"{appointment.Notes}\n[{DateTime.UtcNow:yyyy-MM-dd HH:mm}] {reason}";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAppointmentAsync(int id, string userId)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null || appointment.IsDeleted)
                return false;

            appointment.IsDeleted = true;
            appointment.UpdatedAt = DateTime.UtcNow;
            appointment.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsTimeSlotAvailableAsync(int doctorId, DateTime date, string? timeSlot)
        {
            if (string.IsNullOrEmpty(timeSlot))
                return false;

            var exists = await _context.Appointments.AnyAsync(a =>
                a.DoctorId == doctorId &&
                a.AppointmentDate.Date == date.Date &&
                a.TimeSlot == timeSlot &&
                a.Status != AppointmentStatus.Cancelled &&
                !a.IsDeleted);

            return !exists;
        }
    }
}

