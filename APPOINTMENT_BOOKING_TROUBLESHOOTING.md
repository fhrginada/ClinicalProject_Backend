# Appointment Booking Troubleshooting Guide

## Problem: "Failed to book appointment. Please try again."

This error can occur for several reasons. The backend has been updated to provide more specific error messages.

---

## ✅ Recent Improvements

### Better Error Messages
The backend now returns specific error messages instead of a generic "Failed to book appointment":

1. **"Patient profile not found. Please complete your profile registration."**
   - **Cause**: User registered but patient profile wasn't created
   - **Solution**: Ensure patient profile exists in database

2. **"Doctor not found."**
   - **Cause**: Invalid doctor ID
   - **Solution**: Verify doctor exists and ID is correct

3. **"Time slot not found."**
   - **Cause**: Invalid time slot ID
   - **Solution**: Verify time slot exists

4. **"This time slot is no longer available. Please select another time."**
   - **Cause**: Time slot was already booked by another patient
   - **Solution**: Refresh available slots and select a different time

5. **"Time slot does not belong to the selected doctor."**
   - **Cause**: Time slot belongs to a different doctor
   - **Solution**: Ensure doctor ID matches the time slot's doctor

6. **"Cannot book appointments in the past. Please select a future time slot."**
   - **Cause**: Trying to book a time slot that has already passed
   - **Solution**: Select a future date/time

---

## 🔍 Common Causes & Solutions

### 1. Patient Profile Missing

**Symptom**: Error about patient profile not found

**Check**:
```sql
SELECT * FROM Patients WHERE UserId = {your_user_id}
```

**Solution**: 
- If missing, the patient profile should be created during registration
- Check registration logs for errors
- Manually create patient profile if needed

### 2. Time Slot Already Booked

**Symptom**: "Time slot is no longer available"

**Check**:
```sql
SELECT * FROM TimeSlots WHERE TimeSlotId = {time_slot_id}
-- Status should be "Available"
```

**Solution**:
- Refresh available slots before booking
- Implement real-time slot updates
- Add slot locking mechanism

### 3. Invalid Doctor/Time Slot Relationship

**Symptom**: "Time slot does not belong to the selected doctor"

**Check**:
```sql
SELECT ts.*, ds.DoctorId 
FROM TimeSlots ts
JOIN DoctorSchedules ds ON ts.ScheduleId = ds.ScheduleId
WHERE ts.TimeSlotId = {time_slot_id}
```

**Solution**:
- Ensure you're using the correct doctor ID
- Verify time slot belongs to that doctor

### 4. Past Time Slots

**Symptom**: "Cannot book appointments in the past"

**Check**:
```sql
SELECT SlotDate, StartTime 
FROM TimeSlots 
WHERE TimeSlotId = {time_slot_id}
```

**Solution**:
- Filter out past time slots in frontend
- Only show future available slots

---

## 🧪 Testing Steps

### 1. Verify Patient Profile Exists

```powershell
# Check if patient profile exists for logged-in user
# In Swagger: GET /api/patients/profile
```

### 2. Get Available Slots

```powershell
# Get available slots for a doctor
# GET /api/appointments/available-slots?doctorId=1&startDate=2024-01-01&endDate=2024-01-31
```

### 3. Book Appointment

```json
POST /api/appointments/book
{
  "doctorId": 1,
  "timeSlotId": 123,
  "reasonForVisit": "Regular checkup"
}
```

### 4. Check Error Response

The response will now include a specific error message:
```json
{
  "message": "This time slot is no longer available. Please select another time."
}
```

---

## 🔧 Frontend Implementation

### Update Error Handling

```typescript
try {
  const appointment = await appointmentService.bookAppointment({
    doctorId: selectedDoctorId,
    timeSlotId: selectedTimeSlotId,
    reasonForVisit: reason
  });
  
  Alert.alert('Success', 'Appointment booked successfully!');
} catch (error: any) {
  // Now you'll get specific error messages
  Alert.alert('Booking Failed', error.message || 'Failed to book appointment');
}
```

### Refresh Slots Before Booking

```typescript
// Always refresh available slots before showing booking form
const slots = await appointmentService.getAvailableSlots(doctorId);
// Filter out past slots
const futureSlots = slots.filter(slot => {
  const slotDate = new Date(slot.slotDate);
  return slotDate > new Date();
});
```

---

## 📋 Validation Checklist

Before booking, ensure:

- [ ] User is logged in as Patient role
- [ ] Patient profile exists in database
- [ ] Doctor ID is valid and exists
- [ ] Time slot ID is valid and exists
- [ ] Time slot status is "Available"
- [ ] Time slot belongs to the selected doctor
- [ ] Time slot date/time is in the future
- [ ] JWT token is valid and included in request

---

## 🐛 Debugging

### Enable Detailed Logging

Check backend logs for:
```
Appointment booking failed: {specific error message}
```

### Database Queries

```sql
-- Check patient
SELECT * FROM Patients WHERE UserId = {userId};

-- Check time slot
SELECT * FROM TimeSlots WHERE TimeSlotId = {timeSlotId};

-- Check doctor
SELECT * FROM Doctors WHERE DoctorId = {doctorId};

-- Check appointments
SELECT * FROM Appointments WHERE PatientId = {patientId};
```

---

## ✅ Expected Behavior After Fix

1. **Specific Error Messages**: You'll now see exactly why booking failed
2. **Better Validation**: All relationships are verified before booking
3. **Past Slot Prevention**: Cannot book slots in the past
4. **Doctor-Slot Verification**: Ensures slot belongs to selected doctor

---

## 🚀 Next Steps

1. **Test with specific error messages** - You'll now know exactly what's wrong
2. **Update frontend** - Handle specific error messages appropriately
3. **Add slot refresh** - Refresh available slots before booking
4. **Add loading states** - Show user that booking is in progress

The booking should now work correctly, and if it fails, you'll get a clear error message explaining why!

