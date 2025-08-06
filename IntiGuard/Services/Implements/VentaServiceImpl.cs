using IntiGuard.Models;
using IntiGuard.Repositories.Interfaces;
using IntiGuard.Services.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace IntiGuard.Services.Implements
{
    public class VentaServiceImpl : IVentaService
    {
        private readonly string _connectionString;
        private readonly IVentaCrud _ventaCrud;
        private readonly IProductoCrud _productoCrud;
        private readonly IDetalleVentaCrud _detalleCrud;

        public VentaServiceImpl(
            IConfiguration config,
            IVentaCrud ventaCrud,
            IProductoCrud productoCrud,
            IDetalleVentaCrud detalleCrud)
        {
            _connectionString = config.GetConnectionString("IntiGuardDB");
            _ventaCrud = ventaCrud;
            _productoCrud = productoCrud;
            _detalleCrud = detalleCrud;
        }

        public bool RegistrarVenta(Venta venta, IEnumerable<DetalleVenta> detalles)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var transaction = conn.BeginTransaction();
            try
            {
                int idVenta = _ventaCrud.InsertVentaTransaccion(venta, conn, transaction);
                foreach (var detalle in detalles)
                {
                    detalle.IdVenta = idVenta;
                    _detalleCrud.InsertDetalleVentaTransaccion(detalle, conn, transaction);
                    _productoCrud.DescontarStockTransaccion(detalle.IdProducto, detalle.Cantidad, conn, transaction);
                }
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}