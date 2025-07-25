using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using IntiGuard.Models; 
using System.Collections.Generic;
using System.Threading.Tasks;
using static IntiGuard.Controllers.CartController;

namespace IntiGuard.Controllers
{
    public class OrderController : Controller
    {
        private readonly string _connectionString = "server=LAPTOP-U1JQH077-I9; database=TIENDAVIRTUAL;" +
                                                       "Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=false;Encrypt=false";

        // This helper is similar to the one in CartController but returns richer data for checkout
        private async Task<List<ProductInCartViewModel>> GetCartItemsWithProductDetails(int id_usuario)
        {
            List<ProductInCartViewModel> items = new List<ProductInCartViewModel>();
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();
                string query = @"
                    SELECT c.id_carrito, c.id_producto, c.cantidad, p.nombre_producto, p.precio, p.imagen_url, p.stock
                    FROM carrito c
                    INNER JOIN producto p ON c.id_producto = p.id_producto
                    WHERE c.id_usuario = @id_usuario;";
                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    cmd.Parameters.AddWithValue("@id_usuario", id_usuario);
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            items.Add(new ProductInCartViewModel
                            {
                                CartItemId = dr.GetInt32(dr.GetOrdinal("id_carrito")),
                                ProductId = dr.GetInt32(dr.GetOrdinal("id_producto")),
                                ProductName = dr.GetString(dr.GetOrdinal("nombre_producto")),
                                Price = dr.GetDecimal(dr.GetOrdinal("precio")),
                                Quantity = dr.GetInt32(dr.GetOrdinal("cantidad")),
                                ImageUrl = dr.IsDBNull(dr.GetOrdinal("imagen_url")) ? null : dr.GetString(dr.GetOrdinal("imagen_url")),
                                Subtotal = dr.GetDecimal(dr.GetOrdinal("precio")) * dr.GetInt32(dr.GetOrdinal("cantidad"))
                                // You might want to also return 'stock' to check availability
                            });
                        }
                    }
                }
            }
            return items;
        }
        // Action to display the checkout page
        public async Task<IActionResult> Checkout()
        {
            // You'll need to get the current user's ID from authentication/session
            int currentUserId = 2; // Placeholder
            var cartItems = await GetCartItemsWithProductDetails(currentUserId);
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Tu carrito está vacío. Agrega productos para proceder al pago.";
                return RedirectToAction("Index", "Cart");
            }
            // Check if stock is sufficient for each item
            foreach (var item in cartItems)
            {
                Producto? product = null;
                using (SqlConnection cn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_leer_producto_por_id", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_producto", item.ProductId);
                        await cn.OpenAsync();
                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync())
                            {
                                product = new Producto
                                {
                                    stock = dr.GetInt32(dr.GetOrdinal("stock"))
                                };
                            }
                        }
                    }
                }

                if (product == null || item.Quantity > product.stock)
                {
                    TempData["ErrorMessage"] = $"El producto '{item.ProductName}' no tiene suficiente stock disponible. Stock actual: {product?.stock ?? 0}. Cantidad en carrito: {item.Quantity}.";
                    return RedirectToAction("Index", "Cart"); // Redirect back to cart to adjust quantities
                }
            }
            ViewBag.TotalOrder = cartItems.Sum(item => item.Subtotal);
            // Fetch available comprobante types (Boleta, Factura)
            List<Comprobante> comprobantes = new List<Comprobante>();
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT id_comprobante, tipo_comprobante FROM comprobante", cn))
                {
                    await cn.OpenAsync();
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            comprobantes.Add(new Comprobante
                            {
                                id_comprobante = dr.GetInt32(dr.GetOrdinal("id_comprobante")),
                                tipo_comprobante = dr.GetString(dr.GetOrdinal("tipo_comprobante"))
                            });
                        }
                    }
                }
            }
            ViewBag.Comprobantes = new Microsoft.AspNetCore.Mvc.Rendering.SelectList(comprobantes, "id_comprobante", "tipo_comprobante");
            return View(cartItems);
        }
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(int id_comprobante)
        {
            // You'll need to get the current user's ID from authentication/session
            int currentUserId = 2; // Placeholder
            var cartItems = await GetCartItemsWithProductDetails(currentUserId);
            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["ErrorMessage"] = "Tu carrito está vacío. No se puede procesar la orden.";
                return RedirectToAction("Index", "Cart");
            }
            // Prepare the DataTable for the TVP (DetalleVentaType)
            DataTable detallesVentaTable = new DataTable();
            detallesVentaTable.Columns.Add("id_producto", typeof(int));
            detallesVentaTable.Columns.Add("cantidad", typeof(int));
            detallesVentaTable.Columns.Add("precio_unitario", typeof(decimal));
            decimal totalOrder = 0;
            foreach (var item in cartItems)
            {
                // Re-validate stock before final purchase to prevent race conditions
                Producto? product = null;
                using (SqlConnection cn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_leer_producto_por_id", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_producto", item.ProductId);
                        await cn.OpenAsync();
                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync())
                            {
                                product = new Producto
                                {
                                    stock = dr.GetInt32(dr.GetOrdinal("stock"))
                                };
                            }
                        }
                    }
                }
                if (product == null || item.Quantity > product.stock)
                {
                    TempData["ErrorMessage"] = $"El producto '{item.ProductName}' no tiene suficiente stock disponible. Por favor, ajuste su carrito.";
                    return RedirectToAction("Index", "Cart");
                }
                detallesVentaTable.Rows.Add(item.ProductId, item.Quantity, item.Price);
                totalOrder += item.Subtotal;
            }
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_crear_venta", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", currentUserId);
                    cmd.Parameters.AddWithValue("@id_comprobante", id_comprobante);
                    cmd.Parameters.AddWithValue("@total", totalOrder);
                    // Pass the DataTable as a Table-Valued Parameter
                    SqlParameter tvpParam = cmd.Parameters.AddWithValue("@detalles", detallesVentaTable);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    tvpParam.TypeName = "dbo.DetalleVentaType"; // Important: Match the SQL UDT name
                    // Output parameter for the generated venta ID
                    SqlParameter idVentaParam = new SqlParameter("@id_venta_generado", SqlDbType.Int);
                    idVentaParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(idVentaParam);
                    try
                    {
                        await cn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        int newVentaId = (int)idVentaParam.Value;
                        TempData["SuccessMessage"] = $"¡Su compra ha sido realizada con éxito! Número de Venta: {newVentaId}";
                        return RedirectToAction("OrderConfirmation", new { id = newVentaId });
                    }
                    catch (SqlException ex)
                    {
                        // Log the exception details for debugging
                        Console.WriteLine($"Error al crear la venta: {ex.Message}");
                        ModelState.AddModelError("", "Ocurrió un error al procesar su pedido. Por favor, intente de nuevo.");
                        // Potentially re-fetch cart items and re-render checkout view with error
                        return RedirectToAction("Checkout");
                    }
                }
            }
        }
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            // Fetch sale details and associated products for confirmation page
            Venta? venta = null;
            List<DetalleVenta> detalles = new List<DetalleVenta>();
            Usuario? user = null;
            Comprobante? comprobante = null;
            Dictionary<int, Producto> productDetails = new Dictionary<int, Producto>(); // To store product names etc.
            using (SqlConnection cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();
                // Get Venta details
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM venta WHERE id_venta = @id_venta", cn))
                {
                    cmd.Parameters.AddWithValue("@id_venta", id);
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            venta = new Venta
                            {
                                id_venta = dr.GetInt32(dr.GetOrdinal("id_venta")),
                                id_usuario = dr.GetInt32(dr.GetOrdinal("id_usuario")),
                                id_comprobante = dr.GetInt32(dr.GetOrdinal("id_comprobante")),
                                total = dr.GetDecimal(dr.GetOrdinal("total")),
                                fecha_venta = dr.GetDateTime(dr.GetOrdinal("fecha_venta"))
                            };
                        }
                    }
                }
                if (venta == null) return NotFound();
                // Get User details
                using (SqlCommand cmd = new SqlCommand("sp_leer_usuario_por_id", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id_usuario", venta.id_usuario);
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            user = new Usuario
                            {
                                id_usuario = dr.GetInt32(dr.GetOrdinal("id_usuario")),
                                nombres = dr.GetString(dr.GetOrdinal("nombres")),
                                apellidos = dr.GetString(dr.GetOrdinal("apellidos")),
                                correo = dr.GetString(dr.GetOrdinal("correo"))
                            };
                        }
                    }
                }
                // Get Comprobante details
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM comprobante WHERE id_comprobante = @id_comprobante", cn))
                {
                    cmd.Parameters.AddWithValue("@id_comprobante", venta.id_comprobante);
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            comprobante = new Comprobante
                            {
                                id_comprobante = dr.GetInt32(dr.GetOrdinal("id_comprobante")),
                                tipo_comprobante = dr.GetString(dr.GetOrdinal("tipo_comprobante")),
                                numero_comprobante = dr.GetString(dr.GetOrdinal("numero_comprobante"))
                            };
                        }
                    }
                }
                // Get DetalleVenta details
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM detalle_venta WHERE id_venta = @id_venta", cn))
                {
                    cmd.Parameters.AddWithValue("@id_venta", id);
                    using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                    {
                        while (await dr.ReadAsync())
                        {
                            var detalle = new DetalleVenta
                            {
                                id_detalle_venta = dr.GetInt32(dr.GetOrdinal("id_detalle_venta")),
                                id_venta = dr.GetInt32(dr.GetOrdinal("id_venta")),
                                id_producto = dr.GetInt32(dr.GetOrdinal("id_producto")),
                                cantidad = dr.GetInt32(dr.GetOrdinal("cantidad")),
                                precio_unitario = dr.GetDecimal(dr.GetOrdinal("precio_unitario"))
                            };
                            detalles.Add(detalle);
                            // Store product IDs to fetch their details later
                            if (!productDetails.ContainsKey(detalle.id_producto))
                            {
                                productDetails.Add(detalle.id_producto, new Producto()); // null Placeholder
                            }
                        }
                    }
                }
                // Get Product details for each item in detalle_venta
                foreach (int productId in productDetails.Keys.ToList()) // Use ToList() to avoid modification during iteration
                {
                    using (SqlCommand cmd = new SqlCommand("sp_leer_producto_por_id", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id_producto", productId);
                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            if (await dr.ReadAsync())
                            {
                                productDetails[productId] = new Producto
                                {
                                    id_producto = dr.GetInt32(dr.GetOrdinal("id_producto")),
                                    nombre_producto = dr.GetString(dr.GetOrdinal("nombre_producto")),
                                    imagen_url = dr.IsDBNull(dr.GetOrdinal("imagen_url")) ? null : dr.GetString(dr.GetOrdinal("imagen_url"))
                                };
                            }
                        }
                    }
                }
            }
            // Create a ViewModel for the confirmation page
            var confirmationVm = new OrderConfirmationViewModel
            {
                Venta = venta,
                Usuario = user,
                Comprobante = comprobante,
                DetallesVenta = detalles,
                ProductDetails = productDetails
            };
            return View(confirmationVm);
        }
        public class OrderConfirmationViewModel
        {
            public Venta? Venta { get; set; }
            public Usuario? Usuario { get; set; }
            public Comprobante? Comprobante { get; set; }
            public List<DetalleVenta>? DetallesVenta { get; set; }
            // Update the line causing CS8625 by explicitly specifying a nullable type for the dictionary value.    
            public Dictionary<int, Producto>? ProductDetails { get; set; }
        }
    }
} 
