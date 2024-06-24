using Helpers;
using MySql.Data.MySqlClient;
using PC_Part_Store.Interface;
using System.Transactions;
using static Program;

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
        public string categoryName { get; set; }
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

                string query = "INSERT INTO product (Product_ID,Product_Name,Description , Price, Quantity,Brand,Category_Id) " +
                               "VALUES (@productId,@productName,@description,@price,@quantity,@brand,@categoryId)";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@productId", productId);
                    cmd.Parameters.AddWithValue("@productName", productName); 
                    cmd.Parameters.AddWithValue("@description", descriptionProduct);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@quantity", quantity);                                      
                    cmd.Parameters.AddWithValue("@brand", brand);
                    cmd.Parameters.AddWithValue("@categoryId", categoryId);
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

        public void AddToCart(int productId, int customerId, int amount, MySqlConnection connection)
        {
            try
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Kiểm tra số lượng hàng có sẵn trong kho
                        string checkAmountQuery = "SELECT Quantity FROM product WHERE Product_ID = @productId";
                        int availableQuantity;
                        using (MySqlCommand checkAmountCmd = new MySqlCommand(checkAmountQuery, connection, transaction))
                        {
                            checkAmountCmd.Parameters.AddWithValue("@productId", productId);
                            object resultCheckAmount = checkAmountCmd.ExecuteScalar();
                            if (resultCheckAmount != null)
                            {
                                availableQuantity = Convert.ToInt32(resultCheckAmount);
                                if (availableQuantity < amount)
                                {
                                    Console.WriteLine("Not enough products in stock.");
                                    transaction.Rollback();
                                    return;
                                }
                            }
                            else
                            {
                                Console.WriteLine("Product not found.");
                                transaction.Rollback();
                                return;
                            }
                        }

                        // Kiểm tra xem khách hàng đã có giỏ hàng chưa
                        string checkCartQuery = "SELECT Cart_ID FROM cart WHERE Customer_ID = @customerId";
                        int cartId;
                        using (MySqlCommand checkCartCmd = new MySqlCommand(checkCartQuery, connection, transaction))
                        {
                            checkCartCmd.Parameters.AddWithValue("@customerId", customerId);
                            object result = checkCartCmd.ExecuteScalar();
                            if (result != null)
                            {
                                cartId = Convert.ToInt32(result);
                            }
                            else
                            {
                                // Nếu chưa có giỏ hàng, thêm mới giỏ hàng cho khách hàng
                                string insertCartQuery = "INSERT INTO cart (Customer_Id) VALUES (@customerId)";
                                using (MySqlCommand insertCartCmd = new MySqlCommand(insertCartQuery, connection, transaction))
                                {
                                    insertCartCmd.Parameters.AddWithValue("@customerId", customerId);
                                    insertCartCmd.ExecuteNonQuery();
                                    cartId = (int)insertCartCmd.LastInsertedId;
                                }
                            }
                        }
                        // Kiểm tra xem sản phẩm đã có trong giỏ hàng chi tiết chưa
                        string checkCartDetailQuery = "SELECT Amount FROM cart_detail WHERE Cart_ID = @cartId AND Product_ID = @productId";
                        using (MySqlCommand checkCartDetailCmd = new MySqlCommand(checkCartDetailQuery, connection, transaction))
                        {
                            checkCartDetailCmd.Parameters.AddWithValue("@cartId", cartId);
                            checkCartDetailCmd.Parameters.AddWithValue("@productId", productId);
                            object result = checkCartDetailCmd.ExecuteScalar();
                            if (result != null)
                            {
                                // Nếu sản phẩm đã có trong giỏ hàng, hỏi người dùng có muốn cập nhật số lượng hay không
                                Console.Write("This product is already in your shopping cart. Do you want to add the quantity you just entered? (1.Yes/2.No)");
                                string input = Console.ReadLine();
                                if (input == "1")
                                {
                                    // Cập nhật số lượng sản phẩm trong giỏ hàng chi tiết
                                    int currentAmount = Convert.ToInt32(result);
                                    int newAmount = amount + currentAmount; // Số lượng mới cần cập nhật

                                    // Kiểm tra xem số lượng mới có vượt quá số lượng hàng có sẵn trong kho không
                                    string checkQuantityInStockQuery = "SELECT Quantity FROM product WHERE Product_ID = @productId";
                                    using (MySqlCommand checkQuantityInStockCmd = new MySqlCommand(checkQuantityInStockQuery, connection, transaction))
                                    {
                                        checkQuantityInStockCmd.Parameters.AddWithValue("@productId", productId);
                                        object resultQuantity = checkQuantityInStockCmd.ExecuteScalar();
                                        if (resultQuantity != null)
                                        {
                                            availableQuantity = Convert.ToInt32(resultQuantity);
                                            if (newAmount > availableQuantity)
                                            {
                                                Console.WriteLine("Not enough products in stock to update quantity.");
                                                transaction.Rollback();
                                                return;
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Product not found in stock.");
                                            transaction.Rollback();
                                            return;
                                        }
                                    }

                                    // Nếu đủ hàng trong kho, thực hiện cập nhật số lượng sản phẩm trong giỏ hàng chi tiết
                                    string updateCartDetailQuery = "UPDATE cart_detail SET Amount = @amount WHERE Cart_ID = @cartId AND Product_ID = @productId";
                                    using (MySqlCommand updateCartDetailCmd = new MySqlCommand(updateCartDetailQuery, connection, transaction))
                                    {
                                        updateCartDetailCmd.Parameters.AddWithValue("@cartId", cartId);
                                        updateCartDetailCmd.Parameters.AddWithValue("@productId", productId);
                                        updateCartDetailCmd.Parameters.AddWithValue("@amount", newAmount); // Cập nhật số lượng mới
                                        updateCartDetailCmd.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    transaction.Rollback();
                                    Console.WriteLine("Add product to cart canceled by user.");
                                    return;
                                }
                            }
                            else
                            {
                                // Nếu sản phẩm chưa có trong giỏ hàng, thêm mới vào giỏ hàng chi tiết
                                string addToCartQuery = "INSERT INTO cart_detail (Cart_ID, Product_ID, Amount) VALUES (@cartId, @productId, @amount)";
                                using (MySqlCommand cmdAddToCart = new MySqlCommand(addToCartQuery, connection, transaction))
                                {
                                    cmdAddToCart.Parameters.AddWithValue("@cartId", cartId);
                                    cmdAddToCart.Parameters.AddWithValue("@productId", productId);
                                    cmdAddToCart.Parameters.AddWithValue("@amount", amount);
                                    cmdAddToCart.ExecuteNonQuery();
                                }
                            }
                        }

                        // Cập nhật số lượng hàng trong kho (nếu cần)
                        // string updateQuantityProduct = "UPDATE product SET Quantity = Quantity - @amount WHERE Product_ID = @productId";
                        // using (MySqlCommand cmdUpdateQuantity = new MySqlCommand(updateQuantityProduct, connection, transaction))
                        // {
                        //     cmdUpdateQuantity.Parameters.AddWithValue("@productId", productId);
                        //     cmdUpdateQuantity.Parameters.AddWithValue("@amount", amount);
                        //     cmdUpdateQuantity.ExecuteNonQuery();
                        // }

                        transaction.Commit();
                        Console.WriteLine("Add product to cart successful");
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
            string queryProducts = "SELECT * FROM product WHERE Category_ID=@categoryId";
            string queryCategoryName = "SELECT Category_Name FROM category WHERE Category_ID=@categoryId";

            List<Product> products = new List<Product>();
            string categoryNameSearch = "";

            using (MySqlCommand cmdCategoryName = new MySqlCommand(queryCategoryName, connection))
            {
                cmdCategoryName.Parameters.AddWithValue("@categoryId", categoryId);
                try
                {
                    connection.Open();
                    categoryNameSearch = cmdCategoryName.ExecuteScalar()?.ToString();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while retrieving category name: {ex.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }

            using (MySqlCommand cmdProducts = new MySqlCommand(queryProducts, connection))
            {
                cmdProducts.Parameters.AddWithValue("@categoryId", categoryId);

                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmdProducts.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                productId = reader.GetInt32("Product_ID"),
                                productName = reader.GetString("Product_Name"),
                                descriptionProduct = reader.GetString("Description"),
                                price = reader.GetDecimal("Price"),
                                quantity = reader.GetInt32("Quantity"),
                                brand = reader.GetString("Brand"),
                                categoryName=categoryNameSearch
                            };
                            products.Add(product);
                        }
                    }
                    Console.WriteLine($"Category: {categoryName}");
                    DisplayProductsByPage(products, connection);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while retrieving products: {ex.Message}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
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
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@productName", newProductName);
                cmd.Parameters.AddWithValue("@description", newDescriptionProduct);
                cmd.Parameters.AddWithValue("@price", newPrice);
                cmd.Parameters.AddWithValue("@quantity", newQuantity);
                cmd.Parameters.AddWithValue("@brand", newBrand);
                cmd.Parameters.AddWithValue("@categoriesId", newCategoryId);
                cmd.Parameters.AddWithValue("@productId", id);
                try
                {
                    connection.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Record update sucessfully.");
                    }
                    else
                    {
                        Console.WriteLine("No record found with the specified Id");
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
        public void ViewAllProduct(MySqlConnection connection)
        {
            string query = @" SELECT p.Product_ID, p.Product_Name, p.Description, p.Price, p.Quantity, p.Brand, p.Category_ID, c.Category_Name FROM  product p INNER JOIN category c ON p.Category_ID = c.Category_ID";

            List<Product> products = new List<Product>();

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Product product = new Product
                            {
                                productId = reader.GetInt32("Product_ID"),
                                productName = reader.GetString("Product_Name"),
                                descriptionProduct = reader.GetString("Description"),
                                price = reader.GetDecimal("Price"),
                                quantity = reader.GetInt32("Quantity"),
                                brand = reader.GetString("Brand"),
                                categoryId = reader.GetInt32("Category_ID"),
                                categoryName = reader.GetString("Category_Name")
                            };
                            products.Add(product);
                        }
                    }
                    DisplayProductsByPage(products, connection);
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


        public int ViewProductDetails(int productId, MySqlConnection connection)
        {
            string query = @"SELECT p.Product_ID,p.Product_Name,p.Description,p.Price,p.Quantity,p.Brand,p.Category_ID,c.Category_Name FROM product p INNER JOIN category c ON p.Category_ID = c.Category_ID WHERE p.Product_ID = @productId";

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
                            Console.WriteLine($"ID: {reader["Product_ID"]}");
                            Console.WriteLine($"Name: {reader["Product_Name"]}");
                            Console.WriteLine($"Price: {reader["Price"]}");
                            Console.WriteLine($"Description: {reader["Description"]}");
                            Console.WriteLine($"Quantity: {reader["Quantity"]}");
                            Console.WriteLine($"Brand: {reader["Brand"]}");
                            Console.WriteLine($"Category: {reader["Category_Name"]}");
                            // Option
                            return 1;
                        }
                        else
                        {
                            Console.WriteLine("Product not found.");
                            return 0;
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


        public void DisplayProductsByPage(List<Product> products, MySqlConnection connection)
        {
            if (products.Count == 0)
            {
                Console.WriteLine("List product iss empty");
            }
            else
            {
                //chinh sua phan trang muon test
                int pageSize = 1;
                int totalRecords = products.Count;
                int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
               
                if (pageNumberCurrent < 1)
                {
                    Console.WriteLine("Page number must be greater than 0. Setting to page 1.");
                    pageNumberCurrent = 1;
                }
                else if (pageNumberCurrent > totalPages)
                {
                    Console.WriteLine("Page number exceeds total pages. Setting to last page.");
                    pageNumberCurrent = totalPages;
                }

                int start = (pageNumberCurrent - 1) * pageSize;
                int end = Math.Min(start + pageSize, totalRecords);

                    

                for (int i = start; i < end; i++)
                {
                    Product product = products[i];
                    Console.WriteLine($"ID: {product.productId}, Name: {product.productName}, Description: {product.descriptionProduct}, " +
                                                    $"Price: {product.price}, Quantity: {product.quantity}, Brand: {product.brand}, Category Name: {product.categoryName}");
                }
                Console.WriteLine($"Page {pageNumberCurrent} of {totalPages}");                                             
            }
        }
        public void DisplayAllCategory(MySqlConnection connection)
        {
            string query = "SELECT Category_ID, Category_Name FROM category";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                try
                {
                    connection.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int categoryId = reader.GetInt32("Category_ID");
                            string categoryName = reader.GetString("Category_Name");

                            // Display category directly
                            Console.WriteLine($"ID: {categoryId}, Name: {categoryName}");
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
