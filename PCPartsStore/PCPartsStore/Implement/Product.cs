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
            if (connection == null) { 
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
                    Console.WriteLine("Error: " + ex.Message);
                    throw;
                }
                finally
                {
                    connection.Close();
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
                        //cap nhat so luong hang trong kho
                        string updateQuantityProduct = "UPDATE product SET quantity=quantity-@amount WHERE productId=@productId";
                        using (MySqlCommand cmdUpdateQuantity = new MySqlCommand(updateQuantityProduct, connection, transaction))
                        {
                            cmdUpdateQuantity.Parameters.AddWithValue("@productId", productId);
                            cmdUpdateQuantity.Parameters.AddWithValue("@amount", amount);
                            cmdUpdateQuantity.ExecuteNonQuery();
                        }
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
            string querry = "DELETE FROMM product WHERE productId=@id";
            using (MySqlCommand cmd=new MySqlCommand(querry, connection))
            {
                cmd.Parameters.AddWithValue("@productId", id);
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
                finally
                {
                    connection.Close();
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

        public void ViewAllProduct(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
