using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using IntiGuard.ViewModels; // Clase PagedResult
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

        public IActionResult Index(int page = 1, int pageSize = 5)
        {
            var productos = _productoCrud.GetAll().Where(p => p.Stock > 0);

            int totalProductos = productos.Count();
            int totalPages = (int)Math.Ceiling((double)totalProductos / pageSize);

            if (User.IsInRole("ADMIN"))
            {
                var productosPaginados = productos
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedAdmin = new PagedResult<Producto>
                {
                    Items = productosPaginados,
                    TotalPages = totalPages,
                    CurrentPage = page
                };

                return View("Index", pagedAdmin); 
            }
            else
            {
                
                var pagedUser = new PagedResult<Producto>
                {
                    Items = productos.ToList(),
                    TotalPages = 1, 
                    CurrentPage = 1
                };

                return View("Index", pagedUser);
            }
        }

        // DETAILS
        public IActionResult Details(int id)
        {
            var producto = _productoCrud.GetById(id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // CREATE GET
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Producto());
        }

        // CREATE POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Producto producto)
        {
            if (ModelState.IsValid)
            {
                _productoCrud.Create(producto);
                return RedirectToAction(nameof(Index));
            }
            return View(producto);
        }

        // EDIT GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var producto = _productoCrud.GetById(id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // DELETE GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var producto = _productoCrud.GetById(id);
            if (producto == null)
                return NotFound();

            return View(producto);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _productoCrud.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
