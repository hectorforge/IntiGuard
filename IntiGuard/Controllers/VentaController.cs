using IntiGuard.Helpers;
using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using IntiGuard.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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

            ViewBag.Productos = _productoCrud.GetAll();

            return View(carrito);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarCompra()
        {
            var carrito = HttpContext.Session.GetObject<List<DetalleVenta>>("Carrito");

            if (carrito == null || !carrito.Any())
                return RedirectToAction("Carrito");

            var claimId = User.FindFirst("Id")?.Value;
            if (!int.TryParse(claimId, out int idUsuario))
            {
                TempData["Error"] = "Usuario no autenticado correctamente.";
                return RedirectToAction("Carrito");
            }

            var venta = new Venta
            {
                IdUsuario = idUsuario,
                IdComprobante = 2,
                Total = carrito.Sum(c => c.PrecioUnitario * c.Cantidad)
            };

            bool ok = _ventaService.RegistrarVenta(venta, carrito);

            if (ok)
            {
                HttpContext.Session.Remove("Carrito");

                var comprobante = _ventaService.GenerarComprobante(venta);

                TempData["Detalles"] = Newtonsoft.Json.JsonConvert.SerializeObject(carrito);
                TempData["Total"] = venta.Total.ToString(CultureInfo.InvariantCulture);

                return RedirectToAction("Details", "Comprobante", new { id = comprobante.IdComprobante });
            }

            TempData["Error"] = "No se pudo completar la compra.";
            return RedirectToAction("Carrito");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarCantidad(int idProducto)
        {
            var qtyStr = Request.Form["cantidad"].FirstOrDefault();
            if (!int.TryParse(qtyStr?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int cantidad) || cantidad < 1)
            {
                cantidad = 1;
            }

            var carrito = HttpContext.Session.GetObject<List<DetalleVenta>>("Carrito") ?? new List<DetalleVenta>();
            var item = carrito.FirstOrDefault(c => c.IdProducto == idProducto);
            if (item != null)
            {
                var producto = _productoCrud.GetById(idProducto);
                if (producto != null && producto.Stock < cantidad)
                {
                    cantidad = producto.Stock;
                    TempData["Error"] = $"Stock insuficiente para {producto.NombreProducto}. Se ajustó la cantidad a {cantidad}.";
                }

                item.Cantidad = cantidad;
                HttpContext.Session.SetObject("Carrito", carrito);
            }

            return RedirectToAction("Carrito");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarDelCarrito(int idProducto)
        {
            var carrito = HttpContext.Session.GetObject<List<DetalleVenta>>("Carrito") ?? new List<DetalleVenta>();

            var removed = carrito.RemoveAll(c => c.IdProducto == idProducto);
            if (removed > 0)
            {
                HttpContext.Session.SetObject("Carrito", carrito);
            }

            return RedirectToAction("Carrito");
        }


    }
}
