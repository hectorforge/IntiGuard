using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntiGuard.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioCrud _usuarioCrud;

        public UsuarioController(IUsuarioCrud usuarioCrud)
        {
            _usuarioCrud = usuarioCrud;
        }

        public IActionResult Index()
        {
            var usuarios = _usuarioCrud.GetAll();
            return View(usuarios);
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
            return View(new Usuario());
        }

        [HttpPost]
        public IActionResult Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                _usuarioCrud.Create(usuario);
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var usuario = _usuarioCrud.GetById(id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost]
        public IActionResult Edit(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
                return BadRequest();

            if (ModelState.IsValid)
            {
                _usuarioCrud.Update(id, usuario);
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var usuario = _usuarioCrud.GetById(id);
            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            _usuarioCrud.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
