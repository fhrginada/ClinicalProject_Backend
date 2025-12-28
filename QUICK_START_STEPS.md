# 🚀 Quick Start: Connect Frontend to Backend

## ⚡ 5-Minute Setup Guide

### Step 1: Find Your IP Address (CRITICAL!)

**Windows:**
```powershell
ipconfig
```
Look for **"IPv4 Address"** - Example: `192.168.1.100`

**Mac/Linux:**
```bash
ifconfig | grep "inet "
```

### Step 2: Update API Configuration

**Open:** `frontend/constants/api.ts`

**Find this line:**
```typescript
const YOUR_IP_ADDRESS = '192.168.1.100'; // ← REPLACE THIS!
```

**Replace with YOUR IP address from Step 1**

### Step 3: Start Backend

**Open Terminal 1 (Backend):**
```powershell
cd backend
dotnet run
```

**Wait for:** `Now listening on: http://localhost:5056`

**Verify:** Open `http://localhost:5056/swagger` in browser

### Step 4: Start Frontend

**Open Terminal 2 (Frontend):**
```bash
cd frontend
npm start
```

### Step 5: Test Login

1. Open your app (Expo Go or emulator)
2. Enter credentials that exist in your database
3. Click Login
4. Should successfully login! ✅

---

## ✅ Files Created

All necessary files have been created:

- ✅ `frontend/constants/api.ts` - API configuration
- ✅ `frontend/services/apiClient.ts` - HTTP client
- ✅ `frontend/services/authService.ts` - Auth API calls
- ✅ `frontend/services/appointmentService.ts` - Appointments API calls
- ✅ `frontend/types/api.ts` - TypeScript types
- ✅ `frontend/context/AuthContext.tsx` - Updated with real API

---

## 🔧 Troubleshooting

### "Network request failed"
- ✅ Check you're using IP address (not localhost)
- ✅ Verify backend is running
- ✅ Check firewall settings

### "401 Unauthorized"
- ✅ Verify credentials are correct
- ✅ Check token is being stored

### "Cannot connect"
- ✅ Both devices on same WiFi network
- ✅ Backend running on correct port (5056)
- ✅ IP address is correct

---

## 📝 Next Steps

1. Test all endpoints using the services
2. Add error handling in components
3. Implement loading states
4. Test on physical device

**Everything is ready! Just update the IP address and start both servers!**

