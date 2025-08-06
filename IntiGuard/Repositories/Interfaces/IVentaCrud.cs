using IntiGuard.Models;
using Microsoft.Data.SqlClient;

namespace IntiGuard.Repositories.Interfaces
{
    public interface IVentaCrud : ICrud<Venta>
    {
        int InsertVentaTransaccion(Venta venta, SqlConnection connection, SqlTransaction transaction);
    }
}
