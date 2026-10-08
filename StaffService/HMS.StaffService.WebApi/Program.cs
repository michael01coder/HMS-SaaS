using System.Text;
using DotNetEnv;
using HMS.StaffService.Common.Service;
using HMS.StaffService.Domain;
using HMS.StaffService.Repository;
using HMS.StaffService.Repository.Contracts;
using HMS.StaffService.Service;
using HMS.StaffService.Service.Contracts;
using HMS.StaffService.WebApi.Configurations;
using HMS.StaffService.WebApi.Middleware;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Load local .env variables into Environment
Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Mapster Registration
var config = TypeAdapterConfig.GlobalSettings;
MapsterConfig.RegisterMappings(config);
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper>(sp => new Mapper(config));

var connectionString = builder.Configuration["DB_CONNECTION_STRING"]
                    ?? builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("StaffDbContext connection string is missing.");

builder.Services.AddDbContext<StaffDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory_Staff"));
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

// Current User Service & HttpContext Accessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// JWT Authentication Setup
var secretKey = builder.Configuration["JWT_SECRET_KEY"]
             ?? builder.Configuration["Jwt:Key"]
             ?? builder.Configuration["Jwt:Secret"]
             ?? throw new InvalidOperationException("JWT Secret key is missing in StaffService.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
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
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey)
        ),

        ClockSkew = TimeSpan.Zero
    };
});

// Repositories 
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffDocumentRepository, StaffDocumentRepository>();
builder.Services.AddScoped<IStaffNotesRepository, StaffNotesRepository>();

// Services
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IStaffDocumentService, StaffDocumentService>();
builder.Services.AddScoped<IStaffNotesService, StaffNotesService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Was defined but never registered — every NotFoundException/
// ForbiddenException thrown in the Service layer was propagating as a raw
// unhandled exception instead of the intended JSON error response.
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