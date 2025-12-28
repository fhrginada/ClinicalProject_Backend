# Troubleshooting: Registration 401 Error

## Problem
When registering from the frontend, you see a 401 Unauthorized error on `/api/Auth/profile` in Swagger, and no action is taken.

## Root Cause
The `/api/Auth/profile` endpoint requires authentication (`[Authorize]` attribute). The 401 error appears because:
1. The profile endpoint is protected and requires a JWT token
2. If something tries to access it without a token, it returns 401
3. This might be happening if the app tries to fetch profile after registration

## Solution Applied

### ✅ Fixed Signup.tsx
- **Before**: Used mock `setTimeout` - didn't actually call the API
- **After**: Now uses real `register()` function from `AuthContext`
- Added proper error handling
- Added phone number and role selection fields

### ✅ How It Works Now

1. **User fills form** → Full Name, Email, Password, Phone, Role
2. **Clicks Sign Up** → Calls `register()` from AuthContext
3. **AuthContext** → Calls `authService.register()`
4. **authService** → Makes POST request to `/api/auth/register`
5. **Backend** → Returns `AuthResponse` with token and user data
6. **Frontend** → Stores token in AsyncStorage and sets authentication state
7. **Success** → User is logged in and navigated to dashboard

## Testing Steps

1. **Start Backend:**
   ```powershell
   cd backend
   dotnet run
   ```

2. **Start Frontend:**
   ```bash
   cd frontend
   npm start
   ```

3. **Test Registration:**
   - Go to Sign Up page
   - Fill in all fields:
     - Full Name: "John Doe"
     - Email: "test@example.com"
     - Phone: "1234567890"
     - Password: "password123"
     - Confirm Password: "password123"
     - Select Role: Patient/Doctor/Nurse
   - Click "Sign Up"
   - Should see success message and navigate to dashboard

## Common Issues

### Issue 1: "Network request failed"
**Solution:**
- Check IP address in `frontend/constants/api.ts` matches your computer's IP
- Verify backend is running on port 5056
- Check both devices are on same WiFi network

### Issue 2: "Registration failed" or "Email already exists"
**Solution:**
- Try a different email address
- Check backend logs for specific error message
- Verify all required fields are filled

### Issue 3: Still seeing 401 on profile endpoint
**Solution:**
- This is normal if you're testing in Swagger without a token
- The profile endpoint requires authentication
- After successful registration/login, the token is stored and will be sent automatically

### Issue 4: Token not being stored
**Solution:**
- Check AsyncStorage is installed: `npm list @react-native-async-storage/async-storage`
- Check browser/device console for errors
- Verify token is in response: `response.success && response.token`

## Verification

After successful registration, you should:
- ✅ See success alert
- ✅ Be automatically logged in
- ✅ Navigate to appropriate dashboard (Patient/Doctor)
- ✅ Token stored in AsyncStorage
- ✅ Can make authenticated API calls

## Debugging

Add console logs to see what's happening:

```typescript
// In AuthContext.tsx register function
console.log('Registration response:', response);
console.log('Token:', response.token);
console.log('User:', response.user);
```

Check React Native debugger or browser console for these logs.

