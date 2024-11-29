using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace E_commerce_store_new_.Pages
{
    public class Products2Model : PageModel
    {
        public List<productInfo> products = new List<productInfo>();
        public void OnGet()
        {
            // OnGetMostSelling();

            try
            {
                string connectionString = "Data Source=DELL3501SSD\\SQLEXPRESS;Initial Catalog=store(ecommerce);Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    string sqlQ = "SELECT * FROM Product";

                    using (SqlCommand command = new SqlCommand(sqlQ, sqlConnection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                productInfo product = new productInfo();

                                product.pid = reader.GetInt32(0);
                                product.name = reader.GetString(1);
                                product.description = reader.GetString(2);
                                product.imageURL = reader.GetString(3);
                                product.cat = reader.GetInt32(4);
                                product.price = reader.GetDecimal(5);
                                product.stock = reader.GetInt32(6);

                                products.Add(product);
                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception :" + ex.ToString());
            }
        }

        public void OnPost()
        {
            try
            {
                string connectionString = "Data Source=DELL3501SSD\\SQLEXPRESS;Initial Catalog=store(ecommerce);Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

                    string searchTerm = Request.Form["searchTerm"];
                    //string sqlQ = "SELECT * FROM Product where ProductName like '%" + searchTerm + "%' or Description like '%" + searchTerm + "%'";
                    string sqlQ = "DECLARE @variable VARCHAR(20) = '" + searchTerm + "';" +
              "EXECUTE SearchProduct @searchTerm = @variable;";






                    using (SqlCommand command = new SqlCommand(sqlQ, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@searchTerm", "%" + searchTerm + "%");
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                productInfo product = new productInfo();

                                product.pid = reader.GetInt32(0);
                                product.name = reader.GetString(1);
                                product.description = reader.GetString(2);
                                product.imageURL = reader.GetString(3);
                                product.cat = reader.GetInt32(4);
                                product.price = reader.GetDecimal(5);
                                product.stock = reader.GetInt32(6);

                                products.Add(product);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception :" + ex.ToString());
            }
        }

        public void OnPostAddToCart(int productId)
        {



            // Insert the product into the cart database
            var connectionString = "Data Source=DELL3501SSD\\SQLEXPRESS;Initial Catalog=store(ecommerce);Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DateTime now = DateTime.Now;

                var command = new SqlCommand("INSERT INTO Cart ( ProductId, Quantity,DateAdded) VALUES (@ProductId, @Quantity, @DateAdded)", connection);

                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@Quantity", 1);
                command.Parameters.AddWithValue("@DateAdded", now);
                command.ExecuteNonQuery();
            }

            RedirectToPage("/Product");
        }


       


    }

}
