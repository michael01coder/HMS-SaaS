using System.Text;
using DotNetEnv;
using HMS.AttendanceService.Api.Configurations;
using HMS.AttendanceService.Api.Middleware;
using HMS.AttendanceService.Common.Service;
using HMS.AttendanceService.Domain;
using HMS.AttendanceService.Repository;
using HMS.AttendanceService.Repository.Contracts;
using HMS.AttendanceService.Service;
using HMS.AttendanceService.Service.Contracts;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

Env.Load();
builder.Configuration.AddEnvironmentVariables();

var config = TypeAdapterConfig.GlobalSettings;
MapsterConfig.RegisterMappings(config);
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper>(sp => new Mapper(config));

var connectionString = builder.Configuration["DB_CONNECTION_STRING"]
                    ?? builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("AttendanceDbContext connection string is missing.");

builder.Services.AddDbContext<AttendanceDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory_Attendance"));
    options.EnableSensitiveDataLogging();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var secretKey = builder.Configuration["JWT_SECRET_KEY"]
             ?? builder.Configuration["Jwt:Key"]
             ?? builder.Configuration["Jwt:Secret"]
             ?? throw new InvalidOperationException("JWT Secret key is missing.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IAttendanceAnomalyRepository, AttendanceAnomalyRepository>();

builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IAttendanceAnomalyService, AttendanceAnomalyService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowReact");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();