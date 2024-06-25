using System;
using Helpers;
using MySql.Data.MySqlClient;
using PC_Part_Store;
using PC_Part_Store.Implement;
using static PC_Part_Store.Implement.Product;

public static class Program
{
    public static int pageNumberCurrent {  get; set; }=1;
    public static int customerIdCurrent { get; set; }
    static void Main()
    {
        using (MySqlConnection connection = DBHelper.GetConnection())
        {
            Account account = new Account();
            MenuStore menu = new MenuStore();
            Product productHandler = new Product();
            Cart cartHandler = new Cart();
            Order orderHandler = new Order();

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
                            string chooseCustomer = "0";
                            while (chooseCustomer != "10")
                            {
                                Console.WriteLine("PC PARTS STORE");
                                productHandler.ViewAllProduct(connection);
                                menu.CustomerMenu();                              
                                Console.Write("Select an option: ");
                                chooseCustomer = Console.ReadLine();
                                switch (chooseCustomer)
                                {
                                    case "1":
                                        {
                                            pageNumberCurrent--;
                                            break;
                                        }
                                    case "2":
                                        {
                                            pageNumberCurrent++;
                                            break;
                                        }
                                    case "3":
                                        {
                                            int pageNumber;
                                            do
                                            {
                                                Console.Write("Select tha page you want to go to: ");
                                                string check = Console.ReadLine();
                                                if (!int.TryParse(check, out pageNumber))
                                                {
                                                    Console.WriteLine("Selection Isvalid");
                                                }
                                                else pageNumberCurrent = pageNumber; break;
                                            } while (true);
                                            break;
                                        }
                                    case "4":
                                        {
                                            int viewDetailsId;
                                            do
                                            {
                                                Console.Write("Enter the porduct id you want to view details: ");
                                                string check = Console.ReadLine();
                                                if (!int.TryParse(check, out viewDetailsId))
                                                {
                                                    Console.WriteLine("Id Isvalid");
                                                }
                                                else break;
                                            } while (true);
                                            
                                            if (productHandler.ViewProductDetails(viewDetailsId, connection) == 1)
                                            {
                                                string input;
                                                do
                                                {
                                                    Console.WriteLine("1 - Add product to cart");
                                                    Console.WriteLine("2 - Back");
                                                    Console.Write("Options: ");
                                                    input = Console.ReadLine();
                                                    switch (input)
                                                    {
                                                        case "1":
                                                            Console.Write("Enter quantity to add to cart: ");
                                                            if (int.TryParse(Console.ReadLine(), out int quantity))
                                                            {
                                                                productHandler.AddToCart(viewDetailsId, customerIdCurrent, quantity, connection);
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Invalid quantity. Please enter a valid number.");
                                                            }
                                                            break;
                                                        case "2":
                                                            // Exit 
                                                            break;
                                                        default:
                                                            Console.WriteLine("Invalid option, please try again.");
                                                            break;
                                                    }
                                                }while(input != "2"&&input!="1");
                                            }
                                            break;
                                        }
                                    case "5":
                                        {
                                            Console.WriteLine("Search product selected.");
                                            menu.SearchMenu();
                                            string selectSearch;
                                            do
                                            {
                                                Console.Write("Enter selection: ");
                                                selectSearch = Console.ReadLine();
                                                if(selectSearch != "1" || selectSearch != "2" || selectSearch != "3")
                                                {
                                                    break;
                                                }
                                            } while (true);
                                            switch (selectSearch)
                                            {
                                                case "1":
                                                    {//search by name
                                                        pageNumberCurrent = 1;
                                                        Console.Write("Enter name product:");
                                                        string nameSearch= Console.ReadLine();
                                                        do
                                                        {
                                                            productHandler.SearchProductByName(nameSearch, connection);
                                                            menu.SearchMenuOption();
                                                            Console.Write("Select an option: ");
                                                            string selectOptionSearch = Console.ReadLine();
                                                            switch (selectOptionSearch)
                                                            {
                                                                case "1":
                                                                    {
                                                                        pageNumberCurrent++;
                                                                        break;
                                                                    }
                                                                case "2":
                                                                    {
                                                                        pageNumberCurrent--;
                                                                        break;
                                                                    }
                                                                case "3":
                                                                    {
                                                                        int pageNumber;
                                                                        do
                                                                        {
                                                                            Console.Write("Select tha page you want to go to: ");
                                                                            string check = Console.ReadLine();
                                                                            if (!int.TryParse(check, out pageNumber))
                                                                            {
                                                                                Console.WriteLine("Selection Isvalid");
                                                                            }
                                                                            else pageNumberCurrent = pageNumber; break;
                                                                        } while (true);
                                                                        break;
                                                                    }
                                                                case "4"://xem chi tiet
                                                                    {
                                                                        int viewDetailsId;
                                                                        do
                                                                        {
                                                                            Console.Write("Enter the porduct id you want to view details: ");
                                                                            string check = Console.ReadLine();
                                                                            if (!int.TryParse(check, out viewDetailsId))
                                                                            {
                                                                                Console.WriteLine("Id Isvalid");
                                                                            }
                                                                            else break;
                                                                        } while (true);

                                                                        if (productHandler.ViewProductDetails(viewDetailsId, connection) == 1)
                                                                        {
                                                                            string input;
                                                                            do
                                                                            {
                                                                                Console.WriteLine("1 - Add product to cart");
                                                                                Console.WriteLine("2 - Back");
                                                                                Console.Write("Options: ");
                                                                                input = Console.ReadLine();
                                                                                switch (input)
                                                                                {
                                                                                    case "1":
                                                                                        Console.Write("Enter quantity to add to cart: ");
                                                                                        if (int.TryParse(Console.ReadLine(), out int quantity))
                                                                                        {
                                                                                            productHandler.AddToCart(viewDetailsId, customerIdCurrent, quantity, connection);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            Console.WriteLine("Invalid quantity. Please enter a valid number.");
                                                                                        }
                                                                                        break;
                                                                                    case "2":
                                                                                        // Exit 
                                                                                        break;
                                                                                    default:
                                                                                        Console.WriteLine("Invalid option, please try again.");
                                                                                        break;
                                                                                }
                                                                            } while (input != "2" && input != "1");
                                                                        }
                                                                        break;
                                                                    }
                                                                case "5"://them gio hang
                                                                    {
                                                                        int addProductIdCart;
                                                                        do
                                                                        {
                                                                            Console.Write("Enter the porduct id you want add to cart: ");
                                                                            string check = Console.ReadLine();
                                                                            if (!int.TryParse(check, out addProductIdCart))
                                                                            {
                                                                                Console.WriteLine("Id Isvalid. Please enter a valid number");
                                                                            }
                                                                            else break;
                                                                        } while (true);
                                                                        if (productHandler.ViewProductDetails(addProductIdCart, connection) == 1)
                                                                        {
                                                                            Console.Write("Enter quantity to add to cart: ");
                                                                            if (int.TryParse(Console.ReadLine(), out int quantity))
                                                                            {
                                                                                productHandler.AddToCart(addProductIdCart, customerIdCurrent, quantity, connection);
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Invalid quantity. Please enter a valid number.");
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Adding product to cart failed");
                                                                        }
                                                                        break;
                                                                    }
                                                                case "6"://thoat
                                                                    {
                                                                        Console.WriteLine("Exit search");
                                                                        pageNumberCurrent = 1;
                                                                        break;
                                                                    }
                                                                default:
                                                                    {
                                                                        Console.WriteLine("Selection is valid");
                                                                        break;
                                                                    }
                                                            }
                                                            if (selectOptionSearch == "4" || selectOptionSearch == "5" || selectOptionSearch == "6")
                                                            {
                                                                break;
                                                            }
                                                        } while (true);
                                                        break;
                                                    }
                                                case "2":
                                                    {// tim kiem theo danh muc 
                                                        pageNumberCurrent = 1;
                                                        productHandler.DisplayAllCategory(connection);
                                                        int categoryIdSearch;
                                                        do
                                                        {
                                                            Console.Write("Enter the id category: ");
                                                            string check = Console.ReadLine();
                                                            if (!int.TryParse(check, out categoryIdSearch))
                                                            {
                                                                Console.WriteLine("Selection Isvalid");
                                                            }
                                                            else break;
                                                        } while (true);
                                                        do
                                                        {
                                                            productHandler.SeaProductByCategory(categoryIdSearch, connection);
                                                            menu.SearchMenuOption();
                                                            Console.Write("Select an option: ");
                                                            string selectOptionSearch = Console.ReadLine();
                                                            switch (selectOptionSearch)
                                                            {
                                                                case "1":
                                                                    {
                                                                        pageNumberCurrent++;
                                                                        break;
                                                                    }
                                                                case "2":
                                                                    {
                                                                        pageNumberCurrent--;
                                                                        break;
                                                                    }
                                                                case "3":
                                                                    {
                                                                        int pageNumber;
                                                                        do
                                                                        {
                                                                            Console.Write("Select tha page you want to go to: ");
                                                                            string check = Console.ReadLine();
                                                                            if (!int.TryParse(check, out pageNumber))
                                                                            {
                                                                                Console.WriteLine("Selection Isvalid");
                                                                            }
                                                                            else pageNumberCurrent = pageNumber; break;
                                                                        } while (true);
                                                                        break;
                                                                    }
                                                                case "4"://xem chi tiet
                                                                    {
                                                                        int viewDetailsId;
                                                                        do
                                                                        {
                                                                            Console.Write("Enter the porduct id you want to view details: ");
                                                                            string check = Console.ReadLine();
                                                                            if (!int.TryParse(check, out viewDetailsId))
                                                                            {
                                                                                Console.WriteLine("Id Isvalid");
                                                                            }
                                                                            else break;
                                                                        } while (true);

                                                                        if (productHandler.ViewProductDetails(viewDetailsId, connection) == 1)
                                                                        {
                                                                            string input;
                                                                            do
                                                                            {
                                                                                Console.WriteLine("1 - Add product to cart");
                                                                                Console.WriteLine("2 - Back");
                                                                                Console.Write("Options: ");
                                                                                input = Console.ReadLine();
                                                                                switch (input)
                                                                                {
                                                                                    case "1":
                                                                                        Console.Write("Enter quantity to add to cart: ");
                                                                                        if (int.TryParse(Console.ReadLine(), out int quantity))
                                                                                        {
                                                                                            productHandler.AddToCart(viewDetailsId, customerIdCurrent, quantity, connection);
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            Console.WriteLine("Invalid quantity. Please enter a valid number.");
                                                                                        }
                                                                                        break;
                                                                                    case "2":
                                                                                        // Exit 
                                                                                        break;
                                                                                    default:
                                                                                        Console.WriteLine("Invalid option, please try again.");
                                                                                        break;
                                                                                }
                                                                            } while (input != "2" && input != "1");
                                                                        }
                                                                        break;
                                                                    }
                                                                case "5"://them gio hang
                                                                    {
                                                                        int addProductIdCart;
                                                                        do
                                                                        {
                                                                            Console.Write("Enter the porduct id you want add to cart: ");
                                                                            string check = Console.ReadLine();
                                                                            if (!int.TryParse(check, out addProductIdCart))
                                                                            {
                                                                                Console.WriteLine("Id Isvalid. Please enter a valid number");
                                                                            }
                                                                            else break;
                                                                        } while (true);
                                                                        if (productHandler.ViewProductDetails(addProductIdCart, connection) == 1)
                                                                        {
                                                                            Console.Write("Enter quantity to add to cart: ");
                                                                            if (int.TryParse(Console.ReadLine(), out int quantity))
                                                                            {
                                                                                productHandler.AddToCart(addProductIdCart, customerIdCurrent, quantity, connection);
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine("Invalid quantity. Please enter a valid number.");
                                                                            }
                                                                        }
                                                                        else
                                                                        {
                                                                            Console.WriteLine("Adding product to cart failed");
                                                                        }
                                                                        break;
                                                                    }
                                                                case "6"://thoat
                                                                    {
                                                                        Console.WriteLine("Exit search");
                                                                        pageNumberCurrent = 1;
                                                                        break;
                                                                    }
                                                                default:
                                                                    {
                                                                        Console.WriteLine("Selection is valid");
                                                                        break;
                                                                    }
                                                            }
                                                            if (selectOptionSearch=="4"||selectOptionSearch=="5"||selectOptionSearch=="6")
                                                            {
                                                                break;
                                                            }
                                                        } while (true);
                                                        break;
                                                    }
                                            case "3"://quit
                                                {
                                                    break;
                                                }
                                            default:
                                                {
                                                        Console.WriteLine("selection isvalid");
                                                        break;
                                                }
                                            }
                                            break; 
                                        }
                                    case "6":
                                        //view Cart
                                        cartHandler.ViewCart(customerIdCurrent, connection);
                                        Console.WriteLine("1.Update product in the cart");
                                        Console.WriteLine("2.Pay");
                                        Console.WriteLine("3.Back");
                                        string optionViewCart;
                                        do
                                        {
                                            Console.Write("Enter the option:");
                                            optionViewCart= Console.ReadLine();
                                            if (optionViewCart == "2" || optionViewCart == "1"||optionViewCart=="3")
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Selection isvalid");
                                            }
                                        }while(true);
                                        switch (optionViewCart)
                                        {
                                            case "1":
                                                {
                                                    cartHandler.UpdateProductToCart(customerIdCurrent, connection);
                                                    break;
                                                }
                                            case "2":
                                                {
                                                    orderHandler.Pay(connection, customerIdCurrent);
                                                    break;
                                                }
                                            case "3":
                                                {
                                                    Console.WriteLine("Leave view cart");
                                                    break;
                                                }
                                            default:
                                                {

                                                    break;
                                                }
                                        }
                                        break;
                                    case "7"://view infomation
                                        account.ViewInformationCustomer(customerIdCurrent, connection);
                                        Console.WriteLine("Information option");
                                        Console.WriteLine("1.Update Information");
                                        Console.WriteLine("2.Back");                                       
                                        string optionInformation;
                                        do
                                        {
                                            Console.Write("Enter option:");
                                            optionInformation=Console.ReadLine();
                                            if (optionInformation == "2" || optionInformation == "1")
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Selection isvalid");
                                            }
                                        }while(true);
                                        if (optionInformation == "1")
                                        {
                                            connection.Open();
                                            account.UpdateInformationCustomer(customerIdCurrent, connection);
                                            connection.Close();
                                        }
                                        break;
                                   //add product to cart                                  
                                    case "8":
                                        Console.WriteLine("Add product to cart.");
                                        int addProductId;
                                        do
                                        {
                                            Console.Write("Enter the porduct id you want add to cart: ");
                                            string check = Console.ReadLine();
                                            if (!int.TryParse(check, out addProductId))
                                            {
                                                Console.WriteLine("Id Isvalid. Please enter a valid number");
                                            }
                                            else break;
                                        } while (true);
                                        if (productHandler.ViewProductDetails(addProductId, connection) == 1)
                                        {
                                            Console.Write("Enter quantity to add to cart: ");
                                            if (int.TryParse(Console.ReadLine(), out int quantity))
                                            {
                                                productHandler.AddToCart(addProductId, customerIdCurrent, quantity, connection);
                                            }
                                            else
                                            {
                                                Console.WriteLine("Invalid quantity. Please enter a valid number.");
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Adding product to cart failed");
                                        }
                                        break;
                                    case "9"://view order
                                        orderHandler.ViewOrder(connection,customerIdCurrent);
                                        break;
                                    case "10"://exit
                                        Console.WriteLine("Exit.");
                                        break;
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
