using IntiGuard.Models;
using IntiGuard.Repositories;
using IntiGuard.Repositories.Interfaces;
using IntiGuard.Services.Implements;
using IntiGuard.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
var builder = WebApplication.CreateBuilder(args);

// Configuracion para la seguridad
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

// Add services to the container.
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();

// Para inyectar los repositorios en los servicios
builder.Services.AddScoped<IDetalleVentaCrud, DetalleVentaCrudImpl>();
builder.Services.AddScoped<IProductoCrud, ProductoCrudImpl>();
builder.Services.AddScoped<IVentaCrud, VentaCrudImpl>();
builder.Services.AddScoped<IUsuarioCrud, UsuarioCrudImpl>(); 

builder.Services.AddScoped<ICrud<Comprobante>, ComprobanteCrudImpl>();
builder.Services.AddScoped<ICrud<Rol>, RolCrudImpl>();
builder.Services.AddScoped<ICrud<Usuario>, UsuarioCrudImpl>();


// Para inyectar los servicios en los controladores
builder.Services.AddScoped<IVentaService, VentaServiceImpl>();

//permite acceder al HttpContext actual desde cualquier parte de la aplicación
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication(); // antes de authorization importante para q fuincione
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
