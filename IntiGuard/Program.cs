using IntiGuard.Models;
using IntiGuard.Repositories;
using IntiGuard.Repositories.Interfaces;
using IntiGuard.Services.Implements;
using IntiGuard.Services.Interfaces;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Para inyectar los repositorios en los servicios
builder.Services.AddScoped<IDetalleVentaCrud, DetalleVentaCrudImpl>();
builder.Services.AddScoped<IProductoCrud, ProductoCrudImpl>();
builder.Services.AddScoped<IVentaCrud, VentaCrudImpl>();

builder.Services.AddScoped<ICrud<Comprobante>, ComprobanteCrudImpl>();
builder.Services.AddScoped<ICrud<Rol>, RolCrudImpl>();
builder.Services.AddScoped<ICrud<Usuario>, UsuarioCrudImpl>();

// Para inyectar los servicios en los controladores
builder.Services.AddScoped<IVentaService, VentaServiceImpl>();

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
