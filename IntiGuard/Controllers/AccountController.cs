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
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string correo, string clave)
        {
            Usuario usuario = null;

            using (var conn = new SqlConnection(_config.GetConnectionString("IntiGuardDB")))
            {
                await conn.OpenAsync();
                var cmd = new SqlCommand(@"
                                SELECT u.id_usuario, u.nombres, u.apellidos, u.correo, u.telefono, u.direccion,
                                       u.clave, u.id_rol, r.nombre_rol, u.fecha_registro
                                FROM usuario u
                                INNER JOIN rol r ON u.id_rol = r.id_rol
                                WHERE u.correo = @Correo", conn);
                cmd.CommandType = System.Data.CommandType.Text;
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
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Correo),
                    new Claim(ClaimTypes.Role, usuario.NombreRol),
                    new Claim("FullName", $"{usuario.Nombres} {usuario.Apellidos}")
                };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

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
                insertCmd.Parameters.AddWithValue("@clave", hashedPassword);
                insertCmd.Parameters.AddWithValue("@id_rol", model.IdRol ?? 2);
                insertCmd.ExecuteNonQuery();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, model.Correo),
                new Claim(ClaimTypes.Role, model.NombreRol ?? "User"),
                new Claim("FullName", $"{model.Nombres} {model.Apellidos}")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
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
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal != null)
            {
                var email = result.Principal.FindFirstValue(ClaimTypes.Email);
                var nombre = result.Principal.FindFirstValue(ClaimTypes.Name);

                using (var conn = new SqlConnection(_config.GetConnectionString("IntiGuardDB")))
                {
                    conn.Open();
                    var checkCmd = new SqlCommand("SELECT COUNT(*) FROM usuario WHERE correo=@Correo", conn);
                    checkCmd.Parameters.AddWithValue("@Correo", email);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        var insertCmd = new SqlCommand("sp_usuario_create", conn);
                        insertCmd.CommandType = System.Data.CommandType.StoredProcedure;
                        insertCmd.Parameters.AddWithValue("@nombres", nombre);
                        insertCmd.Parameters.AddWithValue("@apellidos", "");
                        insertCmd.Parameters.AddWithValue("@correo", email);
                        insertCmd.Parameters.AddWithValue("@telefono", "");
                        insertCmd.Parameters.AddWithValue("@direccion", "");
                        insertCmd.Parameters.AddWithValue("@clave", "");
                        insertCmd.Parameters.AddWithValue("@id_rol", 2);
                        insertCmd.ExecuteNonQuery();

                        return RedirectToAction("CompleteProfile", new { email });
                    }
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, email),
                    new Claim(ClaimTypes.Role, "User")
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return RedirectToAction("Index", "Home");
            }

            return RedirectToAction("Login");
        }

        // ----------------------------------- Completar perfil -----------------------------------
        [HttpGet]
        public IActionResult CompleteProfile(string email)
        {
            var model = new Usuario { Correo = email };
            return View(model);
        }

        // Aqui lo que yo hacia era logar pero nunca pedia una contrseña solo que solo se logea con google sin contraseña
        [HttpPost]
        public IActionResult CompleteProfile(Usuario model)
        {
            if (!ModelState.IsValid) return View(model);

            if (string.IsNullOrEmpty(model.Correo))
            {
                return BadRequest("Correo no proporcionado.");
            }

            using (var conn = new SqlConnection(_config.GetConnectionString("IntiGuardDB")))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "UPDATE usuario SET telefono=@Tel, direccion=@Dir WHERE correo=@Correo", conn);
                cmd.Parameters.AddWithValue("@Tel", model.Telefono ?? "");
                cmd.Parameters.AddWithValue("@Dir", model.Direccion ?? "");
                cmd.Parameters.AddWithValue("@Correo", model.Correo);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", "Home");
        }

        /*
        [HttpPost] // En esta nueva version ya si se logea con google puede hacerlo normal pero tambien por correo y contraseña directamente.
        public IActionResult CompleteProfile(Usuario model)
        {
            if (!ModelState.IsValid) return View(model);

            if (string.IsNullOrEmpty(model.Correo))
            {
                return BadRequest("Correo no proporcionado.");
            }

            using (var conn = new SqlConnection(_config.GetConnectionString("IntiGuardDB")))
            {
                conn.Open();

                string sql;
                SqlCommand cmd;

                if (!string.IsNullOrEmpty(model.Clave))
                {
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Clave);
                    sql = "UPDATE usuario SET telefono=@Tel, direccion=@Dir, clave=@Clave WHERE correo=@Correo";
                    cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@Clave", hashedPassword);
                }
                else
                {
                    sql = "UPDATE usuario SET telefono=@Tel, direccion=@Dir WHERE correo=@Correo";
                    cmd = new SqlCommand(sql, conn);
                }

                cmd.Parameters.AddWithValue("@Tel", model.Telefono ?? "");
                cmd.Parameters.AddWithValue("@Dir", model.Direccion ?? "");
                cmd.Parameters.AddWithValue("@Correo", model.Correo);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index", "Home");
        }*/

        // ----------------------------------- Access Denied -----------------------------------
        public IActionResult AccessDenied() => View();
    }
}