using IntiGuard.Models;
using IntiGuard.Repositories;
namespace IntiGuard.Services.Interfaces
{
    public interface IComprobanteService
    {
        IEnumerable<Comprobante> GetAll();
        Comprobante GetById(int id);
        (Comprobante comprobante, decimal? total, IEnumerable<DetalleVenta>? detalles) GetDetails(int id, object? tempData);
        (Comprobante comprobante, decimal? total, IEnumerable<DetalleVenta>? detalles) GetDetailsAdmin(int id);
    }
}