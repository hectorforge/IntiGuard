using IntiGuard.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntiGuard.Controllers
{
    public class ComprobanteController : Controller
    {
        private readonly IComprobanteService _comprobanteService;

        public ComprobanteController(IComprobanteService comprobanteService)
        {
            _comprobanteService = comprobanteService;
        }

        public IActionResult Index()
        {
            var comprobantes = _comprobanteService.GetAll();
            return View(comprobantes);
        }

        public IActionResult Details(int id)
        {
            var (comprobante, total, detalles) = _comprobanteService.GetDetails(id, TempData["Detalles"]);
            if (comprobante == null) return NotFound();

            ViewBag.Total = total;
            ViewBag.Detalles = detalles;

            return View(comprobante);
        }
    }
}
