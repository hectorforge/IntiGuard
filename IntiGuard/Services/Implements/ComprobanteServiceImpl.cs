using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using IntiGuard.Services.Interfaces;

namespace IntiGuard.Services.Implements
{
    public class ComprobanteServiceImpl : IComprobanteService
    {
        private readonly ICrud<Comprobante> _comprobanteCrud;
        private readonly IVentaCrud _ventaCrud;

        public ComprobanteServiceImpl(ICrud<Comprobante> comprobanteCrud, IVentaCrud ventaCrud)
        {
            _comprobanteCrud = comprobanteCrud;
            _ventaCrud = ventaCrud;
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
    }
}
