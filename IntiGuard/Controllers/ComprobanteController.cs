using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntiGuard.Controllers
{
    public class ComprobanteController : Controller
    {
        private readonly ICrud<Comprobante> _comprobanteCrud;

        public ComprobanteController(ICrud<Comprobante> comprobanteCrud)
        {
            _comprobanteCrud = comprobanteCrud;
        }

        public IActionResult Index()
        {
            var comprobantes = _comprobanteCrud.GetAll();
            return View(comprobantes);
        }

        public IActionResult Details(int id)
        {
            var comprobante = _comprobanteCrud.GetById(id);
            if (comprobante == null) return NotFound();

            if (TempData["Detalles"] != null)
            {
                var detalles = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DetalleVenta>>(TempData["Detalles"].ToString());
                ViewBag.Detalles = detalles;
                ViewBag.Total = TempData["Total"];
            }

            return View(comprobante);
        }

    }
}
