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

namespace PC_Part_Store.Implement
{
    public class Account : IAccount
    {
        public int idCustomer { get; set; }
        public int idEmployee { get; set; }
        public string userName { get; set; }
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
            userName = Console.ReadLine();
            while (!validations.UsernameFormCheck(userName))
            {
                Console.Write("Invalid username form, please enter again: ");
                userName = Console.ReadLine();
            }
            while (!validations.EmployeeUsernameDuplicateCheck(userName))
            {
                Console.Write("Duplicated username, please enter again: ");
                userName = Console.ReadLine();
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
            string query = "INSERT INTO employee (userName,password,name, address, phoneNumber, email) VALUES (@userName ,@password, @name, @address, @phoneNumber, @email)";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@userName", userName);
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
            Console.WriteLine("Screen login");
            Console.Write("Enter user name :");
            userName = Console.ReadLine();
            Console.Write("Enter password: ");
            password = Console.ReadLine();
            while (!validations.AccountExistCheck(userName, password))
            {
                Console.WriteLine("Account is not exist, please enter again: ");
                Console.Write("Username: ");
                userName = Console.ReadLine();
                Console.Write("Password: ");
                password = Console.ReadLine();
            }
            string queryCustomer = "SELECT customerId FROM customer WHERE userName = @userName AND password = @password";
            using (MySqlCommand cmd = new MySqlCommand(queryCustomer, connection))
            {
                cmd.Parameters.AddWithValue("@userName", userName);
                cmd.Parameters.AddWithValue("@password", password);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        idCustomer = reader.GetInt32("customerId");
                        Console.WriteLine("Login successful");
                        return "customer";
                    }
                }
            }
            string queryEmployee = "SELECT employeeId FROM employee WHERE userName = @userName AND password = @password";
            using (MySqlCommand cmdEmployee = new MySqlCommand(queryEmployee, connection))
            {
                cmdEmployee.Parameters.AddWithValue("@userName", userName);
                cmdEmployee.Parameters.AddWithValue("@password", password);

                using (MySqlDataReader reader = cmdEmployee.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        idEmployee = reader.GetInt32("employeeId");
                        Console.WriteLine("Login employee successful");
                        return "employee";
                    }
                }
            }
            string queryAdmin = "SELECT adminId FROM admin WHERE userName = @userName AND password = @password";
            using (MySqlCommand cmdAdmin = new MySqlCommand(queryAdmin, connection))
            {
                cmdAdmin.Parameters.AddWithValue("@userName", userName);
                cmdAdmin.Parameters.AddWithValue("@password", password);

                using (MySqlDataReader reader = cmdAdmin.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine("Login admin successful");
                        return "admin";
                    }
                }
            }

            Console.WriteLine("Invalid username or password.");
            return null;
        }
        public void Register(MySqlConnection connection)
        {
            Console.WriteLine("Screen register");
            Console.Write("Enter user name:");
            userName = Console.ReadLine();
            while (!validations.UsernameFormCheck(userName))
            {
                Console.Write("Invalid username form, please enter again: ");
                userName = Console.ReadLine();
            }
            while (!validations.CustomerUsernameDuplicateCheck(userName))
            {
                Console.Write("Duplicated username, please enter again: ");
                userName = Console.ReadLine();
            }
            Console.Write("Enter password:");
            password = Console.ReadLine();
            Console.Write("Enter name:");
            name = Console.ReadLine();
            Console.Write("Enter email: ");
            email = Console.ReadLine();
            Console.Write("Enter Phone number: ");
            phoneNumber = Console.ReadLine();
            Console.Write("Enter address: ");
            address = Console.ReadLine();
            string query = "INSERT INTO customer (username, password, name, email, phoneNnumber, address) " +
                       "VALUES (@userName, @password, @name, @email, @phoneNumber, @address)";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@userName", userName);
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
            if (!string.IsNullOrEmpty(name)) updates.Add("name = @name");
            if (!string.IsNullOrEmpty(email)) updates.Add("email = @email");
            if (!string.IsNullOrEmpty(phoneNumber)) updates.Add("phoneNumber = @phoneNumber");
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
            if (!string.IsNullOrEmpty(name)) updates.Add("name = @name");
            if (!string.IsNullOrEmpty(email)) updates.Add("email = @email");
            if (!string.IsNullOrEmpty(phoneNumber)) updates.Add("phoneNumber = @phoneNumber");
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
