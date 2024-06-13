using Helpers;
using MySql.Data.MySqlClient;
using PC_Part_Store.Interface;
using System.Transactions;

namespace PC_Part_Store.Implement
{
    internal class Product : Super<Product>, IProduct
    {      
        public int productId { get; set; }
        public string productName { get; set; }
        public string descriptionProduct { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public string brand { get; set; }
        public int categoryId { get; set; }
        public override void Add(MySqlConnection connection)
        {
            connection = DBHelper.GetConnection();
            {
                if (connection == null)
                {
                    throw new ArgumentNullException(nameof(connection));
                }

                Console.WriteLine("Add product");
                Console.Write("Enter Id Product: ");
                productId = int.Parse(Console.ReadLine());
                Console.Write("Enter name product: ");
                productName = Console.ReadLine();
                Console.Write("Enter description product: ");
                descriptionProduct = Console.ReadLine();
                Console.Write("Enter price product: ");
                price = decimal.Parse(Console.ReadLine());
                Console.Write("Enter quantity product: ");
                quantity = int.Parse(Console.ReadLine());
                Console.Write("Enter brand product: ");
                brand = Console.ReadLine();
                Console.Write("Enter category id: ");
                categoryId = int.Parse(Console.ReadLine());
                //Might need to check product information

                string query = "INSERT INTO product (description, name, price, quantity, productId, categoriesId, brand) " +
                               "VALUES (@description, @name, @price, @quantity, @productId, @categoriesId, @brand)";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@description", descriptionProduct);
                    cmd.Parameters.AddWithValue("@name", productName);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@categoriesId", categoryId);
                    cmd.Parameters.AddWithValue("@brand", brand);

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Product added successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        throw;
                    }
                }
            }
        }

        public void AddToCart(int productId, int customerId,int amount, MySqlConnection connection)
        {
            try
            {
                connection.Open();
                using(MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string checkCartQuery = "SELECT cartId FROM cart WHERE customerId=@customerId";
                        int cartId;
                        //kiem tra khach hang da co gio hang chua de lay id cart
                        using(MySqlCommand checkCartCmd=new MySqlCommand(checkCartQuery, connection,transaction))
                        {
                            checkCartCmd.Parameters.AddWithValue("@customerId", customerId);
                            var result = checkCartCmd.ExecuteScalar();
                            if (result != null)
                            {
                                cartId = Convert.ToInt32(result);
                            }
                            else
                            {
                                string query = "INSERT INTO cart (customer_id) VALUES (@customerId)";
                                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                {
                                    cmd.Parameters.AddWithValue("@customerId", customerId);
                                    cmd.ExecuteNonQuery();
                                    cartId = Convert.ToInt32(cmd.LastInsertedId);
                                }
                            }                            
                        }
                        //viet them dieu kien kiem tra id product da co trong cart chua
                        string addToCartQuery = @"INSERT INTO cartDetails(cartId,productId,amount) VALUES (@cartId,@productId,@amount)";
                        using (MySqlCommand cmdAddToCart = new MySqlCommand(addToCartQuery, connection, transaction))
                        {
                            cmdAddToCart.Parameters.AddWithValue("@cartId", cartId);
                            cmdAddToCart.Parameters.AddWithValue("@productId", productId);
                            cmdAddToCart.Parameters.AddWithValue("@amount", amount);
                            cmdAddToCart.ExecuteNonQuery();
                        }
                        /*cap nhat so luong hang trong kho
                        string updateQuantityProduct = "UPDATE product SET quantity=quantity-@amount WHERE productId=@productId";
                        using (MySqlCommand cmdUpdateQuantity = new MySqlCommand(updateQuantityProduct, connection, transaction))
                        {
                            cmdUpdateQuantity.Parameters.AddWithValue("@productId", productId);
                            cmdUpdateQuantity.Parameters.AddWithValue("@amount", amount);
                            cmdUpdateQuantity.ExecuteNonQuery();
                        }*/
                        transaction.Commit();
                        Console.WriteLine("Add product to cart sucessful");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
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

        public override void Remove(MySqlConnection connection, int id)
        {


                string query = "DELETE FROM product WHERE productId = @id";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    try
                    {
                        connection.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            Console.WriteLine("Record deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("No record found with the specified Id.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred: {ex.Message}");
                        throw;
                    }
                }
        }
        public void SeaProductByCategory(int categoryId, MySqlConnection connection)
        {
            int pageSize = 10;
            int totalRecords = 0;
            int pageNumber = 1;
            if (connection == null)
            {

                throw new ArgumentNullException(nameof(connection));
            }
            string countQuery = "SELECT COUNT(*) FROM product WHERE categoriesId = @categoryId";
            using (MySqlCommand countCmd = new MySqlCommand(countQuery, connection))
            {
                countCmd.Parameters.AddWithValue("categoriesId", categoryId);
                try
                {
                    connection.Open();
                    totalRecords = Convert.ToInt32(countCmd.ExecuteScalar());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    throw;
                }
                finally
                {
                    connection.Clone();
                }
                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
                while (true)
                {
                    if (pageNumber < 1)
                    {
                        Console.WriteLine("Page number must be greater than 0. Setting to page 1.");
                        pageNumber = 1;
                    }
                    else if (pageNumber > totalPages)
                    {
                        Console.WriteLine("Page number exceeds total pages. Setting to last page.");
                        pageNumber = totalPages;
                    }
                    int offset = (pageNumber - 1) * pageSize;
                    string query = "SELECT productId, name, description, price, quantity, brand, categoriesId " +
                           "FROM product WHERE categoriesId = @categoryId " +
                           "LIMIT @pageSize OFFSET @offset";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@categoryId", categoryId);
                        cmd.Parameters.AddWithValue("@pageSize", pageSize);
                        cmd.Parameters.AddWithValue("@offset", offset);
                        try
                        {
                            connection.Open();
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                List<Product> products = new List<Product>();
                                while (reader.Read())
                                {
                                    Product product = new Product
                                    {
                                        productId = reader.GetInt32("productId"),
                                        productName = reader.GetString("name"),
                                        descriptionProduct = reader.GetString("description"),
                                        price = reader.GetDecimal("price"),
                                        quantity = reader.GetInt32("quantity"),
                                        brand = reader.GetString("brand"),
                                        categoryId = reader.GetInt32("categoriesId")
                                    };
                                    products.Add(product);
                                }
                                if (products.Count == 0)
                                {
                                    Console.WriteLine("No products found in this category.");
                                }
                                else
                                {
                                    foreach (var product in products)
                                    {
                                        Console.WriteLine($"ID: {product.productId}, Name: {product.productName}, Description: {product.descriptionProduct}, " +
                                                  $"Price: {product.price}, Quantity: {product.quantity}, Brand: {product.brand}, Category ID: {product.categoryId}");
                                    }
                                }
                                Console.WriteLine($"Page {pageNumber} of {totalPages}");
                                Console.WriteLine("Options:");
                                Console.WriteLine("1 - Previous page");
                                Console.WriteLine("2 - Next page");
                                Console.WriteLine("3 - destination page option");
                                Console.WriteLine("4 - Quit");
                                string input = Console.ReadLine().ToUpper();
                                if (input == "1")
                                {
                                    pageNumber--;
                                }
                                else
                                {
                                    if (input == "2")
                                    {
                                        pageNumber++;
                                    }
                                    else
                                    {
                                        if (input == "3")
                                        {
                                            pageNumber = int.Parse(Console.ReadLine());
                                        }
                                        else
                                        {

                                            if (input == "4")
                                            {
                                                break;
                                            }
                                            else
                                            {
                                                Console.WriteLine("Invalid option, please try again.");
                                            }
                                        }
                                    }
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
            }
        }

        public void SearchProductById(int idFind, MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public void SearchProductByName(string name, MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public override void Update(MySqlConnection connection, int id)
        {
            Console.WriteLine("Update product");
            Console.Write("Enter new name product: ");
            string newProductName = Console.ReadLine();
            Console.Write("Enter new description product: ");
            string newDescriptionProduct = Console.ReadLine();
            Console.Write("Enter new price product: ");
            decimal newPrice = decimal.Parse(Console.ReadLine());
            Console.Write("Enter new quantity product: ");
            int newQuantity = int.Parse(Console.ReadLine());
            Console.Write("Enter new brand product: ");
            string newBrand = Console.ReadLine();
            Console.Write("Enter new category id: ");
            int newCategoryId = int.Parse(Console.ReadLine());
            string query = "UPDATE product SET name = @name, description = @description, price = @price, " +
                       "quantity = @quantity, brand = @brand, categoriesId = @categoriesId WHERE productId = @productId";
            using (MySqlCommand cmd=new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@productName", newProductName);
                cmd.Parameters.AddWithValue("@description", newDescriptionProduct);
                cmd.Parameters.AddWithValue("@price", newPrice);
                cmd.Parameters.AddWithValue("@quantity", newQuantity);
                cmd.Parameters.AddWithValue("@brand" ,newBrand);
                cmd.Parameters.AddWithValue("@categoriesId", newCategoryId);
                cmd.Parameters.AddWithValue("@productId", id);
                try
                {
                    connection.Open();
                    int rowsAffected=cmd.ExecuteNonQuery();
                    if(rowsAffected > 0)
                    {
                        Console.WriteLine("Record update sucessfully.");
                    }
                    else
                    {
                        Console.WriteLine("No record found with the specified Id");
                    }
                }
                catch(Exception ex)
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
        public void ManageProductOptions(int manageProductId, MySqlConnection connection)
        {
            while (true)
            {
                Console.WriteLine("1. Add Product");
                Console.WriteLine("2. Update Product");
                Console.WriteLine("3. Delete Product");
                Console.WriteLine("4. Back");
                Console.Write("Select an option: ");
                string manageOption = Console.ReadLine();

                switch (manageOption)
                {
                    case "1":

                        break;
                    case "2":

                        break;
                    case "3":

                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }
        public void ViewAllProduct(MySqlConnection connection)
        {
            string query = "SELECT * FROM product";
            MySqlCommand cmd = new MySqlCommand(query, connection);

            connection.Open();
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["productId"]}, Name: {reader["name"]}, Price: {reader["price"]}, Description: {reader["description"]}, Quantity: {reader["quantity"]}, Brand: {reader["brand"]}, Category ID: {reader["categoriesId"]}");
            }
            connection.Close();
        }

        public void viewProductDetails(int productId, MySqlConnection connection)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            string query = "SELECT * FROM product WHERE productId = @productId";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@productId", productId);

                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine($"ID: {reader["productId"]}");
                            Console.WriteLine($"Name: {reader["name"]}");
                            Console.WriteLine($"Price: {reader["price"]}");
                            Console.WriteLine($"Description: {reader["description"]}");
                            Console.WriteLine($"Quantity: {reader["quantity"]}");
                            Console.WriteLine($"Brand: {reader["brand"]}");
                            Console.WriteLine($"Category ID: {reader["categoriesId"]}");

                            // Option
                            Console.WriteLine("Options:");
                            Console.WriteLine("1 - Add product to cart");
                            Console.WriteLine("2 - Back");
                            Console.WriteLine("3 - Quit");

                            string input = Console.ReadLine();
                            switch (input)
                            {
                                case "1":
                                    Console.Write("Enter quantity to add to cart: ");
                                    if (int.TryParse(Console.ReadLine(), out int quantity))
                                    {
                                        Console.Write("Enter customer ID: ");
                                        if (int.TryParse(Console.ReadLine(), out int customerId))
                                        {
                                            AddToCart(productId, customerId, quantity, connection);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid customer ID. Please enter a valid number.");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Invalid quantity. Please enter a valid number.");
                                    }
                                    break;
                                case "2":
                                    // Exit 
                                    return;
                                case "3":
                                    Environment.Exit(0); // Quit 
                                    break;
                                default:
                                    Console.WriteLine("Invalid option, please try again.");
                                    break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Product not found.");
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

    }
}
