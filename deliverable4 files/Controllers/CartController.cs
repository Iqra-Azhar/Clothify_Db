using Clothify_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Clothify_Project.Controllers
{
    public class CartController : Controller
    {
        private readonly string connectionString = "Server=MOSTWANTEDTS5;Database=clothify;Trusted_Connection=True";

        // Display "My Cart" page
        public IActionResult MyCart()
        {
            int userId = 1; // Replace with actual logged-in user ID from session or authentication system

            List<CartItem> cartItems = GetCartItemsForUser(userId);

            return View(cartItems);
        }

        // Get Cart Items from the Database for a specific user
        private List<CartItem> GetCartItemsForUser(int userId)
        {
            List<CartItem> cartItems = new List<CartItem>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT c.CartID, p.ProductName, p.ImageURL, p.Price, c.Quantity, (p.Price * c.Quantity) AS TotalPrice
                    FROM Cart c
                    JOIN Product p ON c.ProductID = p.ProductID
                    WHERE c.UserID = @UserID";

                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        cartItems.Add(new CartItem
                        {
                            CartID = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            ImageURL = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            Quantity = reader.GetInt32(4),
                            TotalPrice = reader.GetDecimal(5)
                        });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return cartItems;
        }

        // Remove an item from the cart
        [HttpPost]
        public JsonResult RemoveFromCart(int cartId)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Cart WHERE CartID = @CartID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@CartID", cartId);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true, message = "Item removed from the cart successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Update quantity of a cart item
        [HttpPost]
        public JsonResult UpdateQuantity(int cartId, int quantity)
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Quantity must be greater than 0." });
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Cart SET Quantity = @Quantity WHERE CartID = @CartID";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@CartID", cartId);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true, message = "Quantity updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Add product to cart
        [HttpPost]
        public JsonResult AddToCart(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Quantity must be greater than 0." });
            }

            int userId = 1; // Replace with actual logged-in user ID from session or authentication system

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = @"
                        IF EXISTS (SELECT 1 FROM Cart WHERE ProductID = @ProductID AND UserID = @UserID)
                        BEGIN
                            UPDATE Cart SET Quantity = Quantity + @Quantity WHERE ProductID = @ProductID AND UserID = @UserID
                        END
                        ELSE
                        BEGIN
                            INSERT INTO Cart (ProductID, UserID, Quantity) VALUES (@ProductID, @UserID, @Quantity)
                        END";

                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                return Json(new { success = true, message = "Item added to cart successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Step 2: Display Checkout page with user cart items
        public IActionResult Checkout()
        {
            int userId = 1; // Replace with actual logged-in user ID
            List<CartItem> cartItems = GetCartItemsForUser(userId);

            decimal totalAmount = cartItems.Sum(c => c.TotalPrice);

            var model = new CheckoutViewModel
            {
                CartItems = cartItems,
                TotalAmount = totalAmount
            };

            return View(model); // Pass cart items and total to the Checkout view
        }

        // Step 4: Place the order and update the inventory
        [HttpPost]
        public IActionResult PlaceOrder()
        {
            int userId = 1; // Replace with actual logged-in user ID
            List<CartItem> cartItems = GetCartItemsForUser(userId);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlTransaction transaction = null;
                connection.Open();
                transaction = connection.BeginTransaction();

                try
                {
                    // Insert into Order table
                    string insertOrderQuery = @"
                        INSERT INTO [Orders] (UserID, OrderDate, TotalAmount)
                        VALUES (@UserID, @OrderDate, @TotalAmount);
                        SELECT SCOPE_IDENTITY();";

                    SqlCommand orderCmd = new SqlCommand(insertOrderQuery, connection, transaction);
                    orderCmd.Parameters.AddWithValue("@UserID", userId);
                    orderCmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                    orderCmd.Parameters.AddWithValue("@TotalAmount", cartItems.Sum(c => c.TotalPrice));

                    int orderId = Convert.ToInt32(orderCmd.ExecuteScalar());

                    // Insert Order Details and update Inventory
                    foreach (var item in cartItems)
                    {
                        string insertOrderDetailsQuery = @"
                            INSERT INTO OrderDetails (OrderID, ProductID, Quantity, Price)
                            VALUES (@OrderID, @ProductID, @Quantity, @Price);
                            
                            UPDATE Product
                            SET Stock = Stock - @Quantity
                            WHERE ProductID = @ProductID;";

                        SqlCommand orderDetailsCmd = new SqlCommand(insertOrderDetailsQuery, connection, transaction);
                        orderDetailsCmd.Parameters.AddWithValue("@OrderID", orderId);
                        orderDetailsCmd.Parameters.AddWithValue("@ProductID", item.CartID); // Replace with correct ProductID
                        orderDetailsCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        orderDetailsCmd.Parameters.AddWithValue("@Price", item.Price);

                        orderDetailsCmd.ExecuteNonQuery();
                    }

                    // Commit the transaction
                    transaction.Commit();

                    // Clear the cart after successful order placement
                    string deleteCartQuery = "DELETE FROM Cart WHERE UserID = @UserID";
                    SqlCommand deleteCmd = new SqlCommand(deleteCartQuery, connection, transaction);
                    deleteCmd.Parameters.AddWithValue("@UserID", userId);
                    deleteCmd.ExecuteNonQuery();

                    return RedirectToAction("OrderConfirmation", new { orderId });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = ex.Message });
                }
            }
        }

        // Confirmation page after placing the order
        public IActionResult OrderConfirmation(int orderId)
        {
            // Fetch order details for confirmation if needed
            return View(orderId);
        }
    }
}
