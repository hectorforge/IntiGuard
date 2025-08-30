using IntiGuard.Models;

namespace IntiGuard.ViewModel
{
    public class ProductoListViewModel
    {
        public IEnumerable<Producto> Productos { get; set; }
        public int PaginaActual { get; set; }
        public int TotalPaginas { get; set; }
        public bool EsAdmin { get; set; }
    }
}
