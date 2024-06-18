using PC_Part_Store.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store
{
    public class MenuStore
    {
        public void MenuLogin()
        {
            Console.WriteLine("Welcome to PC STORE");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Exit");
        }
        public void CustomerMenu()
        {
            Console.WriteLine("1. Next page");
            Console.WriteLine("2. Previous page");
            Console.WriteLine("3. Choose page");
            Console.WriteLine("4. Search product");
            Console.WriteLine("5. View cart");
            Console.WriteLine("6. Enter product ID to view detail");
            Console.WriteLine("7. Enter product ID to add to cart");
            Console.WriteLine("8. Quit");
        }
        public void EmployeeMenu()
        {
            Console.WriteLine("Employee Dashboard");
            Console.WriteLine("1. Approve application");
            Console.WriteLine("2. Product management");
            Console.WriteLine("3. Back");
            Console.WriteLine("4. Quit");
        }
        public void AdminMenu()
        {
            Console.WriteLine("Admin Menu");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. View Employee Details");
            Console.WriteLine("3. Delete Employee by ID");
            Console.WriteLine("4. Update Employee Information by ID");
            Console.WriteLine("5. Search Employee");
            Console.WriteLine("6. Back");
            Console.WriteLine("7. Quit");
        }
        public class SearchHandler
        {
            public void SearchMenu()
            {
                while (true)
                {
                    Console.WriteLine("Search Menu:");
                    Console.WriteLine("1. Search by Name");
                    Console.WriteLine("2. Search by Category");
                    Console.WriteLine("3. Back to List Products");
                    Console.WriteLine("4. Quit");
                    Console.Write("Enter your choice: ");

                    string choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            SearchByName();
                            break;
                        case "2":
                            SearchByCategory();
                            break;
                        case "3":
                            return; // Exit 
                        case "4":
                            Environment.Exit(0); // Quit 
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please enter a number between 1 and 4.");
                            break;
                    }
                }
            }

            private void SearchByName()
            {
                Console.Write("Enter the name to search: ");
                string name = Console.ReadLine();
                Console.WriteLine($"Searching by name: {name}");
            }

            private void SearchByCategory()
            {
                Console.Write("Enter the category to search: ");
                string category = Console.ReadLine();
                Console.WriteLine($"Searching by category: {category}");
            }
        }
    }
}

