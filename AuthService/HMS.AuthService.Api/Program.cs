using System.Text;
using DotNetEnv;
using HMS.AuthService.Api.Configurations;
using HMS.AuthService.Api.Middleware;
using HMS.AuthService.Domain;
using HMS.AuthService.Domain.Data;
using HMS.AuthService.Repository;
using HMS.AuthService.Repository.Contracts;
using HMS.AuthService.Service;
using HMS.AuthService.Service.Contracts;
using HMS.AuthService.Service.External;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Load local .env variables into Environment
Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Mapster Configuration
var config = TypeAdapterConfig.GlobalSettings;
MapsterConfig.RegisterMappings(config);
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

var connectionString = builder.Configuration["DB_CONNECTION_STRING"]
                    ?? builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("AuthServiceDbContext connection string is missing.");

builder.Services.AddDbContext<AuthServiceDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory_Identity")));

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ASP.NET Core Identity Configuration
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AuthServiceDbContext>()
.AddDefaultTokenProviders();

// JWT Secrets & Configurations
var secretKey = builder.Configuration["JWT_SECRET_KEY"]
             ?? builder.Configuration["Jwt:Key"]
             ?? builder.Configuration["Jwt:Secret"]
             ?? throw new InvalidOperationException("JWT Secret key is missing from configuration.");

// Authentication Pipeline Configuration
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

        NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
        RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
    };
});

// Generic Repository Registrations
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// AuthService registrations
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddHttpContextAccessor();

var staffServiceBaseUrl = builder.Configuration["StaffService:BaseUrl"]
    ?? throw new InvalidOperationException("StaffService:BaseUrl is missing from appsettings.json.");
var internalApiKey = builder.Configuration["INTERNAL_API_KEY"]
    ?? throw new InvalidOperationException("INTERNAL_API_KEY is missing. Set it in .env — it must match StaffService's own INTERNAL_API_KEY.");

builder.Services.AddHttpClient<IStaffServiceClient, StaffServiceClient>(client =>
{
    client.BaseAddress = new Uri(staffServiceBaseUrl.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Add("X-Internal-Api-Key", internalApiKey);
    client.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Global Exception Handling Middleware
app.UseMiddleware<ExceptionMiddleware>();

// Configure HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowReact");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();