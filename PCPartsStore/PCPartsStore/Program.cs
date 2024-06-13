using System;
using Helpers;
using MySql.Data.MySqlClient;
using PC_Part_Store;
using PC_Part_Store.Implement;
using static PC_Part_Store.Implement.Product;

public static class Program
{
    static void Main()
    {
        using (MySqlConnection connection = DBHelper.GetConnection())
        {
            Account account = new Account();
            MenuStore menu = new MenuStore();
            MenuStore.SearchHandler searchHandler = new MenuStore.SearchHandler();
            Product productHandler = new Product();
            Cart cartHandler = new Cart();          

            while (true)
            {
                menu.MenuLogin();
                Console.Write("Select an option: ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        string result = account.Login(connection);
                        // customer
                        if (result == "customer")
                        {
                            while (true)
                            {
                                menu.CustomerMenu();
                                Console.Write("Select an option: ");
                                string chooseCustomer = Console.ReadLine();
                                switch (chooseCustomer)
                                {
                                    case "1":
                                        Console.WriteLine("Next page selected.");
                                        break;
                                    case "2":
                                        Console.WriteLine("Previous page selected.");
                                        break;
                                    case "3":
                                        Console.WriteLine("Choose page selected.");
                                        break;
                                    case "4":
                                        Console.WriteLine("Search product selected.");
                                        searchHandler.SearchMenu();
                                        break;
                                    case "5":
                                        Console.WriteLine("View cart selected.");
                                        int customerId = account.idCustomer;
                                        cartHandler.ViewCart(customerId, connection);
                                        cartHandler.ManageCart(customerId, connection);
                                        break;
                                    case "6":
                                        Console.WriteLine("Enter product ID to view detail selected.");
                                        Console.Write("Enter product ID: ");
                                        int productId = int.Parse(Console.ReadLine());
                                        Product product = new Product();
                                        product.viewProductDetails(productId, connection);
                                        break;
                                    case "7":
                                        Console.WriteLine("Enter product ID to add to cart selected.");
                                        break;
                                    case "8":
                                        Console.WriteLine("Exiting customer menu.");
                                        return; // Exiting
                                    default:
                                        Console.WriteLine("Invalid option. Please try again.");
                                        break;
                                }
                            }
                        }
                        else
                        {
                            // employee
                            if (result == "employee")
                            {
                                menu.EmployeeMenu();
                                Console.WriteLine("Select an option:");
                                string chooseEmployee = Console.ReadLine();
                                switch (chooseEmployee)
                                {
                                    case "1":
                                        Console.WriteLine("Approve application selected.");                                       
                                        break;
                                    case "2":
                                        Console.WriteLine("Product management selected.");
                                        Console.WriteLine("1. View Products");
                                        Console.WriteLine("2. Enter Product ID to Manage");
                                        Console.WriteLine("3. Back");
                                        Console.Write("Select an option: ");
                                        string productManageOption = Console.ReadLine();

                                        switch (productManageOption)
                                        {
                                            case "1":
                                                productHandler.ViewAllProduct(connection);
                                                break;
                                            case "2":
                                                Console.Write("Enter product ID to manage: ");
                                                int manageProductId = int.Parse(Console.ReadLine());
                                                productHandler.ManageProductOptions(manageProductId, connection);
                                                break;
                                            case "3":
                                                return;
                                            default:
                                                Console.WriteLine("Invalid option. Please try again.");
                                                break;
                                        }
                                        break;
                                    case "3":
                                        Console.WriteLine("Going back.");
                                        return;
                                    case "4":
                                        Console.WriteLine("Exiting employee menu.");
                                        return;
                                    default:
                                        Console.WriteLine("Invalid option. Please try again.");
                                        break;
                                }
                            }
                            // admin
                            else
                            {
                                if (result == "admin")
                                {
                                    menu.AdminMenu();
                                    Console.WriteLine("Select an option:");
                                    string chooseAdmin = Console.ReadLine();
                                    switch (chooseAdmin)
                                    {
                                        case "1":
                                            Console.WriteLine("Add Employee selected.");
                                            break;
                                        case "2":
                                            Console.WriteLine("View Employee Details selected.");
                                            break;
                                        case "3":
                                            Console.WriteLine("Delete Employee by ID selected.");
                                            break;
                                        case "4":
                                            Console.WriteLine("Update Employee Information by ID selected.");
                                            break;
                                        case "5":
                                            Console.WriteLine("Search Employee selected.");
                                            break;
                                        case "6":
                                            Console.WriteLine("Going back.");
                                            return;
                                        case "7":
                                            Console.WriteLine("Exiting admin menu.");
                                            return;
                                        default:
                                            Console.WriteLine("Invalid option. Please try again.");
                                            break;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Invalid username or password.");
                                }
                            }
                        }
                        break;
                    case "2":
                        account.Register(connection);
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
    }
}
