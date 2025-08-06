using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IntiGuard.Repositories
{
    public class ProductoCrudImpl : IProductoCrud
    {
        private readonly string _connectionString;

        public ProductoCrudImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Producto Create(Producto entity)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_producto_create", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@nombre_producto", entity.NombreProducto);
            cmd.Parameters.AddWithValue("@descripcion", (object?)entity.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@marca", (object?)entity.Marca ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@precio", entity.Precio);
            cmd.Parameters.AddWithValue("@stock", entity.Stock);
            cmd.Parameters.AddWithValue("@imagen_url", (object?)entity.ImagenUrl ?? DBNull.Value);

            connection.Open();
            cmd.ExecuteNonQuery();
            return entity;
        }

        public Producto GetById(int id)
        {
            Producto producto = null;
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_producto_get_by_id", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id_producto", id);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                producto = new Producto
                {
                    IdProducto = reader.GetInt32(0),
                    NombreProducto = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Marca = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Precio = reader.GetDecimal(4),
                    Stock = reader.GetInt32(5),
                    ImagenUrl = reader.IsDBNull(6) ? null : reader.GetString(6)
                };
            }
            return producto;
        }

        public IEnumerable<Producto> GetAll()
        {
            var lista = new List<Producto>();
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_producto_get_all", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Producto
                {
                    IdProducto = reader.GetInt32(0),
                    NombreProducto = reader.GetString(1),
                    Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Marca = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Precio = reader.GetDecimal(4),
                    Stock = reader.GetInt32(5),
                    ImagenUrl = reader.IsDBNull(6) ? null : reader.GetString(6)
                });
            }
            return lista;
        }

        public Producto Update(int id, Producto entity)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_producto_update", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id_producto", id);
            cmd.Parameters.AddWithValue("@nombre_producto", entity.NombreProducto);
            cmd.Parameters.AddWithValue("@descripcion", (object?)entity.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@marca", (object?)entity.Marca ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@precio", entity.Precio);
            cmd.Parameters.AddWithValue("@stock", entity.Stock);
            cmd.Parameters.AddWithValue("@imagen_url", (object?)entity.ImagenUrl ?? DBNull.Value);

            connection.Open();
            cmd.ExecuteNonQuery();
            return entity;
        }

        public bool DeleteById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_producto_delete", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id_producto", id);

            connection.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ExistsById(int id)
        {
            return GetById(id) != null;
        }

        // Para las transacciones
        public void DescontarStockTransaccion(int idProducto, int cantidad, SqlConnection connection, SqlTransaction transaction)
        {
            using var cmd = new SqlCommand("sp_producto_descontar_stock_transaccion", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id_producto", idProducto);
            cmd.Parameters.AddWithValue("@cantidad", cantidad);

            cmd.ExecuteNonQuery();
        }
    }
}