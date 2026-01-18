using Clean_Connect.Application.AssemblyMarker;
using Clean_Connect.Application.Behaviours;
using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Infrastructure.Context;
using Clean_Connect.Persistence.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------
// Serilog configuration
// --------------------
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// --------------------
// JWT settings
// --------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");

// --------------------
// Add DbContext first
// --------------------
builder.Services.AddSqlServer<ApplicationDbContext>(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.MigrationsAssembly("Clean-Connect.Persistence"));

// --------------------
// Identity Core for API
// --------------------
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// --------------------
// Authentication - JWT
// --------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
    };
});

// --------------------
// Authorization
// --------------------
builder.Services.AddAuthorization();

// --------------------
// Controllers & Swagger
// --------------------
builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
         options.JsonSerializerOptions.Converters.Add(
             new System.Text.Json.Serialization.JsonStringEnumConverter()
         );
     });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------
// MediatR
// --------------------
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));

// --------------------
// DI for repositories & UnitOfWork
// --------------------
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// --------------------
// FluentValidation
// --------------------
builder.Services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);

// --------------------
// MediatR Pipeline Behaviors
// --------------------
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

var app = builder.Build();

// --------------------
// Middleware
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ? MUST come BEFORE UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

// --------------------
// Map controllers
// --------------------
app.MapControllers();

// Optional: redirect root to Swagger
app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.Run();
