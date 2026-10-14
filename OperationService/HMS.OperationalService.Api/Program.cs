using DotNetEnv;
using HMS.OperationalService.Api.Middleware;
using HMS.OperationalService.Data;
using HMS.OperationalService.Domain;
using HMS.OperationalService.Repository;
using HMS.OperationalService.Repository.Contracts;
using HMS.OperationalService.Service;
using HMS.OperationalService.Service.Contracts;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Load shared .env (JWT_SECRET_KEY, DB_CONNECTION_STRING) — see appsettings.json for non-secret Jwt:Issuer/Audience
Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Mapster
var config = TypeAdapterConfig.GlobalSettings;
HMS.OperationalService.Api.Configurations.MapsterConfig.RegisterMappings(config);
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper>(sp => new Mapper(config));
builder.Services.AddControllers();
builder.Services.AddOpenApi();
 //repo
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IHouseKeepingRepository, HouseKeepingRepository>();
builder.Services.AddScoped<IMaintainanceRepository, MaintainanceRepository>();
builder.Services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();
builder.Services.AddScoped<IRoomInspectionRepository, RoomInspectionRepository>();
builder.Services.AddScoped<ILostAndFoundRepository, LostAndFoundRepository>();
builder.Services.AddScoped<IStaffTaskAssignmentRepository, StaffTaskAssignmentRepository>();


//service
builder.Services.AddScoped<IHouseKeepingService, HouseKeepingService>();
builder.Services.AddScoped<IMaintainanceService, MaintainanceService>();
builder.Services.AddScoped<IServiceRequestService, ServiceRequestService>();
builder.Services.AddScoped<IRoomInspectionService, RoomInspectionService>();
builder.Services.AddScoped<ILostAndFoundService, LostAndFoundService>();
builder.Services.AddScoped<IStaffTaskAssignmentService, StaffTaskAssignmentService>();

// Register PostgreSQL DbContext
var connectionString = builder.Configuration["DB_CONNECTION_STRING"]
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("HotelServiceDbContext connection string is missing. Set DB_CONNECTION_STRING in .env.");

builder.Services.AddDbContext<OperationalDbContext>(options =>
    options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory_Hotel")));

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

      options.Events = new JwtBearerEvents
      {
          OnAuthenticationFailed = ctx =>
          {
              Console.WriteLine("JWT Authentication failed:");
              Console.WriteLine(ctx.Exception.ToString());
              return Task.CompletedTask;
          }
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
