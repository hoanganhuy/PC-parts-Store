using Helpers;
using MySql.Data.MySqlClient;
using PCPartsStore.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPartsStore
{
    public class Validations : IValidations
    {
        public bool AccountExistCheck(string username, string password)
        {
            string[] tableNames = { "Customer", "Employee", "Admin" };
            bool exists = false;
            try
            {
                using (MySqlConnection conn = DBHelper.GetConnection())
                {
                    conn.Open();

                    foreach (string tableName in tableNames)
                    {
                        string query = $"SELECT COUNT(*) FROM {tableName} WHERE username = @username AND password = @password";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@username", username);
                            cmd.Parameters.AddWithValue("@password", password);

                            exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;

                            if (exists)
                            {
                                
                                break; // If account exists in any table, break the loop
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Account is not exist, please enter again: ");
            }

            return exists;
        }
        public bool ProductExistCheck(int productId, MySqlConnection connection)
        {
            string query = "SELECT COUNT(*) FROM product WHERE Product_ID = @productId";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@productId", productId);
                try
                {
                    connection.Open();
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erorr: {ex.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public bool ProductRemainingCheck(int productId)
        {
            // Initialize the connection string
            string connectionString = DBHelper.DefaultConnectionString;

            // Define a query to check if the product with the given ID exists and has remaining quantity
            string query = "SELECT RemainingQuantity FROM Product WHERE ProductId = @productId";

            // Initialize a boolean variable to store the result
            bool productExistsWithRemaining = false;

            // Create and open a connection to the database
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Create a MySqlCommand object with the query and connection
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Add parameters to the query
                    cmd.Parameters.AddWithValue("@productId", productId);

                    // Execute the query and retrieve the remaining quantity
                    object result = cmd.ExecuteScalar();

                    // Check if the result is not null and is greater than 0
                    if (result != null && Convert.ToInt32(result) > 0)
                    {
                        productExistsWithRemaining = true;
                    }
                }
            }

            // Return the result
            return productExistsWithRemaining;
        }

        public bool CustomerUsernameDuplicateCheck(string username)
        {
            // Initialize the connection string
            string connectionString = DBHelper.DefaultConnectionString;

            // Define the query to check for duplicate username
            string query = "SELECT COUNT(*) FROM Customer WHERE Username = @username";

            // Initialize a boolean variable to store the result
            bool usernameExists = false;

            // Create and open a connection to the database
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Create a MySqlCommand object with the query and connection
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Add parameters to the query
                    cmd.Parameters.AddWithValue("@username", username);

                    // Execute the query and check if any rows are returned
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // If any rows are returned, set usernameExists to true
                        if (reader.Read())
                        {
                            usernameExists = reader.GetInt32(0) > 0;
                        }
                    }
                }
            }

            // Return the result
            return usernameExists;
        }


        public bool EmployeeUsernameDuplicateCheck(string username)
        {
            // Initialize the connection string
            string connectionString = DBHelper.DefaultConnectionString;

            // Define the query to check for duplicate username
            string query = "SELECT COUNT(*) FROM employee WHERE Username = @username";

            // Initialize a boolean variable to store the result
            bool usernameExists = false;

            // Create and open a connection to the database
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                // Create a MySqlCommand object with the query and connection
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    // Add parameters to the query
                    cmd.Parameters.AddWithValue("@username", username);

                    // Execute the query and check if any rows are returned
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        // If any rows are returned, set usernameExists to true
                        if (reader.Read())
                        {
                            usernameExists = reader.GetInt32(0) > 0;
                        }
                    }
                }
            }

            // Return the result
            return usernameExists;
        }
        public bool UsernameFormCheck(string username)
        {
            // Check if the username contains any spaces
            return !username.Contains(" ");
        }

        public int CheckInt()
        {
            int checkOut;
            do
            {
                string check = Console.ReadLine();
                if (!int.TryParse(check, out checkOut))
                {
                    Console.Write("Id Invalid. Please enter a valid number: ");
                }
                else
                {
                    return checkOut;
                }
            } while(true);
        }

        public bool CheckCategoryExist(int categoryId, MySqlConnection connection)
        {
            bool exists = false;
            try
            {
                // Mở kết nối nếu nó chưa mở
                connection.Open();
                // Tạo truy vấn SQL để kiểm tra sự tồn tại của categoryId
                string query = "SELECT COUNT(*) FROM category WHERE Category_ID = @CategoryID";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    // Thêm tham số vào truy vấn để tránh SQL injection
                    cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                    // Thực hiện truy vấn và lấy kết quả
                    var result = cmd.ExecuteScalar();

                    // Kiểm tra kết quả để xem categoryId có tồn tại không
                    if (result != null && Convert.ToInt32(result) > 0)
                    {
                        exists = true;
                    }
                    else
                    {
                        Console.WriteLine("Category id does not exist");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
            finally
            {
                // Đảm bảo kết nối được đóng sau khi hoàn tất
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
            }
            return exists;
        }

        public bool CheckCategoryNameExist(string name, MySqlConnection connection)
        {
            bool exists = false;

            try
            {
                // Open the connection (if not already open)
                connection.Open();

                // Prepare your SQL command to check if the category name exists
                string sql = "SELECT COUNT(*) FROM Category WHERE Category_Name = @CategoryName";

                // Create a command object
                MySqlCommand command = new MySqlCommand(sql, connection);

                // Add parameters to your command
                command.Parameters.AddWithValue("@CategoryName", name);

                // Execute the command and get the result
                int count = Convert.ToInt32(command.ExecuteScalar());

                // If count > 0, category name already exists
                exists = (count > 0);
            }
            catch (Exception ex)
            {
                exists = false; // For simplicity, assume category does not exist on exception
            }
            finally
            {
                // Close the connection
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return exists;
        }
    }
}
