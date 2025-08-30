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

        public void CreateWithTransaction(Usuario usuario)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var cmd = new SqlCommand("sp_usuario_create", connection, transaction);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@nombres", usuario.Nombres ?? "");
                cmd.Parameters.AddWithValue("@apellidos", usuario.Apellidos ?? "");
                cmd.Parameters.AddWithValue("@correo", usuario.Correo ?? "");
                cmd.Parameters.AddWithValue("@telefono", usuario.Telefono ?? "");
                cmd.Parameters.AddWithValue("@direccion", usuario.Direccion ?? "");
                cmd.Parameters.AddWithValue("@foto", usuario.Foto ?? "");
                cmd.Parameters.AddWithValue("@activo", usuario.Activo);
                cmd.Parameters.AddWithValue("@clave", usuario.Clave ?? "");
                cmd.Parameters.AddWithValue("@id_rol", usuario.IdRol ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public Usuario UpdateWithTransaction(int id, Usuario usuario)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                using var cmd = new SqlCommand("sp_usuario_update", connection, transaction);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@id_usuario", id);
                cmd.Parameters.AddWithValue("@nombres", usuario.Nombres ?? "");
                cmd.Parameters.AddWithValue("@apellidos", usuario.Apellidos ?? "");
                cmd.Parameters.AddWithValue("@correo", usuario.Correo ?? "");
                cmd.Parameters.AddWithValue("@telefono", usuario.Telefono ?? "");
                cmd.Parameters.AddWithValue("@direccion", usuario.Direccion ?? "");
                cmd.Parameters.AddWithValue("@activo", usuario.Activo); 
                cmd.Parameters.AddWithValue("@foto", usuario.Foto ?? "");
                cmd.Parameters.AddWithValue("@clave", string.IsNullOrEmpty(usuario.Clave) ? (object)DBNull.Value : usuario.Clave);
                cmd.Parameters.AddWithValue("@id_rol", usuario.IdRol ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();

                transaction.Commit();
                return usuario;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
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
                    Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Direccion = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Foto = reader.IsDBNull(6) ? null : reader.GetString(6),
                    IdRol = reader.GetInt32(7),
                    NombreRol = reader.GetString(8),
                    FechaRegistro = reader.GetDateTime(9),
                    Activo = reader.GetBoolean(10)
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
                    Telefono = reader.IsDBNull(4) ? null : reader.GetString(4),
                    Direccion = reader.IsDBNull(5) ? null : reader.GetString(5),
                    Foto = reader.IsDBNull(6) ? null : reader.GetString(6),
                    IdRol = reader.GetInt32(7),
                    NombreRol = reader.GetString(8),
                    FechaRegistro = reader.GetDateTime(9),
                    Activo = reader.GetBoolean(10)
                });
            }
            return lista;
        }


        public Usuario Update(int id, Usuario usuario) =>
            UpdateWithTransaction(id, usuario);

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

        public Usuario Create(Usuario entity)
        {
            CreateWithTransaction(entity);
            return entity;
        }

        public bool HasPurchases(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM venta WHERE id_usuario = @id", connection);

            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            int count = (int)cmd.ExecuteScalar();

            return count > 0;
        }

    }
}
