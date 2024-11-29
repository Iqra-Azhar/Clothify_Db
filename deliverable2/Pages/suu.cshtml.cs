using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace E_commerce_store_new_.Pages
{
    public class suuModel : PageModel
    {
        public void OnGet()
        {
        }
        public void OnPost()
        {
           
                // Get the form data
                string userRole = Request.Form["userrole"];
                string username = Request.Form["username"];
                string password = Request.Form["password"];
            try
            {
                // Set up the connection string
                string connectionString = "Data Source=DELL3501SSD\\SQLEXPRESS;Initial Catalog=store(ecommerce);Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";



                // Set up the SQL connection and command objects
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Create the SQL query to insert the data into the database
                    //string query = "INSERT INTO UserAccounts (UserRole, UserName, Password) VALUES (@UserRole, @UserName, @Password)";
                    string query = "declare @role varchar(50) = '" + userRole + "';"+ "declare @username varchar(50) = '" + username + "';" + "declare @password varchar(50) = '" + password + "';" + " execute InsertUserAccount @UserRole = @role, @UserName = @username, @Password = @password;";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Add the parameters to the command object
                        //command.Parameters.AddWithValue("@UserRole", userRole);
                      //  command.Parameters.AddWithValue("@UserName", username);
                      //  command.Parameters.AddWithValue("@Password", password);

                        // Open the connection and execute the command

                        command.ExecuteNonQuery();
                    }
                }

                // Redirect the user to the home page after sign-up
                Response.Redirect("/Login");

            }
            catch (Exception ex)
            {
                // Handle the exception here, e.g. log the error or display a message to the user
            }

        }


    }
}
