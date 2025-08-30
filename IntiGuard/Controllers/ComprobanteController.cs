using IntiGuard.Repositories.Interfaces;
using IntiGuard.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IntiGuard.Controllers
{
    public class ComprobanteController : Controller
    {
        private readonly IComprobanteService _comprobanteService;
        private readonly IProductoCrud _productoCrud;

        public ComprobanteController(IComprobanteService comprobanteService, IProductoCrud productoCrud)
        {
            _comprobanteService = comprobanteService;
            _productoCrud = productoCrud;
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

            if (detalles != null && detalles.Any())
            {
                foreach (var d in detalles)
                {
                    var producto = _productoCrud.GetById(d.IdProducto);
                    if (producto != null)
                    {
                        d.NombreProducto = producto.NombreProducto;
                        d.ImagenUrl = producto.ImagenUrl;
                    }
                }
            }

            if (TempData["Total"] != null)
            {
                ViewBag.Total = decimal.Parse(TempData["Total"].ToString());
            }

            ViewBag.Detalles = detalles;

            return View(comprobante);
        }


    }
}
