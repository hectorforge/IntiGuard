using IntiGuard.Models;
using IntiGuard.Repositories;
using IntiGuard.Repositories.Interfaces;
using IntiGuard.Services.Implements;
using IntiGuard.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

//  Configuración de Seguridad
// =============================
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    options.CallbackPath = "/signin-google";
    options.ClaimActions.MapJsonKey("picture", "picture", "url");
});

//  Servicios del contenedor
// =============================
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();

// Repositorios
builder.Services.AddScoped<IDetalleVentaCrud, DetalleVentaCrudImpl>();
builder.Services.AddScoped<IProductoCrud, ProductoCrudImpl>();
builder.Services.AddScoped<IVentaCrud, VentaCrudImpl>();
builder.Services.AddScoped<IUsuarioCrud, UsuarioCrudImpl>();

builder.Services.AddScoped<ICrud<Comprobante>, ComprobanteCrudImpl>();
builder.Services.AddScoped<ICrud<Rol>, RolCrudImpl>();
builder.Services.AddScoped<ICrud<Usuario>, UsuarioCrudImpl>();

builder.Services.AddScoped<IComprobanteCrud, ComprobanteCrudImpl>();

// Servicios
builder.Services.AddScoped<IVentaService, VentaServiceImpl>();

// Acceso a HttpContext
builder.Services.AddHttpContextAccessor();

//  Configuración de Sesiones
// =============================
builder.Services.AddDistributedMemoryCache(); // requerido por Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

//  Configuración del Pipeline
// =============================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();