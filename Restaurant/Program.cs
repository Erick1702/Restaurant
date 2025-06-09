using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Restaurant.Datos;
using Restaurant.Models;
using Restaurant.Servicios;
using Restaurant.Utilidades;
using Rotativa.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Recaptcha configuration
builder.Services.Configure<GoogleReCaptchaConfig>(
    builder.Configuration.GetSection("GoogleReCaptcha"));


builder.Services.AddTransient<IServicoUsuarios, ServicoUsuarios>();
//Tutorial
builder.Services.AddDbContextFactory<ApplicationDbContext>(opciones=>
opciones.UseSqlServer("name=DefaultConnection")
.UseSeeding(Seeding.Aplicar)
.UseAsyncSeeding(Seeding.AplicarAsync));

builder.Services.AddIdentity<Usuario, IdentityRole>(opciones =>
{
    opciones.SignIn.RequireConfirmedAccount = false;

    opciones.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(1);
    opciones.Lockout.MaxFailedAccessAttempts = 3;
    opciones.Lockout.AllowedForNewUsers = true;

}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();



//builder.Services.PostConfigure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, opciones =>
//{
//    opciones.LogoutPath = "/usuarios/login";
//    opciones.AccessDeniedPath = "/usuarios/login";
//});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Usuarios/Login";
    options.AccessDeniedPath = "/Usuarios/AccesoDenegado";
});


////
///RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");
var app = builder.Build();

//RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
//app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


//pdf
IWebHostEnvironment env = app.Environment;
Rotativa.AspNetCore.RotativaConfiguration.Setup(env.WebRootPath, "Rotativa");
//

app.Run();
