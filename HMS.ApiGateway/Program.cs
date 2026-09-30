var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


//CORS
builder.Services.AddCors(o => o.AddDefaultPolicy(p => p.WithOrigins("http://localhost:5173")
.AllowAnyHeader()
.AllowAnyMethod()));

var app = builder.Build();


app.UseCors();

app.MapReverseProxy();

app.Run();
