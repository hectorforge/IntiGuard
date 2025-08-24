using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IntiGuard.Repositories
{
    public class ComprobanteCrudImpl : IComprobanteCrud
    {
        private readonly string _connectionString;

        public ComprobanteCrudImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("IntiGuardDB");
        }

        public int InsertComprobanteTransaccion(Comprobante comprobante, SqlConnection connection, SqlTransaction transaction)
        {
            using var cmd = new SqlCommand("sp_comprobante_insert", connection, transaction);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@TipoComprobante", comprobante.TipoComprobante);
            cmd.Parameters.AddWithValue("@NumeroComprobante", comprobante.NumeroComprobante);

            var idParam = new SqlParameter("@IdComprobante", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(idParam);

            cmd.ExecuteNonQuery();

            return (int)idParam.Value;
        }

        public Comprobante Create(Comprobante entity)
        {
            throw new NotImplementedException("Los comprobantes se generan en el backend y no se crean manualmente aquí.");
        }

        public Comprobante GetById(int id)
        {
            return GetAll().FirstOrDefault(c => c.IdComprobante == id);
        }

        public IEnumerable<Comprobante> GetAll()
        {
            var lista = new List<Comprobante>();
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_comprobante_get_all", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Comprobante
                {
                    IdComprobante = reader.GetInt32(0),
                    TipoComprobante = reader.GetString(1),
                    NumeroComprobante = reader.GetString(2)
                });
            }
            return lista;
        }

        public Comprobante Update(int id, Comprobante entity)
        {
            throw new NotImplementedException("No se actualizan comprobantes desde la aplicación.");
        }

        public bool DeleteById(int id)
        {
            throw new NotImplementedException("No se eliminan comprobantes desde la aplicación.");
        }

        public bool ExistsById(int id)
        {
            return GetById(id) != null;
        }
    }
}
