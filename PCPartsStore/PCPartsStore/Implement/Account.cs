﻿using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using PC_Part_Store.Interface;
using PCPartsStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Principal;
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
            Console.WriteLine("Enter new information employee");
            do
            {
                Console.Write("Enter user name employee: ");
                username = Console.ReadLine();
                if (string.IsNullOrEmpty(username))
                {
                    Console.WriteLine("Please do not leave it blank");
                }
                else
                {
                    break;
                }
            } while (true);
            while (!validations.UsernameFormCheck(username))
            {
                do
                {
                    Console.Write("Invalid username form, please enter again: ");
                    username = Console.ReadLine();
                    if (string.IsNullOrEmpty(username))
                    {
                        Console.WriteLine("Please do not leave it blank");
                    }
                    else
                    {
                        break;
                    }
                } while(true);
            }
            while (validations.EmployeeUsernameDuplicateCheck(username))
            {
                do
                {
                    Console.Write("Duplicated username, please enter again: ");
                    username = Console.ReadLine();
                    if (string.IsNullOrEmpty(username))
                    {
                        Console.WriteLine("Please do not leave it blank");
                    }
                    else
                    {
                        break;
                    }
                } while(true);
            }
            do {
                Console.Write("Enter password: ");
                password = Console.ReadLine();
                if (string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Please do not leave it blank");
                }
                else
                {
                    break;
                }
            }while(true);
            do
            {
                Console.Write("Enter name employee: ");
                name = Console.ReadLine();
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("Please do not leave it blank");
                }
                else
                {
                    break;
                }
            } while (true);
            do
            {
                Console.Write("Enter address employee: ");
                address = Console.ReadLine();
                if (string.IsNullOrEmpty(address))
                {
                    Console.WriteLine("Please do not leave it blank");
                }
                else
                {
                    break;
                }
            } while (true);
            do
            {
                Console.Write("Enter email employee: ");
                email = Console.ReadLine();
            } while (validations.CheckEmail(email)==false);
            do
            {
                Console.Write("Enter phone number: ");
                phoneNumber = Console.ReadLine();               
            } while (validations.CheckPhoneNumber(phoneNumber)==false);
            string query = "INSERT INTO employee (username,password,Employee_name, address, Phone_number, email) VALUES (@username ,@password, @name, @address, @phoneNumber, @email)";
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
                    Console.WriteLine("Create account employee successful!");
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
            Console.Write("Enter user name: ");
            username = Console.ReadLine();
            Console.Write("Enter password: ");
            password = Console.ReadLine();
            int count = 0;
            while (!validations.AccountExistCheck(username, password))
            {
                count++;
                if (count == 3)
                {
                    Console.WriteLine("Do you want to continue re-entering your account?");
                    Console.WriteLine("1. Yes");
                    Console.WriteLine("2. No");
                    Console.Write("Enter seletion: ");
                    int selectionLogin;
                    do
                    {
                        selectionLogin = validations.CheckInt();
                        if(selectionLogin == 1 || selectionLogin == 2)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("selection invalid");
                        }
                    } while (true);
                    if (selectionLogin == 1)
                    {
                        count = 0;
                    }
                    else
                    {
                        return null;
                    }
                }
                Console.WriteLine("Account or password is incorrect");
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
            do {
                Console.Write("Enter user name: ");
                username = Console.ReadLine();
                if (string.IsNullOrEmpty(username))
                {
                    Console.WriteLine("Please do not leave it blank");
                }
                else
                {
                    break;
                }
            }while(true);
            while (!validations.UsernameFormCheck(username))
            {
                do
                {
                    Console.Write("Invalid username form, please enter again: ");
                    username = Console.ReadLine();
                    if (string.IsNullOrEmpty(username))
                    {
                        Console.WriteLine("Please do not leave it blank");
                    }
                    else
                    {
                        break;
                    }
                } while(true);
            }
            while (validations.CustomerUsernameDuplicateCheck(username))
            {
                do
                {
                    Console.Write("Duplicated username, please enter again: ");
                    username = Console.ReadLine();
                    if (string.IsNullOrEmpty(username))
                    {
                        Console.WriteLine("Please do not leave it blank");
                    }
                    else
                    {
                        break;
                    }
                } while (true);
            }
            do
            {
                Console.Write("Enter password: ");
                password = Console.ReadLine();
                if (string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Please do not leave it blank");
                }
                else
                {
                    break;
                }
            } while (true);
            do
            {
                Console.Write("Enter name: ");
                name = Console.ReadLine();
                if (string.IsNullOrEmpty(name))
                {
                    Console.WriteLine("Please do not leave it blank");
                }
                else
                {
                    break;
                }
            } while (true);
            do
            {
                Console.Write("Enter email: ");
                email = Console.ReadLine();
            } while (validations.CheckEmail(email) == false);
            do
            {
                Console.Write("Enter phone number: ");
                phoneNumber = Console.ReadLine();
            } while (validations.CheckPhoneNumber(phoneNumber) == false);
            do
            {
                Console.Write("Enter address: ");
                address = Console.ReadLine();
                if (string.IsNullOrEmpty(address))
                {
                    Console.WriteLine("Please do not leave it blank");
                }
                else
                {
                    break;
                }
            } while (true);
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
            Console.Write("Enter new name: ");
            string name = Console.ReadLine();
            string email;
            do
            {
                Console.Write("Enter new email: ");
                email = Console.ReadLine();
                if(string.IsNullOrEmpty(email) || validations.CheckEmail(email) == true)
                {
                    break;
                }
            } while (true);
            string phoneNumber;
            do
            {
                Console.Write("Enter new phone number: ");
                phoneNumber = Console.ReadLine();
                if ( string.IsNullOrEmpty(phoneNumber) || validations.CheckPhoneNumber(phoneNumber) == true)
                {
                    break;
                }
            } while (true);
            Console.Write("Enter new address: ");
            string address = Console.ReadLine();

            string query = "UPDATE customer SET ";
            List<string> updates = new List<string>();
            if (!string.IsNullOrEmpty(name))
            {
                query += "Customer_name = @name, ";
                updates.Add("Employee_name = @name");
            }
            if (!string.IsNullOrEmpty(email))
            {
                query += "email = @email, ";
                updates.Add("email = @email");
            }
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                query += "phone_number = @phoneNumber, ";
                updates.Add("phone_number = @phoneNumber");
            }
            if (!string.IsNullOrEmpty(address))
            {
                query += "address = @address, ";
                updates.Add("address = @address");
            }

            // Remove the trailing comma and space if any updates were added
            if (updates.Count != 0)
            {
                query = query.Substring(0, query.Length - 2); // Remove last ", "
                query += " WHERE Customer_ID = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                    }
                    if (!string.IsNullOrEmpty(email))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                    }
                    if (!string.IsNullOrEmpty(phoneNumber))
                    {
                        cmd.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                    }
                    if (!string.IsNullOrEmpty(address))
                    {
                        cmd.Parameters.AddWithValue("@address", address);
                    }
                    cmd.Parameters.AddWithValue("@id", id);

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
            else
            {
                Console.WriteLine("No information to update.");
            }
        }

        public void UpdateInformationEmployee(int id, MySqlConnection connection)
        {
            Console.WriteLine("Update Employee information (or press Enter to skip)");
            Console.Write("Update Name employee: ");
            string name = Console.ReadLine();
            Console.Write("Update Address employee: ");
            string address = Console.ReadLine();
            string phoneNumber;
            do
            {
                Console.Write("Update phone number employee: ");
                phoneNumber = Console.ReadLine();
                if (string.IsNullOrEmpty(phoneNumber) || validations.CheckPhoneNumber(phoneNumber))
                {
                    break;
                }
            } while (true);
            string email;
            do
            {
                Console.Write("Update email employee: ");
                email = Console.ReadLine();
                if(string.IsNullOrEmpty(email) || validations.CheckEmail(email))
                {
                    break;
                }
            } while (true);
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
            query += string.Join(", ", updates) + " WHERE Employee_Id = @id";

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

        public void ViewInformationCustomer(int customerId, MySqlConnection connection)
        {
            string queryCustomer = "SELECT Customer_name,Email,Address,phone_number FROM customer WHERE Customer_ID=@customerId";
            try
            {
                connection.Open();
                using(MySqlCommand cmdCustomer = new MySqlCommand(queryCustomer, connection))
                {
                    cmdCustomer.Parameters.AddWithValue("@customerId", customerId);
                    using (MySqlDataReader reader = cmdCustomer.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string customerName = reader.GetString("Customer_name");
                            string customerPhoneNumber = reader.GetString("phone_number");
                            string customerEmail = reader.GetString("Email");
                            string customerAddress = reader.GetString("Address");
                            Console.WriteLine("+----------------------------------------+");
                            Console.WriteLine($"Name: {customerName}");
                            Console.WriteLine($"Phone Number: {customerPhoneNumber}");
                            Console.WriteLine($"Email: {customerEmail}");
                            Console.WriteLine($"Address: {customerAddress}");
                            Console.WriteLine("+----------------------------------------+");
                        }
                    }
                }
            }catch(Exception ex) 
            {
                Console.WriteLine("Cannot connect to database: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public int ViewInformationEmployee(int employeeId, MySqlConnection connection)
        {
            string queryCustomer = "SELECT * from employee WHERE Employee_ID=@employeeId";
            try
            {
                connection.Open();
                using (MySqlCommand cmdCustomer = new MySqlCommand(queryCustomer, connection))
                {
                    cmdCustomer.Parameters.AddWithValue("@employeeId", employeeId);
                    using (MySqlDataReader reader = cmdCustomer.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            idEmployee = reader.GetInt32("Employee_ID");
                            address = reader.GetString("Address");
                            name = reader.GetString("Employee_name");
                            phoneNumber = reader.GetString("Phone_number");
                            password = reader.GetString("Password");
                            email = reader.GetString("Email");
                            username = reader.GetString("username");
                            Console.WriteLine("+----------------------------------------+");
                            Console.WriteLine($"ID: {idEmployee}\n Name: {name}\n Address: {address}\n " +
                                                                $"Phone number: {phoneNumber}\n Email: {email}\n Username: {username}\n Password: {password}");
                            Console.WriteLine("+----------------------------------------+");
                            return 1;
                        }
                        else
                        {
                            Console.WriteLine("Employee not found");
                            return 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot connect to database: " + ex.Message);
                //return 0;
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public void ViewAllEmployee(MySqlConnection connection)
        {
            string query = @" SELECT * FROM employee";

            List<Account> accounts = new List<Account>();

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Account account = new Account
                            {
                                idEmployee = reader.GetInt32("Employee_ID"),
                                address=reader.GetString("Address"),
                                name=reader.GetString("Employee_name"),
                                phoneNumber=reader.GetString("Phone_number"),
                                password=reader.GetString("Password"),
                                email=reader.GetString("Email"),
                                username=reader.GetString("username")
                            };
                            accounts.Add(account);
                        }
                        if (accounts.Count == 0)
                        {
                            Console.WriteLine("List employee iss empty");
                            return;
                        }
                        else
                        {
                            //chinh sua phan trang muon test
                            int pageSize = 5;
                            int totalRecords = accounts.Count;
                            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                            if (pageCurrentEmloyee < 1)
                            {
                                Console.WriteLine("Page number must be greater than 0. Setting to page 1.");
                                pageCurrentEmloyee = 1;
                            }
                            else if (pageCurrentEmloyee > totalPages)
                            {
                                Console.WriteLine("Page number exceeds total pages. Setting to last page.");
                                pageCurrentEmloyee = totalPages;
                            }

                            int start = (pageCurrentEmloyee - 1) * pageSize;
                            int end = Math.Min(start + pageSize, totalRecords);
                            Console.WriteLine("+----------+---------------+---------------+----------------+-------------------+--------------+------------+");
                            Console.WriteLine("| ID       | Name          | Address       | Phone Number   | Email             | Username     | Password   |");
                            Console.WriteLine("+----------+---------------+---------------+----------------+-------------------+--------------+------------+");

                            // Print each employee in the current page
                            for (int i = start; i < end; i++)
                            {
                                Account account = accounts[i];
                                Console.WriteLine($"| {account.idEmployee,-8} | {account.name,-13} | {account.address,-13} | {account.phoneNumber,-14} | {account.email,-17} | {account.username,-12} | {account.password,-10} |");
                            }

                            // Print the footer
                            Console.WriteLine("+----------+---------------+---------------+----------------+-------------------+--------------+------------+");
                            Console.WriteLine($"Page {pageCurrentEmloyee} of {totalPages}");
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void DeleteAccountEmployee(int employeeId, MySqlConnection connection)
        {
            string deleteQuery = "DELETE FROM employee WHERE Employee_ID=@employeeId";
            try
            {
                connection.Open();
                using (MySqlCommand cmdDelete = new MySqlCommand(deleteQuery, connection))
                {
                    cmdDelete.Parameters.AddWithValue("@employeeId", employeeId);
                    int rowsAffected = cmdDelete.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Employee account deleted successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Employee not found.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot connect to database: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }


        public int SearchEmployeeByName(string name, MySqlConnection connection)
        {
            string queryEmployees = "SELECT * FROM employee WHERE Employee_name LIKE @name";
            List<Account> accounts = new List<Account>();
            try
            {
                connection.Open();
                using(MySqlCommand cmdEmployees =new MySqlCommand(queryEmployees, connection))
                {
                    cmdEmployees.Parameters.AddWithValue("@name", "%" + name + "%");

                    using (MySqlDataReader reader = cmdEmployees.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Account account = new Account
                            {
                                idEmployee = reader.GetInt32("Employee_ID"),
                                address = reader.GetString("Address"),
                                name = reader.GetString("Employee_name"),
                                phoneNumber = reader.GetString("Phone_number"),
                                password = reader.GetString("Password"),
                                email = reader.GetString("Email"),
                                username = reader.GetString("username")
                            };
                            accounts.Add(account);
                        }
                    }
                    if (accounts.Count > 0)
                    {
                        Console.WriteLine($"Search results for employees with name containing '{name}':");
                        int pageSize = 1;
                        int totalRecords = accounts.Count;
                        int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

                        if (pageCurrentEmloyee < 1)
                        {
                            Console.WriteLine("Page number must be greater than 0. Setting to page 1.");
                            pageCurrentEmloyee = 1;
                        }
                        else if (pageCurrentEmloyee > totalPages)
                        {
                            Console.WriteLine("Page number exceeds total pages. Setting to last page.");
                            pageCurrentEmloyee = totalPages;
                        }

                        int start = (pageCurrentEmloyee - 1) * pageSize;
                        int end = Math.Min(start + pageSize, totalRecords);
                        Console.WriteLine("+----------+---------------+---------------+----------------+-------------------+--------------+------------+");
                        Console.WriteLine("| ID       | Name          | Address       | Phone Number   | Email             | Username     | Password   |");
                        Console.WriteLine("+----------+---------------+---------------+----------------+-------------------+--------------+------------+");

                        // Print each employee in the current page
                        for (int i = start; i < end; i++)
                        {
                            Account account = accounts[i];
                            Console.WriteLine($"| {account.idEmployee,-8} | {account.name,-13} | {account.address,-13} | {account.phoneNumber,-14} | {account.email,-17} | {account.username,-12} | {account.password,-10} |");
                        }

                        // Print the footer
                        Console.WriteLine("+----------+---------------+---------------+----------------+-------------------+--------------+------------+");
                        Console.WriteLine($"Page {pageCurrentEmloyee} of {totalPages}");
                        return 1;
                    }
                    else
                    {
                        Console.WriteLine("No employees found with the given name.");
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving employees: {ex.Message}");
                throw;
            }
            finally
            {
                connection.Close();
            }

        }
    }
}
