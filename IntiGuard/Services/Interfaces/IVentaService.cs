using IntiGuard.Models;
using IntiGuard.Repositories;
namespace IntiGuard.Services.Interfaces
{
    public interface IVentaService
    {
        bool RegistrarVenta(Venta venta, IEnumerable<DetalleVenta> detalles);
        Comprobante GenerarComprobante(Venta venta);
    }
}
