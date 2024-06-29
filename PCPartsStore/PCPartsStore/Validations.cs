using Helpers;
using MySql.Data.MySqlClient;
using PCPartsStore.Interface;
using System;
using System.Collections.Generic;
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

        public bool ProductExistCheck(int productId)
        {
            bool exists = false;

            using (MySqlConnection conn = DBHelper.GetConnection())
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Product WHERE ProductId = @productId";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@productId", productId);
                    exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }

            return exists;
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
            string query = "SELECT COUNT(*) FROM CustomerTable WHERE Username = @username";

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
    }
}
