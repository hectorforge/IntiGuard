using IntiGuard.Models;
using Microsoft.Data.SqlClient;

namespace IntiGuard.Repositories.Interfaces
{
    public interface IProductoCrud : ICrud<Producto>
    {
        void DescontarStockTransaccion(int idProducto, int cantidad, SqlConnection connection, SqlTransaction transaction);
    }
}
