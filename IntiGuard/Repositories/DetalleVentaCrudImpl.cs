using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IntiGuard.Repositories
{
    public class DetalleVentaCrudImpl : IDetalleVentaCrud
    {
        private readonly string _connectionString;

        public DetalleVentaCrudImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("IntiGuardDB");
        }

        public DetalleVenta Create(DetalleVenta entity)
        {
            throw new NotImplementedException("Los detalles de venta se crean al registrar la venta.");
        }

        public DetalleVenta GetById(int id)
        {
            // No hay SP directo para obtener un detalle de venta por ID
            throw new NotImplementedException("No existe un procedimiento para obtener un detalle por ID.");
        }

        public IEnumerable<DetalleVenta> GetAll()
        {
            // No tiene sentido listar todos los detalles de todas las ventas
            throw new NotImplementedException("No existe un procedimiento para obtener todos los detalles de venta.");
        }

        public IEnumerable<DetalleVenta> GetByVentaId(int idVenta)
        {
            var lista = new List<DetalleVenta>();
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_detalle_venta_get_by_venta", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id_venta", idVenta);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new DetalleVenta
                {
                    IdDetalleVenta = reader.GetInt32(0),
                    NombreProducto = reader.GetString(1),
                    Cantidad = reader.GetInt32(2),
                    PrecioUnitario = reader.GetDecimal(3)
                });
            }
            return lista;
        }

        public DetalleVenta Update(int id, DetalleVenta entity)
        {
            throw new NotImplementedException("No se actualizan detalles de venta desde la aplicación.");
        }

        public bool DeleteById(int id)
        {
            throw new NotImplementedException("No se eliminan detalles de venta desde la aplicación.");
        }

        public bool ExistsById(int id)
        {
            throw new NotImplementedException("No hay validación directa para existencia de detalles.");
        }

        // Para las transacciones
        public void InsertDetalleVentaTransaccion(DetalleVenta detalle, SqlConnection connection, SqlTransaction transaction)
        {
            using var cmd = new SqlCommand("sp_detalle_venta_create_transaccion", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id_venta", detalle.IdVenta);
            cmd.Parameters.AddWithValue("@id_producto", detalle.IdProducto);
            cmd.Parameters.AddWithValue("@cantidad", detalle.Cantidad);
            cmd.Parameters.AddWithValue("@precio_unitario", detalle.PrecioUnitario);

            cmd.ExecuteNonQuery();
        }

    }
}