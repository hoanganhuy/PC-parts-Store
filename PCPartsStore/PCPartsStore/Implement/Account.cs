using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using PC_Part_Store.Interface;
using PCPartsStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Program;

namespace PC_Part_Store.Implement
{
    public class Account : IAccount
    {
        public int idCustomer { get; set; }
        public int idEmployee { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string name { get; set; }
        public Validations validations = new Validations();
        public void CreateAccountEmloyee(MySqlConnection connection)
        {
            Console.WriteLine("Enter new infomation employee:");
            Console.Write("Enter user name employee:");
            username = Console.ReadLine();
            while (!validations.UsernameFormCheck(username))
            {
                Console.Write("Invalid username form, please enter again: ");
                username = Console.ReadLine();
            }
            while (!validations.EmployeeUsernameDuplicateCheck(username))
            {
                Console.Write("Duplicated username, please enter again: ");
                username = Console.ReadLine();
            }
            Console.Write("Enter password:");
            password = Console.ReadLine();
            Console.Write("Enter name employee:");
            name = Console.ReadLine();
            Console.Write("Enter address employee:");
            address = Console.ReadLine();
            Console.Write("Enter email employee:");
            email = Console.ReadLine();
            Console.Write("Enter phone number: ");
            phoneNumber = Console.ReadLine();
            string query = "INSERT INTO employee (username,password,Employee_name, address, phoneNumber, email) VALUES (@username ,@password, @name, @address, @phoneNumber, @email)";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@address", address);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Create account employee sucessful!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public string Login(MySqlConnection connection)
        {
            connection.Open ();
            Console.WriteLine("Screen login");
            Console.Write("Enter user name :");
            username = Console.ReadLine();
            Console.Write("Enter password: ");
            password = Console.ReadLine();
            while (!validations.AccountExistCheck(username, password))
            {
                
                Console.Write("Username: ");
                username = Console.ReadLine();
                Console.Write("Password: ");
                password = Console.ReadLine();
            }
            string queryCustomer = "SELECT Customer_Id FROM customer WHERE username = @username AND password = @password";
            using (MySqlCommand cmd = new MySqlCommand(queryCustomer, connection))
            {
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        
                        idCustomer = reader.GetInt32("Customer_Id");
                        Program.customerIdCurrent = idCustomer;
                        Console.WriteLine("Login successful");
                        connection.Close();
                        return "customer";
                    }
                }
            }
            string queryEmployee = "SELECT Employee_Id FROM employee WHERE username = @username AND password = @password";
            using (MySqlCommand cmdEmployee = new MySqlCommand(queryEmployee, connection))
            {
                cmdEmployee.Parameters.AddWithValue("@username", username);
                cmdEmployee.Parameters.AddWithValue("@password", password);

                using (MySqlDataReader reader = cmdEmployee.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        idEmployee = reader.GetInt32("Employee_Id");
                        Console.WriteLine("Login employee successful");
                        connection.Close();
                        return "employee";
                    }
                }
            }
            string queryAdmin = "SELECT Admin_Id FROM admin WHERE username = @username AND password = @password";
            using (MySqlCommand cmdAdmin = new MySqlCommand(queryAdmin, connection))
            {
                cmdAdmin.Parameters.AddWithValue("@username", username);
                cmdAdmin.Parameters.AddWithValue("@password", password);

                using (MySqlDataReader reader = cmdAdmin.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine("Login admin successful");
                        connection.Close();
                        return "admin";
                    }
                }
            }

            Console.WriteLine("Invalid username or password.");
            connection.Close();
            return null;
        }
        public void Register(MySqlConnection connection)
        {
            Console.WriteLine("Screen register");
            Console.Write("Enter user name: ");
            username = Console.ReadLine();
            while (!validations.UsernameFormCheck(username))
            {
                Console.Write("Invalid username form, please enter again: ");
                username = Console.ReadLine();
            }
            while (validations.CustomerUsernameDuplicateCheck(username))
            {
                Console.Write("Duplicated username, please enter again: ");
                username = Console.ReadLine();
            }
            Console.Write("Enter password: ");
            password = Console.ReadLine();
            Console.Write("Enter name: ");
            name = Console.ReadLine();
            Console.Write("Enter email: ");
            email = Console.ReadLine();
            Console.Write("Enter Phone number: ");
            phoneNumber = Console.ReadLine();
            Console.Write("Enter address: ");
            address = Console.ReadLine();
            string query = "INSERT INTO customer (username, password, Customer_name, email, phone_number, address) " +
                       "VALUES (@username, @password, @name, @email, @phoneNumber, @address)";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                cmd.Parameters.AddWithValue("@address", address);
                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Registration successful!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void UpdateInformationCustomer(int id, MySqlConnection connection)
        {
            Console.WriteLine("Update Customer Information (or press Enter to skip)");
            Console.Write("Enter new name : ");
            string name = Console.ReadLine();
            Console.Write("Enter new email : ");
            string email = Console.ReadLine();
            Console.Write("Enter new phone number : ");
            string phoneNumber = Console.ReadLine();
            Console.Write("Enter new address : ");
            string address = Console.ReadLine();
            string query = "UPDATE customer SET ";
            List<string> updates = new List<string>();
            if (!string.IsNullOrEmpty(name)) updates.Add("Customer_name = @name");
            if (!string.IsNullOrEmpty(email)) updates.Add("email = @email");
            if (!string.IsNullOrEmpty(phoneNumber)) updates.Add("phone_number = @phoneNumber");
            if (!string.IsNullOrEmpty(address)) updates.Add("address = @address");
            if (updates.Count == 0)
            {
                Console.WriteLine("No information to update.");
                return;
            }

            query += string.Join(", ", updates) + " WHERE customerId = @id";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                if (!string.IsNullOrEmpty(name)) cmd.Parameters.AddWithValue("@name", name);
                if (!string.IsNullOrEmpty(email)) cmd.Parameters.AddWithValue("@email", email);
                if (!string.IsNullOrEmpty(phoneNumber)) cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                if (!string.IsNullOrEmpty(address)) cmd.Parameters.AddWithValue("@address", address);
                cmd.Parameters.AddWithValue("@customerId", id);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Customer information updated successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
        public void UpdateInformationEmployee(int id, MySqlConnection connection)
        {
            Console.WriteLine("Update Employee information (or press Enter to skip)");
            Console.Write("Update Name employee: ");
            string name = Console.ReadLine();
            Console.Write("Update Address employee: ");
            string address = Console.ReadLine();
            Console.Write("Update phone number employee: ");
            string phoneNumber = Console.ReadLine();
            Console.Write("Update email employee: ");
            string email = Console.ReadLine();
            string query = "UPDATE employee SET ";
            List<string> updates = new List<string>();
            if (!string.IsNullOrEmpty(name)) updates.Add("Employee_name = @name");
            if (!string.IsNullOrEmpty(email)) updates.Add("email = @email");
            if (!string.IsNullOrEmpty(phoneNumber)) updates.Add("phone_number = @phoneNumber");
            if (!string.IsNullOrEmpty(address)) updates.Add("address = @address");
            if (updates.Count == 0)
            {
                Console.WriteLine("No information to update.");
                return;
            }
            query += string.Join(", ", updates) + " WHERE employeeId = @id";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                if (!string.IsNullOrEmpty(name)) cmd.Parameters.AddWithValue("@name", name);
                if (!string.IsNullOrEmpty(email)) cmd.Parameters.AddWithValue("@email", email);
                if (!string.IsNullOrEmpty(phoneNumber)) cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                if (!string.IsNullOrEmpty(address)) cmd.Parameters.AddWithValue("@address", address);
                cmd.Parameters.AddWithValue("@id", id);
                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                    Console.WriteLine("Employee information updated successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
