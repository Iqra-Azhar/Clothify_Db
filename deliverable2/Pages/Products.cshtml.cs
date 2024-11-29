using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;
using System.Globalization;

namespace E_commerce_store_new_.Pages
{
    public class ProductsModel : PageModel
    {
        public List<productInfo> products = new List<productInfo>();
        public List<productInfo> productsMost = new List<productInfo>();
        public string SearchTerm { get; set; }
        public string category { get; set; }

        public void OnGetMostSelling()
        {
            try
            {
                string connectionString = "Data Source=DELL3501SSD\\SQLEXPRESS;Initial Catalog=store(ecommerce);Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    string sqlQ = "select P1.ProductID,P1.ProductName,P1.[Description],P1.ImageURL,P1.CategoryID,P1.Price,P1.StockCount from Product  as P1 inner join (select  top 5 ProductID from  OrderItem group by ProductID order by sum(quantity) desc) as most_selling  on  P1.ProductID = most_selling.ProductID";

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

        
        public void OnPostCategories()
        {
            try
            {
                string connectionString = "Data Source=DELL3501SSD\\SQLEXPRESS;Initial Catalog=store(ecommerce);Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();

     
                    string category = Request.Form["categories"];
                    //string sqlQ = "select * from Product p join Category c on p.CategoryID = c.CategoryID where c.CategoryName   = @Category ";
                    string sqlQ = "DECLARE @v2 VARCHAR(20) = '" + category + "';" +
                    "EXECUTE SelectProductsByCategory2 @category = @v2;";



                    using (SqlCommand command = new SqlCommand(sqlQ, sqlConnection))
                    {
                        command.Parameters.AddWithValue("@category", "%" + category + "%");
                       // command.Parameters.AddWithValue("@Category", category );
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

    public class productInfo
    {
        public int pid;
        public string name;
        public string description;
        public string imageURL;
        public int cat;
        public decimal price;
        public int stock;
    }

    public class CartItem
    {
        public int CartItemId { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

}
