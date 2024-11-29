using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Clothify_Project.Controllers
{
    public class LoginController : Controller
    {
        private readonly string _connectionString = "Server=MOSTWANTEDTS5;Database=clothify;Trusted_Connection=True;";

        // Display login page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Handle login submission
        [HttpPost]
        public IActionResult Login(string username, string password, string role)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Check username, password, and role
                string sql = "SELECT Role FROM Users WHERE Username = @Username AND Password = @Password";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);

                    var userRole = cmd.ExecuteScalar()?.ToString();

                    if (!string.IsNullOrEmpty(userRole) && userRole == role)
                    {
                        if (role == "Admin")
                        {
                            return RedirectToAction("AdminDashboard", "Admin");
                        }
                        else if (role == "User")
                        {
                            // Redirect to product page for "User" role
                            return RedirectToAction("Index", "Product");
                        }
                    }
                }
            }

            // If invalid login
            ViewBag.Error = "Invalid username, password, or role!";
            return View();
        }

        // Display sign-up page
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        // Handle sign-up submission
        [HttpPost]
        public IActionResult SignUp(string username, string password, string confirmPassword, string role)
        {
            if (password != confirmPassword)
            {
                ViewBag.Error = "Passwords do not match!";
                return View();
            }

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Check if user already exists
                string checkUserSql = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
                using (var checkCmd = new SqlCommand(checkUserSql, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Username", username);
                    int userCount = (int)checkCmd.ExecuteScalar();

                    if (userCount > 0)
                    {
                        // Display message if user already exists
                        ViewBag.Error = "This user already exists!";
                        return View();
                    }
                }

                // Insert new user into the database (UserId is auto-generated)
                string sql = "INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, @Role)";
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Role", role ?? "User");

                    cmd.ExecuteNonQuery();
                }
            }

            // Redirect to login page after successful sign-up
            return RedirectToAction("Login");
        }
    }
}
