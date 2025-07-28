using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using IntiGuard.Models; 
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity; // For password hashing (if not using custom hash)

namespace IntiGuard.Controllers
{
    public class AccountController : Controller
    {
        private readonly string _connectionString = "server=LAPTOP-U1JQH05L; database=IntiGuardDB;" +
                                                      "Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=false;Encrypt=false";

        // This would ideally come from a service/repository layer, not directly in the controller
        private async Task<Usuario?> GetUserByEmailAsync(string email)
        {
            Usuario? user = null;
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_validar_usuario", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@correo", email);
                    await cn.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            user = new Usuario
                            {
                                id_usuario = dr.GetInt32(dr.GetOrdinal("id_usuario")),
                                nombres = dr.GetString(dr.GetOrdinal("nombres")),
                                apellidos = dr.GetString(dr.GetOrdinal("apellidos")),
                                correo = dr.GetString(dr.GetOrdinal("correo")),
                                clave = dr.GetString(dr.GetOrdinal("clave")), // Remember to hash passwords!
                                id_rol = dr.GetInt32(dr.GetOrdinal("id_rol"))
                            };
                        }
                    }
                }
            }
            return user;
        }

        // Example: Login action
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string clave)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(clave))
            {
                ModelState.AddModelError("", "Por favor ingrese correo y clave.");
                return View();
            }

            Usuario? user = await GetUserByEmailAsync(correo);

            if (user != null)
            {
                if (user.clave == clave) 
                {
                 
                    TempData["SuccessMessage"] = $"Bienvenido, {user.nombres}!";
                    return RedirectToAction("Index", "Home"); // Redirect to home page or dashboard
                }
            }

            ModelState.AddModelError("", "Credenciales inválidas.");
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Usuario newUser)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection cn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO usuario (nombres, apellidos, correo, clave, id_rol) VALUES (@nombres, @apellidos, @correo, @clave, @id_rol)", cn))
                    {
                        cmd.Parameters.AddWithValue("@nombres", newUser.nombres);
                        cmd.Parameters.AddWithValue("@apellidos", newUser.apellidos);
                        cmd.Parameters.AddWithValue("@correo", newUser.correo);
                        cmd.Parameters.AddWithValue("@clave", newUser.clave); // Placeholder: Replace with hashed password
                        cmd.Parameters.AddWithValue("@id_rol", 2); // Default to 'Cliente' role
                        await cn.OpenAsync();
                        try
                        {
                            await cmd.ExecuteNonQueryAsync();
                            TempData["SuccessMessage"] = "Registro exitoso. ¡Ahora puedes iniciar sesión!";
                            return RedirectToAction("Login");
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == 2627) // Unique constraint violation (e.g., duplicate email)
                            {
                                ModelState.AddModelError("correo", "Este correo ya está registrado.");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error al registrar el usuario. Por favor, intente de nuevo.");
                            }
                            return View(newUser);
                        }
                    }
                }
            }
            return View(newUser);
        }
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(int id_usuario, string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                ModelState.AddModelError("", "La nueva contraseña no puede estar vacía.");
                return View();
            }

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_cambiar_clave_usuario", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", id_usuario);
                    cmd.Parameters.AddWithValue("@nueva_clave", newPassword); // Placeholder: Replace with hashed password
                    await cn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            TempData["SuccessMessage"] = "Contraseña actualizada exitosamente.";
            return RedirectToAction("Profile", new { id = id_usuario }); // Redirect to user profile or home
        }
        public async Task<IActionResult> Profile(int id)
        {
            Usuario? user = null;
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_leer_usuario_por_id", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", id);
                    await cn.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            user = new Usuario
                            {
                                id_usuario = dr.GetInt32(dr.GetOrdinal("id_usuario")),
                                nombres = dr.GetString(dr.GetOrdinal("nombres")),
                                apellidos = dr.GetString(dr.GetOrdinal("apellidos")),
                                correo = dr.GetString(dr.GetOrdinal("correo")),
                                fecha_registro = dr.GetDateTime(dr.GetOrdinal("fecha_registro")),
                                id_rol = dr.GetInt32(dr.GetOrdinal("id_rol"))
                            };
                        }
                    }
                }
            }
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public async Task<IActionResult> EditProfile(int id)
        {
            Usuario? user = null;
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_leer_usuario_por_id", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", id);
                    await cn.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            user = new Usuario
                            {
                                id_usuario = dr.GetInt32(dr.GetOrdinal("id_usuario")),
                                nombres = dr.GetString(dr.GetOrdinal("nombres")),
                                apellidos = dr.GetString(dr.GetOrdinal("apellidos")),
                                correo = dr.GetString(dr.GetOrdinal("correo")),
                                fecha_registro = dr.GetDateTime(dr.GetOrdinal("fecha_registro")),
                                id_rol = dr.GetInt32(dr.GetOrdinal("id_rol"))
                            };
                        }
                    }
                }
            }
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> EditProfile(Usuario updatedUser)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection cn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizar_usuario", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_usuario", updatedUser.id_usuario);
                        cmd.Parameters.AddWithValue("@nombres", updatedUser.nombres);
                        cmd.Parameters.AddWithValue("@apellidos", updatedUser.apellidos);
                        cmd.Parameters.AddWithValue("@correo", updatedUser.correo);
                        cmd.Parameters.AddWithValue("@id_rol", updatedUser.id_rol); 
                        await cn.OpenAsync();
                        try
                        {
                            await cmd.ExecuteNonQueryAsync();
                            TempData["SuccessMessage"] = "Perfil actualizado exitosamente.";
                            return RedirectToAction("Profile", new { id = updatedUser.id_usuario });
                        }
                        catch (SqlException ex)
                        {
                            if (ex.Number == 2627) 
                            {
                                ModelState.AddModelError("correo", "Este correo ya está registrado por otro usuario.");
                            }
                            else
                            {
                                ModelState.AddModelError("", "Error al actualizar el perfil. Por favor, intente de nuevo.");
                            }
                            return View(updatedUser);
                        }
                    }
                }
            }
            return View(updatedUser);
        }
    }
}
