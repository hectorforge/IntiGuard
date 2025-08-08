using IntiGuard.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntiGuard.Controllers
{
    public class DetalleVentaController : Controller
    {
        private readonly IDetalleVentaCrud _detalleCrud;

        public DetalleVentaController(IDetalleVentaCrud detalleCrud)
        {
            _detalleCrud = detalleCrud;
        }

        public IActionResult Venta(int id)
        {
            var detalles = _detalleCrud.GetByVentaId(id);
            ViewBag.IdVenta = id;
            return View(detalles);
        }

    }
}
