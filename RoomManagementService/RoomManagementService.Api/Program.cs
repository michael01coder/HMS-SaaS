using HMSRoomManagement.Common.Service;
using HMSRoomManagement.Domain;
using HMSRoomManagement.Repository;
using HMSRoomManagement.Repository.Contracts;
using HMSRoomManagement.Service;
using HMSRoomManagement.Service.Contracts;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RoomManagementService.Api.Configurations;
using RoomManagementService.Api.Middleware;
using System.Text;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Load shared .env (JWT_SECRET_KEY, DB_CONNECTION_STRING) — see appsettings.json for non-secret Jwt:Issuer/Audience
Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Mapster
var config = TypeAdapterConfig.GlobalSettings;
MapsterConfig.RegisterMappings(config);
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper>(sp => new Mapper(config));

// Current User Service & HttpContext Accessor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Repositories
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IRoomTypeRepository, RoomTypeRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRatePlanRepository, RatePlanRepository>();
builder.Services.AddScoped<ICancellationPolicyRepository, CancellationPolicyRepository>();
builder.Services.AddScoped<IRoomTypeAmenityRepository, RoomTypeAmenityRepository>();

// Services
builder.Services.AddScoped<IRoomTypeService, RoomTypeService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRatePlanService, RatePlanService>();
builder.Services.AddScoped<IRoomTypeAmenityService, RoomTypeAmenityService>();

// Register PostgreSQL DbContext
var connectionString = builder.Configuration["DB_CONNECTION_STRING"]
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("HMSRoomManagementDbContext connection string is missing. Set DB_CONNECTION_STRING in .env.");

builder.Services.AddDbContext<HMSRoomManagementDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory_Room")));

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtSecretKey = builder.Configuration["JWT_SECRET_KEY"]
    ?? jwtSection["SecretKey"]
    ?? throw new InvalidOperationException("JWT_SECRET_KEY is missing. Set it in .env.");
var signingKey = Encoding.UTF8.GetBytes(jwtSecretKey);

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
          IssuerSigningKey = new SymmetricSecurityKey(signingKey),
          ValidateLifetime = true,
          ClockSkew = TimeSpan.Zero,
      };
  });

builder.Services.AddAuthorization();

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
