using System.Text;
using DotNetEnv;
using HMS.OperationalService.Api.Middleware;
using HMS.OperationalService.Common.Security;
using HMS.OperationalService.Domain;
using HMS.OperationalService.Repository;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service;
using HMS.OperationalService.Service.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddOpenApi();

builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IHouseKeepingRepository, HouseKeepingRepository>();
builder.Services.AddScoped<IMaintainanceRepository, MaintainanceRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
builder.Services.AddScoped<IRoomInspectionRepository, RoomInspectionRepository>();
builder.Services.AddScoped<ILostAndFoundRepository, LostAndFoundRepository>();
builder.Services.AddScoped<IStaffTaskAssignmentRepository, StaffTaskAssignmentRepository>();

builder.Services.AddScoped<IHouseKeepingService, HouseKeepingService>();
builder.Services.AddScoped<IMaintainanceService, MaintainanceService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IRoomInspectionService, RoomInspectionService>();
builder.Services.AddScoped<ILostAndFoundService, LostAndFoundService>();
builder.Services.AddScoped<IStaffTaskAssignmentService, StaffTaskAssignmentService>();

var connectionString = builder.Configuration["DB_CONNECTION_STRING"]
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Database connection string is missing. Set DB_CONNECTION_STRING in .env.");

builder.Services.AddDbContext<OperationalDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory_Operations")));

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSecretKey = builder.Configuration["JWT_SECRET_KEY"]
    ?? jwtSection["SecretKey"]
    ?? throw new InvalidOperationException("JWT_SECRET_KEY is missing. Set it in .env.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped<IAccessGuard, AccessGuard>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
