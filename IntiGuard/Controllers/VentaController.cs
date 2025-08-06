using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using IntiGuard.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntiGuard.Controllers
{
    public class VentaController : Controller
    {
        private readonly IVentaService _ventaService;
        private readonly IVentaCrud _ventaCrud;
        private readonly IProductoCrud _productoCrud;

        public VentaController(IVentaService ventaService, IVentaCrud ventaCrud, IProductoCrud productoCrud)
        {
            _ventaService = ventaService;
            _ventaCrud = ventaCrud;
            _productoCrud = productoCrud;
        }

        public IActionResult Index()
        {
            var ventas = _ventaCrud.GetAll();
            return View(ventas);
        }

        public IActionResult Details(int id)
        {
            var venta = _ventaCrud.GetById(id);
            if (venta == null) return NotFound();
            return View(venta);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Productos = _productoCrud.GetAll();
            return View(new Venta());
        }

        [HttpPost]
        public IActionResult Create(Venta venta, List<DetalleVenta> detalles)
        {
            if (_ventaService.RegistrarVenta(venta, detalles))
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "No se pudo registrar la venta");
            ViewBag.Productos = _productoCrud.GetAll();
            return View(venta);
        }
    }
}