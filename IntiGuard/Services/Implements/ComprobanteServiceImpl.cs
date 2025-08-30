using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using IntiGuard.Services.Interfaces;

namespace IntiGuard.Services.Implements
{
    public class ComprobanteServiceImpl : IComprobanteService
    {
        private readonly ICrud<Comprobante> _comprobanteCrud;
        private readonly IVentaCrud _ventaCrud;
        private readonly IDetalleVentaCrud _detalleVentaCrud;
        private readonly IProductoCrud _productoCrud;

        public ComprobanteServiceImpl(
            ICrud<Comprobante> comprobanteCrud,
            IVentaCrud ventaCrud,
            IDetalleVentaCrud detalleVentaCrud,
            IProductoCrud productoCrud)
        {
            _comprobanteCrud = comprobanteCrud;
            _ventaCrud = ventaCrud;
            _detalleVentaCrud = detalleVentaCrud;
            _productoCrud = productoCrud;
        }

        public IEnumerable<Comprobante> GetAll()
        {
            return _comprobanteCrud.GetAll();
        }

        public Comprobante GetById(int id)
        {
            return _comprobanteCrud.GetById(id);
        }

        public (Comprobante comprobante, decimal? total, IEnumerable<DetalleVenta>? detalles) GetDetails(int id, object? tempData)
        {
            var comprobante = _comprobanteCrud.GetById(id);
            if (comprobante == null)
                throw new Exception("Comprobante no encontrado");

            var venta = _ventaCrud.GetAll().FirstOrDefault(v => v.IdVenta == id);
            var total = venta?.Total;

            IEnumerable<DetalleVenta>? detalles = null;
            if (tempData != null)
            {
                detalles = Newtonsoft.Json.JsonConvert
                            .DeserializeObject<List<DetalleVenta>>(tempData.ToString()!);
            }

            return (comprobante, total, detalles);
        }

        public (Comprobante comprobante, decimal? total, IEnumerable<DetalleVenta>? detalles) GetDetailsAdmin(int id)
        {
            var comprobante = _comprobanteCrud.GetById(id);
            if (comprobante == null)
                throw new Exception("Comprobante no encontrado");

            var venta = _ventaCrud.GetAll().FirstOrDefault(v => v.IdVenta == id);
            if (venta == null)
                throw new Exception("Venta no encontrada");

            var detalles = _detalleVentaCrud.GetByVentaId(venta.IdVenta)?.ToList();

            if (detalles != null && detalles.Any())
            {
                foreach (var d in detalles)
                {
                    var producto = _productoCrud.GetById(d.IdProducto);
                    if (producto != null)
                    {
                        d.NombreProducto = producto.NombreProducto;
                    }
                }
            }

            decimal? total = detalles?.Any() == true
                ? detalles.Sum(d => d.Cantidad * d.PrecioUnitario)
                : venta?.Total;

            return (comprobante, total, detalles);
        }
    }
}
