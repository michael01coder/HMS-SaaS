using DotNetEnv;
using HMS.BillingService.Domain;
using HMS.BillingService.Repository;
using HMS.BillingService.Repository.Contracts;
using HMS.BillingService.Service;
using HMS.BillingService.Service.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Load shared .env (JWT_SECRET_KEY, DB_CONNECTION_STRING) — see appsettings.json for non-secret Jwt:Issuer/Audience
Env.Load();
builder.Configuration.AddEnvironmentVariables();

// Add services to the container.

builder.Services.AddControllers();

// Register Repositories
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// Register Services
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Register PostgreSQL DbContext
var connectionString = builder.Configuration["DB_CONNECTION_STRING"]
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("HotelServiceDbContext connection string is missing. Set DB_CONNECTION_STRING in .env.");

builder.Services.AddDbContext<BillingDbContext>(options =>
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

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
