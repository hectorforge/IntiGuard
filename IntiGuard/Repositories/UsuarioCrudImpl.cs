using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace IntiGuard.Repositories
{
    public class UsuarioCrudImpl : IUsuarioCrud
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

            cmd.Parameters.AddWithValue("@nombres", entity.Nombres ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@apellidos", entity.Apellidos ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@correo", entity.Correo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@telefono", entity.Telefono ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", entity.Direccion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@foto", entity.Foto ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@clave", entity.Clave ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@id_rol", entity.IdRol ?? (object)DBNull.Value);

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
                    IdUsuario = reader["id_usuario"] as int?,
                    Nombres = reader["nombres"] as string,
                    Apellidos = reader["apellidos"] as string,
                    Correo = reader["correo"] as string,
                    Telefono = reader["telefono"] as string,
                    Direccion = reader["direccion"] as string,
                    Foto = reader["foto"] as string,
                    Clave = reader["clave"] as string,
                    IdRol = reader["id_rol"] as int?,
                    NombreRol = reader["nombre_rol"] as string,
                    FechaRegistro = reader["fecha_registro"] is DBNull ? null : (DateTime?)reader["fecha_registro"]
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
                    IdUsuario = reader["id_usuario"] as int?,
                    Nombres = reader["nombres"] as string,
                    Apellidos = reader["apellidos"] as string,
                    Correo = reader["correo"] as string,
                    Telefono = reader["telefono"] as string,
                    Direccion = reader["direccion"] as string,
                    Foto = reader["foto"] as string,
                    Clave = reader["clave"] as string,
                    IdRol = reader["id_rol"] as int?,
                    NombreRol = reader["nombre_rol"] as string,
                    FechaRegistro = reader["fecha_registro"] is DBNull ? null : (DateTime?)reader["fecha_registro"]
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
            cmd.Parameters.AddWithValue("@nombres", entity.Nombres ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@apellidos", entity.Apellidos ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@correo", entity.Correo ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@telefono", entity.Telefono ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@direccion", entity.Direccion ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@foto", entity.Foto ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@clave", entity.Clave ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@id_rol", entity.IdRol ?? (object)DBNull.Value);

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
