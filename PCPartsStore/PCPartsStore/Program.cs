using System;
using Helpers;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using PC_Part_Store;
using PC_Part_Store.Implement;
using PCPartsStore;
using static PC_Part_Store.Implement.Product;

public static class Program
{
    public static int pageNumberCurrent {  get; set; }=1;
    public static int customerIdCurrent { get; set; }
    public static int pageCurrentEmloyee { get; set; } = 1;
    static void Main()
    {
        using (MySqlConnection connection = DBHelper.GetConnection())
        {
            Account account = new Account();
            MenuStore menu = new MenuStore();
            Product productHandler = new Product();
            Cart cartHandler = new Cart();
            Order orderHandler = new Order();
            Validations validations = new Validations();
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
                            pageNumberCurrent = 1;
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
                                            Console.Write("Enter Page number go to: ");
                                            pageNumber = validations.CheckInt();
                                            pageNumberCurrent = pageNumber;
                                            break;
                                        }
                                    case "4":
                                        {
                                            int viewDetailsId;
                                            Console.Write("Enter the id you want to see details: ");
                                            viewDetailsId =validations.CheckInt();
                                            
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
                                                            int quantity;
                                                            do
                                                            {
                                                                Console.Write("Enter quantity to add to cart: ");
                                                                quantity = validations.CheckInt();
                                                                if (quantity > 0)
                                                                {
                                                                    productHandler.AddToCart(viewDetailsId, customerIdCurrent, quantity, connection);
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Quantity must be a positive number.");
                                                                }
                                                            } while (true);
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
                                    case "5"://tim kiem
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
                                                        Console.Write("Enter name product: ");
                                                        string nameSearch= Console.ReadLine();
                                                        do
                                                        {
                                                            if(productHandler.SearchProductByName(nameSearch, connection) == 2)
                                                            {
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
                                                                            Console.Write("Select the page you want to go to: ");
                                                                            pageNumber = validations.CheckInt();
                                                                            pageNumberCurrent = pageNumber;
                                                                            break;
                                                                        }
                                                                    case "4"://xem chi tiet
                                                                        {
                                                                            int viewDetailsSearchId;
                                                                            Console.Write("Enter the id you want to see details: ");
                                                                            viewDetailsSearchId = validations.CheckInt();

                                                                            if (productHandler.ViewProductDetails(viewDetailsSearchId, connection) == 1)
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
                                                                                            int quantity;
                                                                                            do
                                                                                            {
                                                                                                Console.Write("Enter quantity to add to cart: ");
                                                                                                quantity = validations.CheckInt();
                                                                                                if (quantity > 0)
                                                                                                {
                                                                                                    productHandler.AddToCart(viewDetailsSearchId, customerIdCurrent, quantity, connection);
                                                                                                    break;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    Console.WriteLine("Quantity must be a positive number.");
                                                                                                }
                                                                                            } while (true);
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
                                                                            Console.Write("Enter the product id you want add to cart: ");
                                                                            addProductIdCart=validations.CheckInt();
                                                                            if (productHandler.ViewProductDetails(addProductIdCart, connection) == 1)
                                                                            {
                                                                                do
                                                                                {
                                                                                    Console.Write("Enter quantity to add to cart: ");
                                                                                    int quantity;
                                                                                    quantity = validations.CheckInt();
                                                                                    if (quantity > 0)
                                                                                    {
                                                                                        productHandler.AddToCart(addProductIdCart, customerIdCurrent, quantity, connection);
                                                                                        break;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Console.WriteLine("Quantity must be a positive number.");
                                                                                    }
                                                                                } while (true);
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
                                                            }
                                                            else
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
                                                                Console.WriteLine("Selection Invalid");
                                                            }
                                                            else break;
                                                        } while (true);
                                                        do
                                                        {
                                                            if (productHandler.SeaProductByCategory(categoryIdSearch, connection) == 2)
                                                            {
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
                                                                            Console.Write("Select the page you want to go to: ");
                                                                            pageNumber = validations.CheckInt();
                                                                            pageNumberCurrent = pageNumber;
                                                                            break;
                                                                        }
                                                                    case "4"://xem chi tiet
                                                                        {
                                                                            int viewDetailsSearchId;
                                                                            Console.Write("Enter the id you want to see details: ");
                                                                            viewDetailsSearchId = validations.CheckInt();
                                                                            if (productHandler.ViewProductDetails(viewDetailsSearchId, connection) == 1)
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
                                                                                            int quantity;
                                                                                            do
                                                                                            {
                                                                                                Console.Write("Enter quantity to add to cart: ");
                                                                                                quantity = validations.CheckInt();
                                                                                                if (quantity > 0)
                                                                                                {
                                                                                                    productHandler.AddToCart(viewDetailsSearchId, customerIdCurrent, quantity, connection);
                                                                                                    break;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    Console.WriteLine("Quantity must be a positive number.");
                                                                                                }
                                                                                            } while (true);
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
                                                                            Console.Write("Enter the product id you want add to cart: ");
                                                                            addProductIdCart = validations.CheckInt();
                                                                            if (productHandler.ViewProductDetails(addProductIdCart, connection) == 1)
                                                                            {
                                                                                do
                                                                                {
                                                                                    Console.Write("Enter quantity to add to cart: ");
                                                                                    int quantity;

                                                                                    quantity = validations.CheckInt();
                                                                                    if (quantity > 0)
                                                                                    {
                                                                                        productHandler.AddToCart(addProductIdCart, customerIdCurrent, quantity, connection);
                                                                                        break;
                                                                                    }
                                                                                    else
                                                                                    {
                                                                                        Console.WriteLine("Quantity must be a positive number.");
                                                                                    }
                                                                                } while (true);
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
                                                            }
                                                            else
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
                                                        Console.WriteLine("Selection Invalid");
                                                        break;
                                                }
                                            }
                                            break; 
                                        }
                                    case "6":
                                        //view Cart
                                        if(cartHandler.ViewCart(customerIdCurrent, connection) == 1)
                                        {
                                            Console.WriteLine("1.Update product in the cart");
                                            Console.WriteLine("2.Pay");
                                            Console.WriteLine("3.Back");
                                            string optionViewCart;
                                            do
                                            {
                                                Console.Write("Enter the option: ");
                                                optionViewCart = Console.ReadLine();
                                                if (optionViewCart == "2" || optionViewCart == "1" || optionViewCart == "3")
                                                {
                                                    break;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Selection Invalid");
                                                }
                                            } while (true);
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
                                        }
                                        else
                                        {
                                            Console.WriteLine("Would you like to add more products?");
                                            Console.WriteLine("1. Yes");
                                            Console.WriteLine("2. Back");
                                            string optionViewCart;
                                            do
                                            {
                                                Console.Write("Enter option: ");
                                                optionViewCart =Console.ReadLine();
                                                if (optionViewCart == "1" || optionViewCart == "2")
                                                {
                                                    break;
                                                }
                                                else
                                                {
                                                    Console.WriteLine("Selection invalid");
                                                }
                                            }while(true);
                                            if (optionViewCart == "1")
                                            {
                                                Console.WriteLine("Add product to cart.");
                                                int viewProductDetailsAdd;
                                                do
                                                {
                                                    Console.Write("Enter the product id you want add to cart: ");
                                                    string check = Console.ReadLine();
                                                    if (!int.TryParse(check, out viewProductDetailsAdd))
                                                    {
                                                        Console.WriteLine("Id Invalid. Please enter a valid number");
                                                    }
                                                    else break;
                                                } while (true);
                                                if (productHandler.ViewProductDetails(viewProductDetailsAdd, connection) == 1)
                                                {
                                                    Console.Write("Enter quantity to add to cart: ");
                                                    if (int.TryParse(Console.ReadLine(), out int quantity))
                                                    {
                                                        if (quantity > 0)
                                                        {
                                                            productHandler.AddToCart(viewProductDetailsAdd, customerIdCurrent, quantity, connection);
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("Quantity must be a positive number.");
                                                        }
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
                                            Console.Write("Enter option: ");
                                            optionInformation=Console.ReadLine();
                                            if (optionInformation == "2" || optionInformation == "1")
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Selection Invalid");
                                            }
                                        }while(true);
                                        if (optionInformation == "1")
                                        {
                                            connection.Close();
                                            account.UpdateInformationCustomer(customerIdCurrent, connection);
                                            //connection.Close();
                                        }
                                        break;
                                    //add product to cart                                  
                                    case "8":
                                        Console.WriteLine("Add product to cart.");
                                        int viewDetailsAddToCartId;
                                        Console.Write("Enter the id you want to see details: ");
                                        viewDetailsAddToCartId = validations.CheckInt();
                                        do
                                        {
                                            Console.Write("Enter quantity to add to cart: ");
                                            int quantity;

                                            quantity = validations.CheckInt();
                                            if (quantity > 0)
                                            {
                                                productHandler.AddToCart(viewDetailsAddToCartId, customerIdCurrent, quantity, connection);
                                                break;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Quantity must be a positive number.");
                                            }
                                        } while (true);
                                        break;
                                    case "9"://view order
                                        string optionViewOrder="0";
                                        do
                                        {
                                            if (orderHandler.ViewOrder(connection, customerIdCurrent) == 1)
                                            {
                                                Console.WriteLine("1. View order detail");
                                                Console.WriteLine("2. Back");

                                                do
                                                {
                                                    Console.Write("Enter option: ");
                                                    optionViewOrder = Console.ReadLine();
                                                    if (optionViewOrder == "1" || optionViewOrder == "2")
                                                    {
                                                        break;
                                                    }
                                                    else Console.WriteLine("selection invalid");
                                                } while (true);
                                                if (optionViewOrder == "1")
                                                {
                                                    int viewOrderId;
                                                    Console.Write("Enter the id you want to view: ");
                                                    viewOrderId=validations.CheckInt();                                                 
                                                    orderHandler.ViewOrderDetails(viewOrderId, connection);
                                                }
                                            }
                                        }while(optionViewOrder != "2");
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
                                pageNumberCurrent = 1;
                                do
                                {
                                    menu.EmployeeMenu();
                                    Console.Write("Select an option: ");
                                    string chooseEmployee = Console.ReadLine();
                                    switch (chooseEmployee)
                                    {
                                        //dong y thanh toan
                                        case "1":
                                            do
                                            {
                                                Console.WriteLine("Approve application selected.");
                                                orderHandler.Accepted(connection);
                                                string approveOption;
                                                do
                                                {
                                                    Console.Write("Continue approving the order(1.Yes/2.No): ");
                                                    approveOption = Console.ReadLine();
                                                    if (approveOption == "1" || approveOption == "2")
                                                    {
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Selection invalid");
                                                    }
                                                } while (true);
                                                if (approveOption == "2")
                                                {
                                                    break;
                                                }
                                            } while (true);
                                            break;
                                        //quan li san pham
                                        case "2":
                                            do
                                            {
                                                productHandler.ViewAllProduct(connection);
                                                menu.ProductManagemnt();
                                                Console.Write("Select an option: ");
                                                string productManageOption = Console.ReadLine();

                                                switch (productManageOption)
                                                {
                                                    case "1"://next
                                                        {
                                                            pageNumberCurrent++;
                                                            break;
                                                        }                                                    
                                                    case "2"://previous
                                                        {
                                                            pageNumberCurrent--;
                                                            break;
                                                        }                                                                                                    
                                                    case "3"://go to
                                                        {
                                                            int pageNumber;
                                                            do
                                                            {
                                                                Console.Write("Select the page you want to go to: ");
                                                                string check = Console.ReadLine();
                                                                if (!int.TryParse(check, out pageNumber))
                                                                {
                                                                    Console.WriteLine("Selection Invalid");
                                                                }
                                                                else pageNumberCurrent = pageNumber; break;
                                                            } while (true);
                                                            break;
                                                        }
                                                    case "4"://view details
                                                        {
                                                            int viewDetailsId;
                                                            do
                                                            {
                                                                Console.Write("Enter the product id you want to view details: ");
                                                                string check = Console.ReadLine();
                                                                if (!int.TryParse(check, out viewDetailsId))
                                                                {
                                                                    Console.WriteLine("Id Invalid");
                                                                }
                                                                else break;
                                                            } while (true);
                                                            if (productHandler.ViewProductDetails(viewDetailsId, connection) == 1)
                                                            {
                                                                Console.WriteLine("1. Update product");
                                                                //Console.WriteLine("2. Remove product");
                                                                Console.WriteLine("2. Back ");
                                                                
                                                                string employeeInput;
                                                                int checkEmployeeInput;
                                                                do
                                                                {
                                                                    Console.Write("Enter option: ");
                                                                    employeeInput = Console.ReadLine();
                                                                    if(!int.TryParse(employeeInput, out checkEmployeeInput))
                                                                    {
                                                                        if (checkEmployeeInput != 1 || checkEmployeeInput != 2 || checkEmployeeInput != 3)
                                                                        {
                                                                            Console.WriteLine("Selection Invalid");
                                                                        }                                                                        
                                                                    }
                                                                    else
                                                                    {
                                                                        break;
                                                                    }
                                                                } while (true);
                                                                if (checkEmployeeInput == 1)//update
                                                                {
                                                                    productHandler.Update(connection, viewDetailsId);
                                                                }
                                                                else//remove
                                                                {
                                                                    //productHandler.Remove(connection, viewDetailsId);
                                                                }
                                                            }
                                                            break;
                                                        }
                                                    case "5"://tim kiem
                                                        {
                                                            Console.WriteLine("Search product selected.");
                                                            menu.SearchMenu();
                                                            string selectSearch;
                                                            do
                                                            {
                                                                Console.Write("Enter selection: ");
                                                                selectSearch = Console.ReadLine();
                                                                if (selectSearch != "1" || selectSearch != "2" || selectSearch != "3")
                                                                {
                                                                    break;
                                                                }
                                                            } while (true);
                                                            switch (selectSearch)
                                                            {
                                                                case "1"://tim theo ten
                                                                    {
                                                                        pageNumberCurrent = 1;
                                                                        Console.Write("Enter name product: ");
                                                                        string nameSearch = Console.ReadLine();
                                                                        do
                                                                        {
                                                                            if (productHandler.SearchProductByName(nameSearch, connection) == 2)
                                                                            {
                                                                                menu.SearchMenuEmployee();
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
                                                                                                Console.Write("Select the page you want to go to: ");
                                                                                                string check = Console.ReadLine();
                                                                                                if (!int.TryParse(check, out pageNumber))
                                                                                                {
                                                                                                    Console.WriteLine("Selection Invalid");
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
                                                                                                Console.Write("Enter the product id you want to view details: ");
                                                                                                string check = Console.ReadLine();
                                                                                                if (!int.TryParse(check, out viewDetailsId))
                                                                                                {
                                                                                                    Console.WriteLine("Id Invalid");
                                                                                                }
                                                                                                else break;
                                                                                            } while (true);

                                                                                            if (productHandler.ViewProductDetails(viewDetailsId, connection) == 1)
                                                                                            {
                                                                                                string input;
                                                                                                do
                                                                                                {
                                                                                                    Console.WriteLine("1. Update product");
                                                                                                    Console.WriteLine("2. Delete product");
                                                                                                    Console.WriteLine("3. Back");
                                                                                                    Console.Write("Options: ");
                                                                                                    input = Console.ReadLine();
                                                                                                    switch (input)
                                                                                                    {
                                                                                                        case "1":
                                                                                                            {
                                                                                                                productHandler.Update(connection, viewDetailsId);
                                                                                                                break;
                                                                                                            }
                                                                                                        case "2":
                                                                                                            {
                                                                                                                productHandler.Remove(connection,viewDetailsId);
                                                                                                                break;
                                                                                                            }
                                                                                                        case "3":
                                                                                                            {
                                                                                                                break;
                                                                                                            }
                                                                                                        default:
                                                                                                            {
                                                                                                                Console.WriteLine("Invalid option, please try again.");
                                                                                                                break;
                                                                                                            }
                                                                                                    }
                                                                                                } while (input != "2" && input != "1");
                                                                                            }
                                                                                            break;
                                                                                        }                                                                                 
                                                                                    case "5"://thoat
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
                                                                                if (selectOptionSearch == "4" || selectOptionSearch == "5" )
                                                                                {
                                                                                    break;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                break;
                                                                            }
                                                                        } while (true);
                                                                        break;
                                                                    }
                                                                case "2":// tim kiem theo category
                                                                    {
                                                                        pageNumberCurrent = 1;
                                                                        productHandler.DisplayAllCategory(connection);
                                                                        int categoryIdSearch;
                                                                        do
                                                                        {
                                                                            Console.Write("Enter the id category: ");
                                                                            string check = Console.ReadLine();
                                                                            if (!int.TryParse(check, out categoryIdSearch))
                                                                            {
                                                                                Console.WriteLine("Selection Invalid");
                                                                            }
                                                                            else break;
                                                                        } while (true);
                                                                        do
                                                                        {
                                                                            if (productHandler.SeaProductByCategory(categoryIdSearch,connection)==2)
                                                                            {
                                                                                menu.SearchMenuEmployee();
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
                                                                                                Console.Write("Select the page you want to go to: ");
                                                                                                string check = Console.ReadLine();
                                                                                                if (!int.TryParse(check, out pageNumber))
                                                                                                {
                                                                                                    Console.WriteLine("Selection Invalid");
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
                                                                                                Console.Write("Enter the product id you want to view details: ");
                                                                                                string check = Console.ReadLine();
                                                                                                if (!int.TryParse(check, out viewDetailsId))
                                                                                                {
                                                                                                    Console.WriteLine("Id Invalid");
                                                                                                }
                                                                                                else break;
                                                                                            } while (true);

                                                                                            if (productHandler.ViewProductDetails(viewDetailsId, connection) == 1)
                                                                                            {
                                                                                                string input;
                                                                                                do
                                                                                                {
                                                                                                    Console.WriteLine("1. Update product");
                                                                                                    Console.WriteLine("2. Delete product");
                                                                                                    Console.WriteLine("3. Back");
                                                                                                    Console.Write("Options: ");
                                                                                                    input = Console.ReadLine();
                                                                                                    switch (input)
                                                                                                    {
                                                                                                        case "1":
                                                                                                            {
                                                                                                                productHandler.Update(connection, viewDetailsId);
                                                                                                                break;
                                                                                                            }
                                                                                                        case "2":
                                                                                                            {
                                                                                                                productHandler.Remove(connection, viewDetailsId);
                                                                                                                break;
                                                                                                            }
                                                                                                        case "3":
                                                                                                            {
                                                                                                                break;
                                                                                                            }
                                                                                                        default:
                                                                                                            {
                                                                                                                Console.WriteLine("Invalid option, please try again.");
                                                                                                                break;
                                                                                                            }
                                                                                                    }
                                                                                                } while (input != "2" && input != "1");
                                                                                            }
                                                                                            break;
                                                                                        }
                                                                                    case "5"://thoat
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
                                                                                if (selectOptionSearch == "4" || selectOptionSearch == "5")
                                                                                {
                                                                                    break;
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                break;
                                                                            }
                                                                        } while (true);
                                                                        break;
                                                                    }
                                                                case "3":
                                                                    {
                                                                        break;
                                                                    }
                                                                default:
                                                                    {
                                                                        Console.WriteLine("Selection is valid");
                                                                        break;
                                                                    }
                                                            }
                                                            break;
                                                        }
                                                    case "6"://them
                                                        {
                                                            productHandler.Add(connection);
                                                            break;
                                                        }
                                                    case "7":
                                                        {
                                                            Console.WriteLine("Back");
                                                            break;
                                                        }
                                                    default:
                                                        Console.WriteLine("Invalid option. Please try again.");
                                                        break;
                                                }
                                                if (productManageOption == "7")
                                                {
                                                    break;
                                                }
                                            } while (true);
                                            break;                                       
                                        case "3":
                                            {
                                                Console.WriteLine("Exiting employee menu.");
                                                break;
                                            }
                                        default:
                                            {
                                                Console.WriteLine("Invalid option. Please try again.");
                                                break;
                                            }
                                    }
                                    if (chooseEmployee == "3")
                                    {
                                        break;
                                    }
                                }while(true);
                            }
                            // admin
                            else
                            {
                                if (result == "admin")
                                {
                                    pageCurrentEmloyee = 1;
                                    do
                                    {
                                        account.ViewAllEmployee(connection);
                                        menu.AdminMenu();
                                        Console.Write("Select an option: ");
                                        string chooseAdmin = Console.ReadLine();
                                        switch (chooseAdmin)
                                        {
                                            case "1"://next page
                                                {
                                                    pageCurrentEmloyee++;
                                                    break;
                                                }
                                            case "2"://previous page
                                                {
                                                    pageCurrentEmloyee--;
                                                    break;
                                                }
                                            case "3"://goto page
                                                {
                                                    int pageNumber;
                                                    do
                                                    {
                                                        Console.Write("Select the page you want to go to: ");
                                                        string check = Console.ReadLine();
                                                        if (!int.TryParse(check, out pageNumber))
                                                        {
                                                            Console.WriteLine("Selection Invalid");
                                                        }
                                                        else pageCurrentEmloyee = pageNumber; break;
                                                    } while (true);
                                                    break;
                                                }
                                            case "4"://tao tai khoan nv
                                                {
                                                    Console.WriteLine("Create account employee");
                                                    account.CreateAccountEmloyee(connection);
                                                    break;
                                                }
                                            case "5"://view details
                                                {
                                                    int viewDetailsId;
                                                    do
                                                    {
                                                        Console.Write("Enter the employee id you want to view details: ");
                                                        string check = Console.ReadLine();
                                                        if (!int.TryParse(check, out viewDetailsId))
                                                        {
                                                            Console.WriteLine("Id Invalid");
                                                        }
                                                        else break;
                                                    } while (true);
                                                    if (account.ViewInformationEmployee(viewDetailsId, connection) == 1)
                                                    {
                                                        Console.WriteLine("1. Update Employee information");
                                                        Console.WriteLine("2. Remove Employee ");
                                                        Console.WriteLine("3. Back");
                                                        int adminInput;
                                                        string checkAdminInput;
                                                        do
                                                        {
                                                            Console.Write("Enter option: ");
                                                            checkAdminInput = Console.ReadLine();
                                                            if (!int.TryParse(checkAdminInput, out adminInput))
                                                            {
                                                                if (adminInput != 1 || adminInput != 2 || adminInput != 3)
                                                                {
                                                                    Console.WriteLine("Selection Invalid");
                                                                }
                                                            }
                                                            else
                                                            {
                                                                break;
                                                            }
                                                        } while (true);
                                                        if (adminInput == 1)//update
                                                        {
                                                            account.UpdateInformationEmployee(viewDetailsId, connection);
                                                        }
                                                        else
                                                        {
                                                            if(adminInput==2){
                                                                account.DeleteAccountEmployee(viewDetailsId, connection);
                                                            }
                                                        }
                                                    }
                                                    break;
                                                }
                                            case "6"://xoa nv
                                                {
                                                    int viewDetailsId;
                                                    do
                                                    {
                                                        Console.Write("Enter the employee id you want delete: ");
                                                        string check = Console.ReadLine();
                                                        if (!int.TryParse(check, out viewDetailsId))
                                                        {
                                                            Console.WriteLine("Id Invalid");
                                                        }
                                                        else break;
                                                    } while (true);
                                                    account.DeleteAccountEmployee(viewDetailsId, connection);
                                                    break;
                                                }
                                            case "7"://cap nhat
                                                {
                                                    int viewDetailsId;
                                                    do
                                                    {
                                                        Console.Write("Enter the employee id you want delete: ");
                                                        string check = Console.ReadLine();
                                                        if (!int.TryParse(check, out viewDetailsId))
                                                        {
                                                            Console.WriteLine("Id Invalid");
                                                        }
                                                        else break;
                                                    } while (true);
                                                    account.UpdateInformationEmployee(viewDetailsId, connection);
                                                    break;
                                                }
                                            case "8"://tim kiem
                                                {

                                                    pageCurrentEmloyee = 1;
                                                    Console.Write("Enter name employee: ");
                                                    string nameSearch = Console.ReadLine();
                                                    if(account.SearchEmployeeByName(nameSearch, connection)==1)
                                                    {
                                                        menu.SearchMenuEmployeeOption();
                                                        Console.Write("Select an option: ");
                                                        string selectOptionSearch = Console.ReadLine();
                                                        switch(selectOptionSearch)
                                                        {
                                                            case "1":
                                                                {
                                                                    pageCurrentEmloyee++;
                                                                    break;
                                                                }
                                                            case "2":
                                                                {
                                                                    pageCurrentEmloyee--; 
                                                                    break;
                                                                }
                                                            case "3":
                                                                {
                                                                    int pageNumber;
                                                                    do
                                                                    {
                                                                        Console.Write("Select the page you want to go to: ");
                                                                        string check = Console.ReadLine();
                                                                        if (!int.TryParse(check, out pageNumber))
                                                                        {
                                                                            Console.WriteLine("Selection Invalid");
                                                                        }
                                                                        else pageCurrentEmloyee = pageNumber; break;
                                                                    } while (true);
                                                                    break;
                                                                }
                                                            case "4"://xem chi tiet
                                                                {
                                                                    int viewDetailsId;
                                                                    do
                                                                    {
                                                                        Console.Write("Enter the employee id you want to view details: ");
                                                                        string check = Console.ReadLine();
                                                                        if (!int.TryParse(check, out viewDetailsId))
                                                                        {
                                                                            Console.WriteLine("Id Invalid");
                                                                        }
                                                                        else break;
                                                                    } while (true);
                                                                    if (account.ViewInformationEmployee(viewDetailsId, connection) == 1)
                                                                    {
                                                                        Console.WriteLine("1. Update information");
                                                                        Console.WriteLine("2. Remove information");
                                                                        Console.WriteLine("3. Back");
                                                                        int adminInput;
                                                                        string checkAdminInput;
                                                                        do
                                                                        {
                                                                            Console.Write("Enter option: ");
                                                                            checkAdminInput = Console.ReadLine();
                                                                            if (!int.TryParse(checkAdminInput, out adminInput))
                                                                            {
                                                                                if (adminInput != 1 || adminInput != 2 || adminInput != 3)
                                                                                {
                                                                                    Console.WriteLine("Selection Invalid");
                                                                                }
                                                                            }
                                                                            else
                                                                            {
                                                                                break;
                                                                            }
                                                                        } while (true);
                                                                        if (adminInput == 1)//update
                                                                        {
                                                                            account.UpdateInformationEmployee(viewDetailsId, connection);
                                                                        }
                                                                        else
                                                                        {
                                                                            if (adminInput == 2)
                                                                            {
                                                                                account.DeleteAccountEmployee(viewDetailsId, connection);
                                                                            }
                                                                        }
                                                                    }
                                                                    break;
                                                                }
                                                            case "5":
                                                                {
                                                                    break;
                                                                }
                                                            default:
                                                                {
                                                                    Console.WriteLine("Selection is valid");
                                                                    break;
                                                                }
                                                        }
                                                    }
                                                    else
                                                    {

                                                    }
                                                    break;
                                                }
                                        }
                                        if (chooseAdmin == "9")
                                        {
                                            break;
                                        }
                                    }while(true);
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
