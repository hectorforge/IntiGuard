using IntiGuard.Models;
using Microsoft.Data.SqlClient;

namespace IntiGuard.Repositories.Interfaces
{
    public interface IComprobanteCrud : ICrud<Comprobante>
    {
        int InsertComprobanteTransaccion(Comprobante comprobante, SqlConnection connection, SqlTransaction transaction);
    }
}