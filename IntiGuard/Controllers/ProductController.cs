using IntiGuard.Models; 
using IntiGuard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace IntiGuard.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductoService _productoService;

        public ProductController(IProductoService productoService)
        {
            _productoService = productoService;
        }
    }
}
