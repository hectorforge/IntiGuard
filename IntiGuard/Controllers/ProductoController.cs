using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntiGuard.Controllers
{
    public class ProductoController : Controller
    {
        private readonly IProductoCrud _productoCrud;

        public ProductoController(IProductoCrud productoCrud)
        {
            _productoCrud = productoCrud;
        }
        public IActionResult Index()
        {
            var productos = _productoCrud.GetAll();
            if (User.IsInRole("ADMIN"))
                return View("AdminIndex", productos); // vista admin
            return View("Index", productos); // vista usuario
        }
        public IActionResult Details(int id)
        {
            var producto = _productoCrud.GetById(id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Producto());
        }

        [HttpPost]
        public IActionResult Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _productoCrud.Create(producto);
                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var producto = _productoCrud.GetById(id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [HttpPost]
        public IActionResult Edit(int id, Producto producto)
        {
            if (id != producto.IdProducto)
                return BadRequest();

            if (ModelState.IsValid)
            {
                _productoCrud.Update(id, producto);
                return RedirectToAction(nameof(Index));
            }

            return View(producto);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var producto = _productoCrud.GetById(id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        [HttpPost]
        public IActionResult DeleteConfirmed(int id)
        {
            _productoCrud.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
