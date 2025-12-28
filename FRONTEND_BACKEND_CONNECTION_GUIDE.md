# Complete Step-by-Step Guide: Connect React Native Frontend to ASP.NET Core Backend

## ✅ Prerequisites Checklist

Before starting, ensure you have:
- [ ] Backend running successfully (test at `http://localhost:5056/swagger`)
- [ ] React Native/Expo project open in VS Code
- [ ] Node.js and npm installed
- [ ] Both terminals ready (one for backend, one for frontend)

---

## 📋 STEP 1: Install Required Dependencies

**Open Terminal in VS Code (frontend folder)**

```bash
cd frontend
npm install @react-native-async-storage/async-storage
```

**Verify axios is installed:**
```bash
npm list axios
```
If not installed:
```bash
npm install axios
```

---

## 📋 STEP 2: Find Your Computer's IP Address

**⚠️ CRITICAL: React Native cannot use `localhost`. You MUST use your IP address.**

### On Windows:
1. Open PowerShell or CMD
2. Run:
   ```powershell
   ipconfig
   ```
3. Find **"IPv4 Address"** under your active network adapter
   - Example: `192.168.1.100`
   - **Write this down - you'll need it!**

### On Mac/Linux:
```bash
ifconfig | grep "inet "
```

---

## 📋 STEP 3: Verify Backend is Running

1. **Start the backend** (if not already running):
   ```powershell
   cd backend
   dotnet run
   ```

2. **Test the backend:**
   - Open browser: `http://localhost:5056/swagger`
   - You should see the Swagger UI
   - If it works, backend is ready ✅

3. **Note the backend URL:**
   - HTTP: `http://localhost:5056`
   - Your IP version: `http://YOUR_IP:5056` (e.g., `http://192.168.1.100:5056`)

---

## 📋 STEP 4: Create API Configuration File

**Create:** `frontend/constants/api.ts`

This file will be created automatically in the next steps. It contains:
- Base URL configuration
- All API endpoints
- Helper functions

**Important:** Replace `YOUR_IP_ADDRESS` with your actual IP from Step 2.

---

## 📋 STEP 5: Create API Client Service

**Create:** `frontend/services/apiClient.ts`

This handles:
- HTTP requests with axios
- Automatic JWT token injection
- Error handling
- Request/response interceptors

---

## 📋 STEP 6: Create TypeScript Types

**Create:** `frontend/types/api.ts`

Type definitions matching your backend DTOs for type safety.

---

## 📋 STEP 7: Create Service Files

**Create:**
- `frontend/services/authService.ts` - Authentication API calls
- `frontend/services/appointmentService.ts` - Appointments API calls

---

## 📋 STEP 8: Update AuthContext

**Update:** `frontend/context/AuthContext.tsx`

Replace mock login with real API integration:
- Real API calls
- Token storage
- User data persistence

---

## 📋 STEP 9: Test the Connection

### 9.1 Start Backend
```powershell
cd backend
dotnet run
```
Wait for: `Now listening on: http://localhost:5056`

### 9.2 Start Frontend
```bash
cd frontend
npm start
```

### 9.3 Test Login
1. Open your app (Expo Go or emulator)
2. Enter credentials that exist in your database
3. Check for errors in console
4. Should successfully login and navigate

---

## 📋 STEP 10: Verify Everything Works

### Test Checklist:
- [ ] Backend is running (`http://localhost:5056/swagger` works)
- [ ] Frontend starts without errors
- [ ] Login screen loads
- [ ] Can register new user
- [ ] Can login with existing user
- [ ] Token is stored correctly
- [ ] Can access protected endpoints

---

## 🔧 Troubleshooting Common Issues

### Issue 1: "Network request failed"
**Solution:**
- ✅ Verify you're using IP address, not `localhost`
- ✅ Check backend is running
- ✅ Check firewall isn't blocking port 5056
- ✅ Verify IP address is correct

### Issue 2: "CORS error"
**Solution:**
- ✅ Backend CORS is already configured
- ✅ Ensure CORS middleware is before Authorization in `Program.cs`
- ✅ Check browser console for specific CORS error

### Issue 3: "401 Unauthorized"
**Solution:**
- ✅ Verify token is being sent in requests
- ✅ Check token format: `Bearer {token}`
- ✅ Verify token hasn't expired
- ✅ Check user role matches endpoint requirements

### Issue 4: "Cannot connect to backend"
**Solution:**
- ✅ Verify backend is running
- ✅ Check IP address is correct
- ✅ Try accessing `http://YOUR_IP:5056/swagger` in browser
- ✅ Check both devices are on same network

---

## 📝 File Structure After Setup

```
frontend/
├── constants/
│   └── api.ts                    ← API configuration
├── services/
│   ├── apiClient.ts              ← HTTP client
│   ├── authService.ts            ← Auth API calls
│   └── appointmentService.ts     ← Appointments API calls
├── types/
│   └── api.ts                    ← TypeScript types
└── context/
    └── AuthContext.tsx            ← Updated with real API
```

---

## 🎯 Next Steps After Connection

1. **Test all endpoints** using the services
2. **Add error handling** in your components
3. **Implement loading states** for better UX
4. **Add token refresh** if needed
5. **Test on physical device** (not just emulator)

---

## ⚠️ Important Notes

1. **Development vs Production:**
   - Development: Use your IP address
   - Production: Use your production API URL

2. **Security:**
   - Never commit tokens to git
   - Use environment variables for API URLs in production
   - Store tokens securely (AsyncStorage for dev, Keychain for prod)

3. **Testing:**
   - Test on same network (WiFi)
   - Physical device testing recommended
   - Check both iOS and Android if targeting both

---

## ✅ Success Indicators

You'll know it's working when:
- ✅ Login succeeds and you get a JWT token
- ✅ You can access protected endpoints
- ✅ User data persists after app restart
- ✅ No console errors related to API calls
- ✅ Network requests show in React Native debugger

---

**Ready to proceed? The files will be created in the next steps. Follow each step carefully!**

