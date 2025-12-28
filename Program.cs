using ClinicalProject_API.Data;
using Microsoft.EntityFrameworkCore;
using ClinicalProject_API.Services;
using ClinicalProject_API.Services.Interfaces;
using ClinicalProject_API.Repositories.Interfaces;
using ClinicalProject_API.Repositories.Implementations;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Add DbContext
builder.Services.AddDbContext<ClinicalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 2️⃣ Add Services (existing)
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IConsultationService, ConsultationService>();

// 2️⃣ Add Services (Prescription System)
builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();

// 3️⃣ Add Repositories (Prescription System)
builder.Services.AddScoped<IPrescriptionRepository, PrescriptionRepository>();

// 4️⃣ Add Controllers
builder.Services.AddControllers();

// 5️⃣ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Set URLs properly
builder.WebHost.UseUrls("http://localhost:5187", "https://localhost:7249");

var app = builder.Build();

// Always enable Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clinical API V1");
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();



