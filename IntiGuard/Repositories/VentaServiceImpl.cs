using IntiGuard.Models;
using IntiGuard.Services;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IntiGuard.Repositories
{
    public class VentaServiceImpl : ICrud<Venta>
    {
        private readonly string _connectionString;

        public VentaServiceImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public Venta Create(Venta entity)
        {
            throw new NotImplementedException("La creación de ventas se realiza en el backend mediante lógica específica.");
        }

        public Venta GetById(int id)
        {
            Venta venta = null;
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_venta_get_by_id", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id_venta", id);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                venta = new Venta
                {
                    IdVenta = reader.GetInt32(0),
                    Cliente = reader.GetString(1),
                    TipoComprobante = reader.GetString(2),
                    NumeroComprobante = reader.GetString(3),
                    Total = reader.GetDecimal(4),
                    FechaVenta = reader.GetDateTime(5)
                };
            }
            return venta;
        }

        public IEnumerable<Venta> GetAll()
        {
            var lista = new List<Venta>();
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_venta_get_all", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Venta
                {
                    IdVenta = reader.GetInt32(0),
                    Cliente = reader.GetString(1),
                    TipoComprobante = reader.GetString(2),
                    NumeroComprobante = reader.GetString(3),
                    Total = reader.GetDecimal(4),
                    FechaVenta = reader.GetDateTime(5)
                });
            }
            return lista;
        }

        public Venta Update(int id, Venta entity)
        {
            throw new NotImplementedException("No se actualizan ventas desde la aplicación.");
        }

        public bool DeleteById(int id)
        {
            throw new NotImplementedException("No se eliminan ventas desde la aplicación.");
        }

        public bool ExistsById(int id)
        {
            return GetById(id) != null;
        }
    }
}