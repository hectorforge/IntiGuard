using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using IntiGuard.Models; 
using System.Collections.Generic;
using System.Threading.Tasks;
namespace IntiGuard.Controllers
{
    public class CartController : Controller
    {
        private readonly string _connectionString = "server=LAPTOP-U1JQH05L; database=TIENDAVIRTUAL;" +
                                                        "Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=false;Encrypt=false";

        // Helper to get cart items for a user
        private async Task<List<Carrito>> GetUserCartItemsAsync(int id_usuario)
        {
            List<Carrito> cartItems = new List<Carrito>();
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                // You might want a stored procedure like sp_leer_carrito_por_usuario
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM carrito WHERE id_usuario = @id_usuario", cn))
                {
                    cmd.Parameters.AddWithValue("@id_usuario", id_usuario);
                    await cn.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            cartItems.Add(new Carrito
                            {
                                id_carrito = dr.GetInt32(dr.GetOrdinal("id_carrito")),
                                id_usuario = dr.GetInt32(dr.GetOrdinal("id_usuario")),
                                id_producto = dr.GetInt32(dr.GetOrdinal("id_producto")),
                                cantidad = dr.GetInt32(dr.GetOrdinal("cantidad"))
                            });
                        }
                    }
                }
            }
            return cartItems;
        }

        public async Task<IActionResult> Index()
        {
            // You'll need to get the current user's ID from authentication/session
            int currentUserId = 2; // Placeholder: Replace with actual user ID from session/authentication
            var cartItems = await GetUserCartItemsAsync(currentUserId);

            // Optionally, fetch product details for each item in the cart to display name, image, etc.
            List<ProductInCartViewModel> productsInCart = new List<ProductInCartViewModel>();
            foreach (var item in cartItems)
            {
                Producto? product = null;
                using (SqlConnection cn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_leer_producto_por_id", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_producto", item.id_producto);
                        await cn.OpenAsync();
                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync())
                            {
                                product = new Producto
                                {
                                    id_producto = dr.GetInt32(dr.GetOrdinal("id_producto")),
                                    nombre_producto = dr.GetString(dr.GetOrdinal("nombre_producto")),
                                    precio = dr.GetDecimal(dr.GetOrdinal("precio")),
                                    imagen_url = dr.IsDBNull(dr.GetOrdinal("imagen_url")) ? null : dr.GetString(dr.GetOrdinal("imagen_url"))
                                };
                            }
                        }
                    }
                }
                if (product != null)
                {
                    productsInCart.Add(new ProductInCartViewModel
                    {
                        CartItemId = item.id_carrito,
                        ProductId = product.id_producto,
                        ProductName = product.nombre_producto,
                        Price = product.precio,
                        Quantity = item.cantidad,
                        ImageUrl = product.imagen_url,
                        Subtotal = product.precio * item.cantidad
                    });
                }
            }

            return View(productsInCart);
        }

        // A simple ViewModel to combine product and cart details for display
        public class ProductInCartViewModel
        {
            public int CartItemId { get; set; }
            public int ProductId { get; set; }
            public string? ProductName { get; set; }
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public string? ImageUrl { get; set; }
            public decimal Subtotal { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id_producto, int cantidad = 1)
        {
            // You'll need to get the current user's ID from authentication/session
            int currentUserId = 2; // Placeholder: Replace with actual user ID from session/authentication

            if (cantidad <= 0)
            {
                TempData["ErrorMessage"] = "La cantidad debe ser mayor que cero.";
                return RedirectToAction("Details", "Product", new { id = id_producto });
            }

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();
                // Check if product already exists in cart for this user
                using (SqlCommand checkCmd = new SqlCommand("SELECT id_carrito, cantidad FROM carrito WHERE id_usuario = @id_usuario AND id_producto = @id_producto", cn))
                {
                    checkCmd.Parameters.AddWithValue("@id_usuario", currentUserId);
                    checkCmd.Parameters.AddWithValue("@id_producto", id_producto);

                    using (SqlDataReader dr = await checkCmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            int existingCartItemId = dr.GetInt32(0);
                            int existingQuantity = dr.GetInt32(1);
                            await dr.CloseAsync(); // Close reader before executing another command

                            // Update existing item
                            using (SqlCommand updateCmd = new SqlCommand("UPDATE carrito SET cantidad = @new_cantidad WHERE id_carrito = @id_carrito", cn))
                            {
                                updateCmd.Parameters.AddWithValue("@new_cantidad", existingQuantity + cantidad);
                                updateCmd.Parameters.AddWithValue("@id_carrito", existingCartItemId);
                                await updateCmd.ExecuteNonQueryAsync();
                            }
                            TempData["SuccessMessage"] = $"Cantidad actualizada para el producto en el carrito.";
                        }
                        else
                        {
                            await dr.CloseAsync(); // Close reader before executing another command
                            // Add new item to cart
                            using (SqlCommand insertCmd = new SqlCommand("INSERT INTO carrito (id_usuario, id_producto, cantidad) VALUES (@id_usuario, @id_producto, @cantidad)", cn))
                            {
                                insertCmd.Parameters.AddWithValue("@id_usuario", currentUserId);
                                insertCmd.Parameters.AddWithValue("@id_producto", id_producto);
                                insertCmd.Parameters.AddWithValue("@cantidad", cantidad);
                                await insertCmd.ExecuteNonQueryAsync();
                            }
                            TempData["SuccessMessage"] = "Producto agregado al carrito.";
                        }
                    }
                }
            }
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id_carrito)
        {
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("DELETE FROM carrito WHERE id_carrito = @id_carrito", cn))
                {
                    cmd.Parameters.AddWithValue("@id_carrito", id_carrito);
                    await cn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            TempData["SuccessMessage"] = "Producto eliminado del carrito.";
            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCartQuantity(int id_carrito, int newQuantity)
        {
            if (newQuantity <= 0)
            {
                return await RemoveFromCart(id_carrito); // Remove if quantity is zero or less
            }

            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE carrito SET cantidad = @new_quantity WHERE id_carrito = @id_carrito", cn))
                {
                    cmd.Parameters.AddWithValue("@new_quantity", newQuantity);
                    cmd.Parameters.AddWithValue("@id_carrito", id_carrito);
                    await cn.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
            TempData["SuccessMessage"] = "Cantidad del producto en el carrito actualizada.";
            return RedirectToAction("Index", "Cart");
        }
    }
}
