namespace CLINICSYSTEM.Enums;

/// <summary>
/// Represents user roles in the clinic system
/// The system supports three roles: Doctor, Patient, and Nurse
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Patient user who can book appointments and view their records
    /// </summary>
    Patient = 0,

    /// <summary>
    /// Doctor who can manage appointments and consultations
    /// </summary>
    Doctor = 1,

    /// <summary>
    /// Nurse who can make appointments, track doctor schedules, and receive tasks from doctors
    /// </summary>
    Nurse = 2
}
