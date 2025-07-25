using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using IntiGuard.Models; 
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntiGuard.Controllers
{
    public class ProductController : Controller
    {
        private readonly string _connectionString = "server=LAPTOP-U1JQH077-I9; database=TIENDAVIRTUAL;" +
                                                        "Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=false;Encrypt=false";

        // Helper to get all products
        private async Task<IEnumerable<Producto>> GetAllProductsAsync()
        {
            List<Producto> products = new List<Producto>();
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM producto", cn)) // Or a sp_leer_productos
                {
                    await cn.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            products.Add(new Producto
                            {
                                id_producto = dr.GetInt32(dr.GetOrdinal("id_producto")),
                                nombre_producto = dr.GetString(dr.GetOrdinal("nombre_producto")),
                                descripcion = dr.IsDBNull(dr.GetOrdinal("descripcion")) ? null : dr.GetString(dr.GetOrdinal("descripcion")),
                                marca = dr.IsDBNull(dr.GetOrdinal("marca")) ? null : dr.GetString(dr.GetOrdinal("marca")),
                                precio = dr.GetDecimal(dr.GetOrdinal("precio")),
                                stock = dr.GetInt32(dr.GetOrdinal("stock")),
                                imagen_url = dr.IsDBNull(dr.GetOrdinal("imagen_url")) ? null : dr.GetString(dr.GetOrdinal("imagen_url"))
                            });
                        }
                    }
                }
            }
            return products;
        }

        // Action to display all products
        public async Task<IActionResult> Index()
        {
            var products = await GetAllProductsAsync();
            return View(products);
        }

        // Action to display product details by ID
        public async Task<IActionResult> Details(int id)
        {
            Producto? product = null;
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_leer_producto_por_id", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_producto", id);
                    await cn.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            product = new Producto
                            {
                                id_producto = dr.GetInt32(dr.GetOrdinal("id_producto")),
                                nombre_producto = dr.GetString(dr.GetOrdinal("nombre_producto")),
                                descripcion = dr.IsDBNull(dr.GetOrdinal("descripcion")) ? null : dr.GetString(dr.GetOrdinal("descripcion")),
                                marca = dr.IsDBNull(dr.GetOrdinal("marca")) ? null : dr.GetString(dr.GetOrdinal("marca")),
                                precio = dr.GetDecimal(dr.GetOrdinal("precio")),
                                stock = dr.GetInt32(dr.GetOrdinal("stock")),
                                imagen_url = dr.IsDBNull(dr.GetOrdinal("imagen_url")) ? null : dr.GetString(dr.GetOrdinal("imagen_url"))
                            };
                        }
                    }
                }
            }

            if (product == null)
            {
                return NotFound(); // Product not found
            }
            return View(product);
        }

        // Action for product search
        public async Task<IActionResult> Search(string searchTerm)
        {
            List<Producto> products = new List<Producto>();
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // If search term is empty, return all products or an empty list
                return View("Index", await GetAllProductsAsync());
            }

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_buscar_productos", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@termino_busqueda", searchTerm);
                    await cn.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            products.Add(new Producto
                            {
                                id_producto = dr.GetInt32(dr.GetOrdinal("id_producto")),
                                nombre_producto = dr.GetString(dr.GetOrdinal("nombre_producto")),
                                descripcion = dr.IsDBNull(dr.GetOrdinal("descripcion")) ? null : dr.GetString(dr.GetOrdinal("descripcion")),
                                marca = dr.IsDBNull(dr.GetOrdinal("marca")) ? null : dr.GetString(dr.GetOrdinal("marca")),
                                precio = dr.GetDecimal(dr.GetOrdinal("precio")),
                                stock = dr.GetInt32(dr.GetOrdinal("stock")),
                                imagen_url = dr.IsDBNull(dr.GetOrdinal("imagen_url")) ? null : dr.GetString(dr.GetOrdinal("imagen_url"))
                            });
                        }
                    }
                }
            }
            return View("Index", products); // Display search results using the Index view
        }
    }
}
