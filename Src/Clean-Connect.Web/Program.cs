using AspNetCoreHero.ToastNotification;
using Clean_Connect.Application.AssemblyMarker;
using Clean_Connect.Application.Behaviours;
using Clean_Connect.Application.Command.Services;
using Clean_Connect.Application.Interface.Repositories;
using Clean_Connect.Application.Interface.Services;
using Clean_Connect.Domain.Entities;
using Clean_Connect.Infrastructure.Configuration;
using Clean_Connect.Infrastructure.Context;
using Clean_Connect.Persistence.Repositories;
using Clean_Connect.Web.Hubs;
using Clean_Connect.Web.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// --------------------
// JWT settings
// --------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");

// MediatR
// --------------------
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyMarker).Assembly));

builder.Services.AddSqlServer<ApplicationDbContext>(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    b => b.UseNetTopologySuite()
   .MigrationsAssembly("Clean-Connect.Persistence"));




// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();


// --------------------
// DI for Services
// --------------------
builder.Services.AddHttpClient<GeocodingService>();
builder.Services.AddScoped<WorkerAvailabilityService>();
builder.Services.AddScoped<BookingRuleService>();
builder.Services.AddScoped<AcceptBookingService>();
builder.Services.AddScoped<MarkAsCompletedService>();
builder.Services.AddScoped<EscrowService>();
builder.Services.AddScoped<PayoutService>();
builder.Services.AddScoped<WalletService>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddHttpClient<IPaystackService, PaystackService>();



builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
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

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();



// --------------------
// DI for repositories & UnitOfWork
// --------------------
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();
builder.Services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IEscrowRepository, EscrowRepository>();
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddSingleton<IExperienceDataService, ExperienceDataService>();
builder.Services.AddSingleton<IPortalExperienceService, PortalExperienceService>();
builder.Services.AddSingleton<IDashboardFeed, DashboardFeed>();
builder.Services.AddHostedService<DashboardRealtimeService>();

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehaviour<,>));

// NOTYF (works only in MVC)
builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<AdminDashboardHub>("/hubs/admin-dashboard");

app.Run();
