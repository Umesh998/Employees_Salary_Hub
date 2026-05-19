//using Employees_Salary_Hub.Data;
//using Employees_Salary_Hub.Helpers;
//using Employees_Salary_Hub.Models;
//using Employees_Salary_Hub.Repositories;
//using Employees_Salary_Hub.Repositories.Interfaces;
//using Employees_Salary_Hub.Service.Interfaces;
//using Employees_Salary_Hub.Services;
//using Employees_Salary_Hub.Services.Interfaces;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// ── Database ──────────────────────────────────────────────────────
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// ── Identity ──────────────────────────────────────────────────────
//builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
//{
//    options.Password.RequireDigit = true;
//    options.Password.RequiredLength = 8;
//    options.Password.RequireUppercase = true;
//    options.Password.RequireNonAlphanumeric = true;
//    options.Lockout.MaxFailedAccessAttempts = 5;
//    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
//    options.User.RequireUniqueEmail = false;
//})
//.AddEntityFrameworkStores<ApplicationDbContext>()
//.AddDefaultTokenProviders();

//// ── Custom Password Validator ─────────────────────────────────────
//builder.Services.AddScoped<IPasswordValidator<ApplicationUser>,
//    CustomPasswordValidator<ApplicationUser>>();

//// ── JWT ───────────────────────────────────────────────────────────
//var jwtKey = builder.Configuration["Jwt:Key"]!;
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
//        ValidateIssuer = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidateAudience = true,
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        ClockSkew = TimeSpan.Zero
//    };
//});

//// ── Authorization Policies ────────────────────────────────────────
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//    options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
//});

//// ── Application Services ──────────────────────────────────────────
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<IOtpService, OtpService>();
//builder.Services.AddScoped<IExcelService, ExcelService>();
//builder.Services.AddScoped<IPdfService, PdfService>();
//builder.Services.AddScoped<ISalarySlipService, SalarySlipService>();
//builder.Services.AddScoped<ISmsService, SmsService>();
//builder.Services.AddScoped<IAuditService, AuditService>();
//builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

//builder.Services.AddControllersWithViews();
//builder.Services.AddAntiforgery();

//var app = builder.Build();

//// ── Middleware Pipeline ───────────────────────────────────────────
//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Auth}/{action=Login}/{id?}");

//app.Run();










using Employees_Salary_Hub.Data;
using Employees_Salary_Hub.Helpers;
using Employees_Salary_Hub.Models;
using Employees_Salary_Hub.Repositories;
using Employees_Salary_Hub.Repositories.Interfaces;
using Employees_Salary_Hub.Service.Interfaces;
using Employees_Salary_Hub.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Identity ──────────────────────────────────────────────────────
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ── Custom Password Validator ─────────────────────────────────────
builder.Services.AddScoped<IPasswordValidator<ApplicationUser>,
    CustomPasswordValidator<ApplicationUser>>();

// ── JWT ───────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT Key is not configured. Add 'Jwt:Key' to appsettings.json.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})


.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero
    };

    // ✅ Add this — read JWT from cookie instead of Authorization header
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["jwt"];
            return Task.CompletedTask;
        }
    };
});
// ── Authorization Policies ────────────────────────────────────────
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
});

// ── Application Services ──────────────────────────────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IEmailService, EmailService>();   // ← replaces SmsService
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<ISalarySlipService, SalarySlipService>();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery();

var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────────────────────
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

// ── Seed Database ─────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    await DbSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
