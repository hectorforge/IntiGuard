using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using IntiGuard.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using IntiGuard.Helpers;

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
            if (detalles == null || !detalles.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un producto.");
                ViewBag.Productos = _productoCrud.GetAll();
                return View(venta);
            }

            bool ok = _ventaService.RegistrarVenta(venta, detalles);
            if (ok)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError("", "No se pudo registrar la venta.");
            ViewBag.Productos = _productoCrud.GetAll();
            return View(venta);
        }

        // uso de SessionExtensions en Helpers
        [HttpPost]
        public IActionResult AgregarAlCarrito(int idProducto, int cantidad)
        {
            var producto = _productoCrud.GetById(idProducto);
            if (producto == null) return NotFound();

            var carrito = HttpContext.Session.GetObject<List<DetalleVenta>>("Carrito") ?? new List<DetalleVenta>();

            var item = carrito.FirstOrDefault(c => c.IdProducto == idProducto);
            if (item != null)
            {
                item.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new DetalleVenta
                {
                    IdProducto = producto.IdProducto,
                    Cantidad = cantidad,
                    PrecioUnitario = producto.Precio
                });
            }

            HttpContext.Session.SetObject("Carrito", carrito);

            return RedirectToAction("Carrito");
        }

        public IActionResult Carrito()
        {
            var carrito = HttpContext.Session.GetObject<List<DetalleVenta>>("Carrito") ?? new List<DetalleVenta>();
            return View(carrito);
        }

        [HttpPost]
        public IActionResult ConfirmarCompra()
        {
            var carrito = HttpContext.Session.GetObject<List<DetalleVenta>>("Carrito");

            Console.WriteLine($"Este es el carrito : {carrito.Count}");

            if (carrito == null || !carrito.Any())
                return RedirectToAction("Carrito");

            var venta = new Venta
            {
                IdUsuario = 1,
                IdComprobante = 2,
                Total = carrito.Sum(c => c.PrecioUnitario * c.Cantidad)
            };

            Console.WriteLine($"Este es la venta crea : {venta.Total}");

            bool ok = _ventaService.RegistrarVenta(venta, carrito);

            Console.WriteLine($"Este es el bool de la venta : {ok}");

            if (ok)
            {
                HttpContext.Session.Remove("Carrito");

                var comprobante = _ventaService.GenerarComprobante(venta);

                TempData["Detalles"] = Newtonsoft.Json.JsonConvert.SerializeObject(carrito);
                TempData["Total"] = venta.Total;

                return RedirectToAction("Details", "Comprobante", new { id = comprobante.IdComprobante });
            }

            TempData["Error"] = "No se pudo completar la compra.";
            return RedirectToAction("Carrito");
        }
    }
}
