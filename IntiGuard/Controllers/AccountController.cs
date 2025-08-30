using IntiGuard.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Security.Claims;

namespace IntiGuard.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        public AccountController(IConfiguration config)
        {
            _config = config;
        }

        // ----------------------------------- Login -----------------------------------
        // GET: Mostrar formulario de login
        // test github quitar 
        [HttpGet]
        public IActionResult Login(string? message)
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string clave)
        {
            Usuario? usuario = null;

            using (var conn = new SqlConnection(_config.GetConnectionString("IntiGuardDB")))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                                SELECT u.id_usuario, u.nombres, u.apellidos, u.correo, u.telefono, u.direccion, u.foto,
                                       u.clave, u.id_rol, r.nombre_rol
                                FROM usuario u
                                INNER JOIN rol r ON u.id_rol = r.id_rol
                                WHERE u.correo = @Correo", conn);
                cmd.Parameters.AddWithValue("@Correo", correo);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        usuario = new Usuario
                        {
                            IdUsuario = (int)reader["id_usuario"],
                            Nombres = reader["nombres"].ToString(),
                            Apellidos = reader["apellidos"].ToString(),
                            Correo = reader["correo"].ToString(),
                            Telefono = reader["telefono"].ToString(),
                            Direccion = reader["direccion"].ToString(),
                            Foto = reader["foto"].ToString(),
                            Clave = reader["clave"].ToString(),
                            IdRol = (int)reader["id_rol"],
                            NombreRol = reader["nombre_rol"].ToString()
                        };
                    }
                }
            }

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(clave, usuario.Clave))
            {
                ViewBag.Error = "Credenciales inválidas";
                return View();
            }

            await SignInUser(usuario);
            return RedirectToAction("Index", "Home");
        }

        // ----------------------------------- Logout -----------------------------------
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // ----------------------------------- Registro local -----------------------------------
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(Usuario model)
        {
            if (!ModelState.IsValid) return View(model);

            using (var conn = new SqlConnection(_config.GetConnectionString("IntiGuardDB")))
            {
                conn.Open();
                var checkCmd = new SqlCommand("SELECT COUNT(*) FROM usuario WHERE correo=@Correo", conn);
                checkCmd.Parameters.AddWithValue("@Correo", model.Correo);
                int count = (int)checkCmd.ExecuteScalar();

                if (count > 0)
                {
                    ViewBag.Error = "El correo ya está registrado";
                    return View(model);
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Clave);

                var insertCmd = new SqlCommand("sp_usuario_create", conn);
                insertCmd.CommandType = System.Data.CommandType.StoredProcedure;
                insertCmd.Parameters.AddWithValue("@nombres", model.Nombres);
                insertCmd.Parameters.AddWithValue("@apellidos", model.Apellidos);
                insertCmd.Parameters.AddWithValue("@correo", model.Correo);
                insertCmd.Parameters.AddWithValue("@telefono", model.Telefono ?? "");
                insertCmd.Parameters.AddWithValue("@direccion", model.Direccion ?? "");
                insertCmd.Parameters.AddWithValue("@foto", model.Foto ?? "https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg");
                insertCmd.Parameters.AddWithValue("@clave", hashedPassword);
                insertCmd.Parameters.AddWithValue("@id_rol", model.IdRol ?? 2);
                insertCmd.ExecuteNonQuery();
            }

            return RedirectToAction("Login", new { message = "Cuenta registrada exitosamente" });
        }

        // ----------------------------------- Login con Google -----------------------------------
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync("Google");

            if (result?.Principal != null)
            {
                var email = result.Principal.FindFirstValue(ClaimTypes.Email);
                var nombre = result.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "";
                var apellido = result.Principal.FindFirstValue(ClaimTypes.Surname) ?? "";

                var fotoClaim = result.Principal.Claims.FirstOrDefault(c => c.Type.EndsWith("picture"));
                var foto = fotoClaim?.Value
                    ?? "https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg";

                Usuario usuario = null;

                using (var conn = new SqlConnection(_config.GetConnectionString("IntiGuardDB")))
                {
                    conn.Open();
                    var checkCmd = new SqlCommand("SELECT * FROM usuario WHERE correo=@Correo", conn);
                    checkCmd.Parameters.AddWithValue("@Correo", email);

                    using (var reader = checkCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = (int)reader["id_usuario"],
                                Nombres = reader["nombres"].ToString(),
                                Apellidos = reader["apellidos"].ToString(),
                                Correo = reader["correo"].ToString(),
                                Telefono = reader["telefono"].ToString(),
                                Direccion = reader["direccion"].ToString(),
                                Foto = string.IsNullOrEmpty(reader["foto"].ToString())
                                    ? foto
                                    : reader["foto"].ToString(),
                                IdRol = (int)reader["id_rol"],
                                NombreRol = reader["id_rol"].ToString(),
                                Clave = reader["clave"].ToString()
                            };
                        }
                    }
                }

                if (usuario == null)
                {
                    TempData["GoogleEmail"] = email;
                    TempData["GoogleNombre"] = nombre;
                    TempData["GoogleApellido"] = apellido;
                    TempData["GoogleFoto"] = foto;

                    return RedirectToAction("CompleteProfile");
                }

                await SignInUser(usuario);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login");
        }



        // ----------------------------------- Completar perfil -----------------------------------
        [HttpGet]
        public IActionResult CompleteProfile()
        {
            var model = new Usuario
            {
                Correo = TempData["GoogleEmail"]?.ToString(),
                Nombres = TempData["GoogleNombre"]?.ToString(),
                Apellidos = TempData["GoogleApellido"]?.ToString(),
                Foto = TempData["GoogleFoto"]?.ToString()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CompleteProfile(Usuario model)
        {
            if (!ModelState.IsValid) return View(model);

            Usuario usuario = null;

            using (var conn = new SqlConnection(_config.GetConnectionString("IntiGuardDB")))
            {
                conn.Open();

                var checkCmd = new SqlCommand("SELECT COUNT(*) FROM usuario WHERE correo=@Correo", conn);
                checkCmd.Parameters.AddWithValue("@Correo", model.Correo);
                int count = (int)checkCmd.ExecuteScalar();

                if (count == 0)
                {
                    string hashedPassword = !string.IsNullOrEmpty(model.Clave)
                        ? BCrypt.Net.BCrypt.HashPassword(model.Clave)
                        : "";

                    var insertCmd = new SqlCommand("sp_usuario_create", conn);
                    insertCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    insertCmd.Parameters.AddWithValue("@nombres", model.Nombres ?? "");
                    insertCmd.Parameters.AddWithValue("@apellidos", model.Apellidos ?? "");
                    insertCmd.Parameters.AddWithValue("@correo", model.Correo ?? "");
                    insertCmd.Parameters.AddWithValue("@telefono", model.Telefono ?? "");
                    insertCmd.Parameters.AddWithValue("@direccion", model.Direccion ?? "");
                    insertCmd.Parameters.AddWithValue("@foto", model.Foto ?? "https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg");
                    insertCmd.Parameters.AddWithValue("@clave", hashedPassword);
                    insertCmd.Parameters.AddWithValue("@id_rol", model.IdRol ?? 2);
                    insertCmd.ExecuteNonQuery();
                }
                else
                {
                    string sql;
                    SqlCommand cmd;

                    if (!string.IsNullOrEmpty(model.Clave))
                    {
                        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Clave);
                        sql = "UPDATE usuario SET nombres=@Nom, apellidos=@Ape, telefono=@Tel, direccion=@Dir, clave=@Clave, foto=@Foto WHERE correo=@Correo";
                        cmd = new SqlCommand(sql, conn);
                        cmd.Parameters.AddWithValue("@Clave", hashedPassword);
                    }
                    else
                    {
                        sql = "UPDATE usuario SET nombres=@Nom, apellidos=@Ape, telefono=@Tel, direccion=@Dir, foto=@Foto WHERE correo=@Correo";
                        cmd = new SqlCommand(sql, conn);
                    }

                    cmd.Parameters.AddWithValue("@Nom", model.Nombres ?? "");
                    cmd.Parameters.AddWithValue("@Ape", model.Apellidos ?? "");
                    cmd.Parameters.AddWithValue("@Tel", model.Telefono ?? "");
                    cmd.Parameters.AddWithValue("@Dir", model.Direccion ?? "");
                    cmd.Parameters.AddWithValue("@Foto", string.IsNullOrEmpty(model.Foto)
                        ? "https://thumbs.dreamstime.com/b/vector-de-perfil-avatar-predeterminado-foto-usuario-redes-sociales-desconocida-icono-desconocido-en-184816085.jpg"
                        : model.Foto);
                    cmd.Parameters.AddWithValue("@Correo", model.Correo);
                    cmd.ExecuteNonQuery();
                }

                var reloadCmd = new SqlCommand("SELECT TOP 1 * FROM usuario WHERE correo=@Correo", conn);
                reloadCmd.Parameters.AddWithValue("@Correo", model.Correo);
                using var readerReload = reloadCmd.ExecuteReader();
                if (readerReload.Read())
                {
                    usuario = new Usuario
                    {
                        IdUsuario = (int)readerReload["id_usuario"],
                        Nombres = readerReload["nombres"].ToString(),
                        Apellidos = readerReload["apellidos"].ToString(),
                        Correo = readerReload["correo"].ToString(),
                        Telefono = readerReload["telefono"].ToString(),
                        Direccion = readerReload["direccion"].ToString(),
                        Foto = readerReload["foto"].ToString(),
                        IdRol = (int)readerReload["id_rol"],
                        NombreRol = readerReload["id_rol"].ToString()
                    };
                }
            }
            /*
            if (usuario != null)
            {
                await SignInUser(usuario);
                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login");*/

            return RedirectToAction("Login", new { message = "Cuenta registrada exitosamente" });

        }

        // ----------------------------------- Access Denied -----------------------------------
        public IActionResult AccessDenied() => View();

        // ----------------------------------- Helper: Firmar sesion con claims completos -----------------------------------
        private async Task SignInUser(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Name, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.NombreRol ?? "User"),
                new Claim("FullName", $"{usuario.Nombres} {usuario.Apellidos}"),
                new Claim("Nombres", usuario.Nombres ?? ""),
                new Claim("Apellidos", usuario.Apellidos ?? ""),
                new Claim("Foto", usuario.Foto ?? ""),
                new Claim("Correo", usuario.Correo ?? ""),
                new Claim("Telefono", usuario.Telefono ?? ""),
                new Claim("Direccion", usuario.Direccion ?? ""),
                new Claim("IdRol", usuario.IdRol.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }
}