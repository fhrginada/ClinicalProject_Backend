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
    public interface IConsultationService
    {
        Task<ConsultationResponse> CreateConsultationAsync(ConsultationRequest request, string userId);
        Task<ConsultationResponse> GetConsultationByIdAsync(int id);
        Task<ConsultationResponse> GetConsultationByAppointmentIdAsync(int appointmentId);
        Task<IEnumerable<ConsultationResponse>> GetConsultationsByPatientIdAsync(int patientId);
        Task<IEnumerable<ConsultationResponse>> GetConsultationsByDoctorIdAsync(int doctorId);
        Task<ConsultationResponse> UpdateConsultationAsync(int id, ConsultationRequest request, string userId);
        Task<bool> MarkConsultationAsPaidAsync(int id, string userId);
    }

    public class ConsultationService : IConsultationService
    {
        private readonly ClinicalDbContext _context;

        public ConsultationService(ClinicalDbContext context)
        {
            _context = context;
        }

        public async Task<ConsultationResponse> CreateConsultationAsync(ConsultationRequest request, string userId)
        {
            if (request.AppointmentId == null)
            {
                throw new ArgumentException("AppointmentId cannot be null");
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(a => a.Id == request.AppointmentId.Value && !a.IsDeleted);

            if (appointment == null)
            {
                throw new InvalidOperationException("Appointment not found.");
            }

            var existingConsultation = await _context.Consultations
                .AnyAsync(c => c.AppointmentId == request.AppointmentId.Value && !c.IsDeleted);

            if (existingConsultation)
            {
                throw new InvalidOperationException("Consultation already exists for this appointment.");
            }

            var consultation = new Consultation
            {
                AppointmentId = request.AppointmentId.Value,
                ConsultationDate = DateTime.UtcNow,
                Symptoms = request.Symptoms,
                Diagnosis = request.Diagnosis,
                Prescription = request.Prescription,
                TreatmentPlan = request.TreatmentPlan,
                FollowUpInstructions = request.FollowUpInstructions,
                FollowUpDate = request.FollowUpDate,
                ConsultationFee = request.ConsultationFee,
                IsPaid = false,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                UpdatedBy = userId,
                IsDeleted = false
            };

            _context.Consultations.Add(consultation);

            // Update appointment status using enum
            appointment.Status = AppointmentStatus.Completed;
            appointment.UpdatedAt = DateTime.UtcNow;
            appointment.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            return await GetConsultationByIdAsync(consultation.Id);
        }

        public async Task<ConsultationResponse> GetConsultationByIdAsync(int id)
        {
            var consultation = await _context.Consultations
                .Include(c => c.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(c => c.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Where(c => c.Id == id && !c.IsDeleted)
                .Select(c => new ConsultationResponse
                {
                    Id = c.Id,
                    AppointmentId = c.AppointmentId,
                    ConsultationDate = c.ConsultationDate,
                    Symptoms = c.Symptoms,
                    Diagnosis = c.Diagnosis,
                    Prescription = c.Prescription,
                    TreatmentPlan = c.TreatmentPlan,
                    FollowUpInstructions = c.FollowUpInstructions,
                    FollowUpDate = c.FollowUpDate,
                    ConsultationFee = c.ConsultationFee,
                    IsPaid = c.IsPaid,
                    Notes = c.Notes,
                    CreatedAt = c.CreatedAt,
                    PatientName = c.Appointment.Patient.FullName,
                    DoctorName = c.Appointment.Doctor.FullName,
                    AppointmentDate = c.Appointment.AppointmentDate
                })
                .FirstOrDefaultAsync();

            return consultation;
        }

        public async Task<ConsultationResponse> GetConsultationByAppointmentIdAsync(int appointmentId)
        {
            var consultation = await _context.Consultations
                .Include(c => c.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(c => c.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Where(c => c.AppointmentId == appointmentId && !c.IsDeleted)
                .Select(c => new ConsultationResponse
                {
                    Id = c.Id,
                    AppointmentId = c.AppointmentId,
                    ConsultationDate = c.ConsultationDate,
                    Symptoms = c.Symptoms,
                    Diagnosis = c.Diagnosis,
                    Prescription = c.Prescription,
                    TreatmentPlan = c.TreatmentPlan,
                    FollowUpInstructions = c.FollowUpInstructions,
                    FollowUpDate = c.FollowUpDate,
                    ConsultationFee = c.ConsultationFee,
                    IsPaid = c.IsPaid,
                    Notes = c.Notes,
                    CreatedAt = c.CreatedAt,
                    PatientName = c.Appointment.Patient.FullName,
                    DoctorName = c.Appointment.Doctor.FullName,
                    AppointmentDate = c.Appointment.AppointmentDate
                })
                .FirstOrDefaultAsync();

            return consultation;
        }

        public async Task<IEnumerable<ConsultationResponse>> GetConsultationsByPatientIdAsync(int patientId)
        {
            return await _context.Consultations
                .Include(c => c.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(c => c.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Where(c => c.Appointment.PatientId == patientId && !c.IsDeleted)
                .OrderByDescending(c => c.ConsultationDate)
                .Select(c => new ConsultationResponse
                {
                    Id = c.Id,
                    AppointmentId = c.AppointmentId,
                    ConsultationDate = c.ConsultationDate,
                    Symptoms = c.Symptoms,
                    Diagnosis = c.Diagnosis,
                    Prescription = c.Prescription,
                    TreatmentPlan = c.TreatmentPlan,
                    FollowUpInstructions = c.FollowUpInstructions,
                    FollowUpDate = c.FollowUpDate,
                    ConsultationFee = c.ConsultationFee,
                    IsPaid = c.IsPaid,
                    Notes = c.Notes,
                    CreatedAt = c.CreatedAt,
                    PatientName = c.Appointment.Patient.FullName,
                    DoctorName = c.Appointment.Doctor.FullName,
                    AppointmentDate = c.Appointment.AppointmentDate
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ConsultationResponse>> GetConsultationsByDoctorIdAsync(int doctorId)
        {
            return await _context.Consultations
                .Include(c => c.Appointment)
                    .ThenInclude(a => a.Patient)
                .Include(c => c.Appointment)
                    .ThenInclude(a => a.Doctor)
                .Where(c => c.Appointment.DoctorId == doctorId && !c.IsDeleted)
                .OrderByDescending(c => c.ConsultationDate)
                .Select(c => new ConsultationResponse
                {
                    Id = c.Id,
                    AppointmentId = c.AppointmentId,
                    ConsultationDate = c.ConsultationDate,
                    Symptoms = c.Symptoms,
                    Diagnosis = c.Diagnosis,
                    Prescription = c.Prescription,
                    TreatmentPlan = c.TreatmentPlan,
                    FollowUpInstructions = c.FollowUpInstructions,
                    FollowUpDate = c.FollowUpDate,
                    ConsultationFee = c.ConsultationFee,
                    IsPaid = c.IsPaid,
                    Notes = c.Notes,
                    CreatedAt = c.CreatedAt,
                    PatientName = c.Appointment.Patient.FullName,
                    DoctorName = c.Appointment.Doctor.FullName,
                    AppointmentDate = c.Appointment.AppointmentDate
                })
                .ToListAsync();
        }

        public async Task<ConsultationResponse> UpdateConsultationAsync(int id, ConsultationRequest request, string userId)
        {
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null || consultation.IsDeleted)
            {
                throw new InvalidOperationException("Consultation not found.");
            }

            consultation.Symptoms = request.Symptoms;
            consultation.Diagnosis = request.Diagnosis;
            consultation.Prescription = request.Prescription;
            consultation.TreatmentPlan = request.TreatmentPlan;
            consultation.FollowUpInstructions = request.FollowUpInstructions;
            consultation.FollowUpDate = request.FollowUpDate;
            consultation.ConsultationFee = request.ConsultationFee;
            consultation.Notes = request.Notes;
            consultation.UpdatedAt = DateTime.UtcNow;
            consultation.UpdatedBy = userId;

            await _context.SaveChangesAsync();

            return await GetConsultationByIdAsync(consultation.Id);
        }

        public async Task<bool> MarkConsultationAsPaidAsync(int id, string userId)
        {
            var consultation = await _context.Consultations.FindAsync(id);
            if (consultation == null || consultation.IsDeleted)
            {
                return false;
            }

            consultation.IsPaid = true;
            consultation.UpdatedAt = DateTime.UtcNow;
            consultation.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
