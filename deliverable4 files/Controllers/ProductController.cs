using Clothify_Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Clothify_Project.Controllers
{
    public class ProductController : Controller
    {
        private string connectionString = "Server=MOSTWANTEDTS5;Database=clothify;Trusted_Connection=True;";

        // Action to display all products
        public ActionResult Index(string category = null)
        {
            List<Product> products = new List<Product>();
            string query = "SELECT p.ProductID, p.ProductName, p.Description, p.ImageURL, p.Price, p.StockCount, p.AverageRating, c.CategoryName " +
                           "FROM Product p " +
                           "JOIN Category c ON p.CategoryID = c.CategoryID";

            if (!string.IsNullOrEmpty(category))
            {
                query += " WHERE c.CategoryName = @CategoryName";
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                if (!string.IsNullOrEmpty(category))
                {
                    cmd.Parameters.AddWithValue("@CategoryName", category);
                }

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Product product = new Product()
                    {
                        ProductID = reader.GetInt32(0),
                        ProductName = reader.GetString(1),
                        Description = reader.GetString(2),
                        ImageURL = reader.GetString(3),
                        Price = reader.GetDecimal(4),
                        StockCount = reader.GetInt32(5),
                        AverageRating = reader.GetDecimal(6),
                        CategoryName = reader.GetString(7)
                    };
                    products.Add(product);
                }
            }

            return View(products);
        }

        // Action to display a single product's details
        public ActionResult Details(int id)
        {
            Product product = null;
            string query = "SELECT p.ProductID, p.ProductName, p.Description, p.ImageURL, p.Price, p.StockCount, p.AverageRating, c.CategoryName " +
                           "FROM Product p " +
                           "JOIN Category c ON p.CategoryID = c.CategoryID " +
                           "WHERE p.ProductID = @ProductID";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@ProductID", id);

                connection.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    product = new Product()
                    {
                        ProductID = reader.GetInt32(0),
                        ProductName = reader.GetString(1),
                        Description = reader.GetString(2),
                        ImageURL = reader.GetString(3),
                        Price = reader.GetDecimal(4),
                        StockCount = reader.GetInt32(5),
                        AverageRating = reader.GetDecimal(6),
                        CategoryName = reader.GetString(7)
                    };
                }
            }

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
