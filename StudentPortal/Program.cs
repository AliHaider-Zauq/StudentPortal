using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Data;
using StudentPortal.Models;
using StudentPortal.Repositories;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Register Student Repository
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

// Configure Database Connection for PostgreSQL
builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); // ✅ UseNpgsql for PostgreSQL

// ✅ Configure Identity with Role & User Management
builder.Services.AddIdentity<ApplicationUser, IdentityRole>() // Enables Role & User Manager
    .AddEntityFrameworkStores<StudentDbContext>()
    .AddDefaultTokenProviders();

// ✅ Register RoleManager & UserManager explicitly
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddScoped<UserManager<ApplicationUser>>(); // 🔥 FIX: Added UserManager registration
builder.Services.AddScoped<SignInManager<ApplicationUser>>(); // ✅ Also registering SignInManager

// Configure Authentication Redirects
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";  // Redirect unauthorized users to Login page
    options.AccessDeniedPath = "/Account/AccessDenied"; // If user is logged in but lacks permission
});

//POLICY BASE AUTHORIZATION
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanCreateRoles", policy => policy.RequireClaim("CreateRoles", "true"));
    options.AddPolicy("CanDeleteRoles", policy => policy.RequireClaim("DeleteRoles", "true"));
    options.AddPolicy("CanEditRoles", policy => policy.RequireClaim("EditRoles", "true"));
    options.AddPolicy("CanCreateStudent", policy => policy.RequireClaim("CreateStudent", "true"));

    //CUSTOMIZE POLICY
    options.AddPolicy("CanDeleteStudent", policy => policy
    .RequireClaim("DeleteStudent", "true")
    .RequireRole("Admin")
    .RequireRole("Super Admin"));

    //POLICY BASED ON FUNC CHOICE
    options.AddPolicy("example", policy => policy
    .RequireAssertion(param =>
    param.User.HasClaim(Claim => Claim.Type == "DeleteStudent" && Claim.Value == "true") &&
    param.User.IsInRole("Admin") ||
    param.User.IsInRole("Super Admin")

    ));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EditOtherUsersPolicy", policy =>
        policy.Requirements.Add(new EditOtherUsersRequirement()));
});

builder.Services.AddScoped<IAuthorizationHandler, EditOtherUsersHandler>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();



//REDIRECTS TO ACCESSS DENIED PAGE
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Student/AccessDenied"; // Redirect unauthorized users
});


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Student}/{action=Index}/{id?}");

app.MapRazorPages();
app.Run();
