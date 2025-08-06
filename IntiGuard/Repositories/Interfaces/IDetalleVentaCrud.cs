using IntiGuard.Models;
using Microsoft.Data.SqlClient;

namespace IntiGuard.Repositories.Interfaces
{
    public interface IDetalleVentaCrud : ICrud<DetalleVenta>
    {
        void InsertDetalleVentaTransaccion(DetalleVenta detalle, SqlConnection connection, SqlTransaction transaction);
    }
}
