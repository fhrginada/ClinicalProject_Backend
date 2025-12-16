using ClinicalProject_API.Data;
using Microsoft.EntityFrameworkCore;
using ClinicalProject_API.Services;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Add DbContext
builder.Services.AddDbContext<ClinicalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 2️⃣ Add Services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IConsultationService, ConsultationService>();

// 3️⃣ Add Controllers
builder.Services.AddControllers();

// 4️⃣ Swagger
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



