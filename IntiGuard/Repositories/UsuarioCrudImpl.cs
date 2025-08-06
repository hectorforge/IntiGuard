using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IntiGuard.Repositories
{
    public class UsuarioCrudImpl : ICrud<Usuario>
    {
        private readonly string _connectionString;

        public UsuarioCrudImpl(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("IntiGuardDB");
        }

        public Usuario Create(Usuario entity)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_usuario_create", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@nombres", entity.Nombres);
            cmd.Parameters.AddWithValue("@apellidos", entity.Apellidos);
            cmd.Parameters.AddWithValue("@correo", entity.Correo);
            cmd.Parameters.AddWithValue("@clave", entity.Clave);
            cmd.Parameters.AddWithValue("@id_rol", entity.IdRol);

            connection.Open();
            cmd.ExecuteNonQuery();
            return entity;
        }

        public Usuario GetById(int id)
        {
            Usuario usuario = null;
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_usuario_get_by_id", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id_usuario", id);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                usuario = new Usuario
                {
                    IdUsuario = reader.GetInt32(0),
                    Nombres = reader.GetString(1),
                    Apellidos = reader.GetString(2),
                    Correo = reader.GetString(3),
                    IdRol = reader.GetInt32(4),
                    NombreRol = reader.GetString(5),
                    FechaRegistro = reader.GetDateTime(6)
                };
            }
            return usuario;
        }

        public IEnumerable<Usuario> GetAll()
        {
            var lista = new List<Usuario>();
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_usuario_get_all", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Usuario
                {
                    IdUsuario = reader.GetInt32(0),
                    Nombres = reader.GetString(1),
                    Apellidos = reader.GetString(2),
                    Correo = reader.GetString(3),
                    IdRol = reader.GetInt32(4),
                    NombreRol = reader.GetString(5),
                    FechaRegistro = reader.GetDateTime(6)
                });
            }
            return lista;
        }

        public Usuario Update(int id, Usuario entity)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_usuario_update", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@id_usuario", id);
            cmd.Parameters.AddWithValue("@nombres", entity.Nombres);
            cmd.Parameters.AddWithValue("@apellidos", entity.Apellidos);
            cmd.Parameters.AddWithValue("@correo", entity.Correo);
            cmd.Parameters.AddWithValue("@clave", entity.Clave);
            cmd.Parameters.AddWithValue("@id_rol", entity.IdRol);

            connection.Open();
            cmd.ExecuteNonQuery();
            return entity;
        }

        public bool DeleteById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_usuario_delete", connection);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id_usuario", id);

            connection.Open();
            return cmd.ExecuteNonQuery() > 0;
        }

        public bool ExistsById(int id)
        {
            return GetById(id) != null;
        }
    }
}