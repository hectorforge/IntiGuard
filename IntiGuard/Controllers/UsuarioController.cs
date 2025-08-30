using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace IntiGuard.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class UsuarioController : Controller
    {
        private readonly IUsuarioCrud _usuarioCrud;
        private readonly string _connectionString;

        public UsuarioController(IUsuarioCrud usuarioCrud, IConfiguration configuration)
        {
            _usuarioCrud = usuarioCrud;
            _connectionString = configuration.GetConnectionString("IntiGuardDB");
        }

        private IEnumerable<Rol> GetRoles()
        {
            var roles = new List<Rol>();
            using var connection = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT id_rol, nombre_rol FROM rol", connection);

            connection.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                roles.Add(new Rol
                {
                    IdRol = reader.GetInt32(0),
                    NombreRol = reader.GetString(1)
                });
            }
            return roles;
        }

        public IActionResult Index()
        {
            var usuarios = _usuarioCrud.GetAll();
            var usuariosConEstado = usuarios.Select(u =>
            {
                u.HasPurchases = _usuarioCrud.HasPurchases(u.IdUsuario);
                return u;
            }).ToList();
            return View(usuariosConEstado);
        }

        public IActionResult Details(int id)
        {
            var usuario = _usuarioCrud.GetById(id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(GetRoles(), "IdRol", "NombreRol");
            return View(new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(usuario.Clave))
                    usuario.Clave = BCrypt.Net.BCrypt.HashPassword(usuario.Clave);

                _usuarioCrud.Create(usuario);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = new SelectList(GetRoles(), "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var usuario = _usuarioCrud.GetById(id);
            if (usuario == null)
                return NotFound();

            ViewBag.Roles = new SelectList(GetRoles(), "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
                return BadRequest();

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(usuario.Clave))
                    usuario.Clave = BCrypt.Net.BCrypt.HashPassword(usuario.Clave);
                else
                    usuario.Clave = null; // No actualizar clave

                _usuarioCrud.Update(id, usuario);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Roles = new SelectList(GetRoles(), "IdRol", "NombreRol", usuario.IdRol);
            return View(usuario);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var usuario = _usuarioCrud.GetById(id);
            if (usuario == null)
                return NotFound();

            if (_usuarioCrud.HasPurchases(id))
            {
                TempData["Error"] = "No se puede eliminar el usuario porque tiene compras registradas.";
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (_usuarioCrud.HasPurchases(id))
            {
                TempData["Error"] = "No se puede eliminar el usuario porque tiene compras registradas.";
                return RedirectToAction(nameof(Index));
            }

            _usuarioCrud.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
