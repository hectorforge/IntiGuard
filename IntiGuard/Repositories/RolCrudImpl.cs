using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IntiGuard.Repositories
{
    public class RolCrudImpl : ICrud<Rol>
    {
        private readonly string _connectionString;

        public RolCrudImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("IntiGuardDB");
        }

        public Rol Create(Rol entity)
        {
            throw new NotImplementedException("Los roles son constantes y no se crean desde la aplicación.");
        }

        public Rol GetById(int id)
        {
            return GetAll().FirstOrDefault(r => r.IdRol == id);
        }

        public IEnumerable<Rol> GetAll()
        {
            var lista = new List<Rol>();
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_rol_get_all", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Rol
                {
                    IdRol = reader.GetInt32(0),
                    NombreRol = reader.GetString(1)
                });
            }
            return lista;
        }

        public Rol Update(int id, Rol entity)
        {
            throw new NotImplementedException("No se actualizan roles desde la aplicación.");
        }

        public bool DeleteById(int id)
        {
            throw new NotImplementedException("No se eliminan roles desde la aplicación.");
        }

        public bool ExistsById(int id)
        {
            return GetById(id) != null;
        }
    }
}