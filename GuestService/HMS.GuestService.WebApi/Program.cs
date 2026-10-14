using HMS.GuestService.Repository;
using HMS.GuestService.Repository.Contracts;
using HMS.GuestService.Service.Contracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Register open generic repository with two type parameters (TEntity, TId)
builder.Services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

// Register specific repositories
builder.Services.AddScoped<IGuestRepository, GuestRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IGuestRepository, GuestRepository>();


// Register services
builder.Services.AddScoped<IGuestService, IGuestService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
