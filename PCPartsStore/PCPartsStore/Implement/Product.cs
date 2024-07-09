using Helpers;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using PC_Part_Store.Interface;
using PCPartsStore;
using System.Data;
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
        Validations validation = new Validations();
        public override void Add(MySqlConnection connection)
        {
            string check;
            Console.WriteLine("Add product");
            Console.Write("Enter Id Product: ");
            productId = validation.CheckInt();
            while (validation.ProductExistCheck(productId, connection))
            {
                Console.WriteLine("The product id is duplicated, do you want to update the product?");
                Console.WriteLine("1. Update product");
                Console.WriteLine("2. Rewrite ID");
                Console.WriteLine("3. Back");
                string optionAdd="0";
                do
                {
                    Console.Write("Enter Option: ");
                    optionAdd = Console.ReadLine();
                    if (optionAdd != "1" || optionAdd != "2"|| optionAdd!="3") break;
                    else Console.WriteLine("Selection invalid");
                } while (true);
                if(optionAdd == "1")
                {
                    Console.WriteLine("Product details ");
                    ViewProductDetails(productId, connection);
                    Update(connection,productId);
                }
                else
                {
                    if(optionAdd == "2") 
                    {
                        Console.Write("Enter Id Product: ");
                        productId = validation.CheckInt();
                    }
                    else
                    {
                        return;
                    }
                }
            }
            Console.Write("Enter name product: ");
            productName = Console.ReadLine();
            Console.Write("Enter description product: ");
            descriptionProduct = Console.ReadLine();
            decimal priceInput;
            do
            {
                Console.Write("Enter price product: ");
                if (decimal.TryParse(Console.ReadLine(), out priceInput))
                {
                    // Điều kiện kiểm tra giá sản phẩm
                    if (priceInput > 0)
                    {
                        price= priceInput;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("The price must be greater than 0. Please try again.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid decimal number.");
                }

            } while (true);
            do
            {
                Console.Write("Enter quantity product: ");
                quantity = validation.CheckInt();
                if (quantity > 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Please enter a number greater than 0");
                }
            } while (true);
            Console.Write("Enter brand product: ");
            brand = Console.ReadLine();
            do
            {
                Console.Write("Enter category id: ");
                categoryId = validation.CheckInt();
                if (!validation.CheckCategoryExist(categoryId, connection))
                {
                    Console.WriteLine("Do you want to re-enter the category id");
                    Console.WriteLine("1. Re enter the category id");
                    Console.WriteLine("2. Create Category");
                    Console.WriteLine("3. Back to menu");
                    int optionAddCategory;
                    do
                    {
                        Console.Write("Enter option: ");
                        optionAddCategory = validation.CheckInt();
                        if (optionAddCategory == 1 || optionAddCategory == 2 || optionAddCategory == 3)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Selection invalid");
                        }
                    } while (true);
                    if (optionAddCategory == 1)
                    {
                        //nhap lai
                    }
                    else if(optionAddCategory==2)
                    {
                        CreateCategory(connection,categoryId);
                        break;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    break;
                }
            } while (true);
            //Might need to check product information

            string query = "INSERT INTO product (Product_ID,Product_Name,Description,Price, Quantity,Brand,Category_Id) " +
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
                finally
                {
                    connection.Close();
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

                        //Cập nhật số lượng hàng trong kho(nếu cần)
                        string updateQuantityProduct = "UPDATE product SET Quantity = Quantity - @amount WHERE Product_ID = @productId";
                        using (MySqlCommand cmdUpdateQuantity = new MySqlCommand(updateQuantityProduct, connection, transaction))
                        {
                            cmdUpdateQuantity.Parameters.AddWithValue("@productId", productId);
                            cmdUpdateQuantity.Parameters.AddWithValue("@amount", amount);
                            cmdUpdateQuantity.ExecuteNonQuery();
                        }

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
            //throw new NotImplementedException();
            string queryCart = "Delete from cart_detail where product_id=@productId";
            using (MySqlCommand cmdCart = new MySqlCommand(queryCart, connection))
            {
                cmdCart.Parameters.AddWithValue("@productId", id);
                try
                {
                    connection.Open();
                    int rowsAffected = cmdCart.ExecuteNonQuery();

                    if (rowsAffected < 0)
                    {
                        Console.WriteLine("No record found with the specified Id.");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    throw;
                }
                finally { connection.Close(); }
            }
            string query = "DELETE FROM product WHERE Product_ID = @id";

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
                finally { connection.Close(); }
            }
        }
        public int SeaProductByCategory(int categoryId, MySqlConnection connection)
        {
            string queryProducts = "SELECT * FROM product WHERE Category_ID=@categoryId";
            string queryCategoryName = "SELECT Category_Name FROM category WHERE Category_ID=@categoryId";

            List<Product> products = new List<Product>();
            string categoryNameSearch = "";

            try
            {
                connection.Open();

                // Fetch category name
                using (MySqlCommand cmdCategoryName = new MySqlCommand(queryCategoryName, connection))
                {
                    cmdCategoryName.Parameters.AddWithValue("@categoryId", categoryId);
                    object result = cmdCategoryName.ExecuteScalar();
                    if (result != null)
                    {
                        categoryNameSearch = result.ToString();
                    }
                    else
                    {
                        Console.WriteLine("Category not found.");
                        return 1; // No category found with the given ID
                    }
                }

                // Fetch products
                using (MySqlCommand cmdProducts = new MySqlCommand(queryProducts, connection))
                {
                    cmdProducts.Parameters.AddWithValue("@categoryId", categoryId);
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
                                categoryName = categoryNameSearch
                            };
                            products.Add(product);
                        }
                    }
                }

                // Display products
                if (products.Count > 0)
                {
                    Console.WriteLine($"Category: {categoryNameSearch}");
                    DisplayProductsByPage(products,connection);
                    return 2;
                }
                else
                {
                    Console.WriteLine("No products found for the given category.");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving data: {ex.Message}");
                return -1; // Indicating an error occurred
            }
            finally
            {
                connection.Close();
            }
        }
        public int SearchProductByName(string name, MySqlConnection connection)
        {
            string queryProducts = @"
    SELECT Product_ID, Product_Name, Price, Description, Quantity, Brand, Category_ID
    FROM product 
    WHERE Product_Name LIKE @name";

            string queryCategoryName = "SELECT Category_Name FROM category WHERE Category_ID=@categoryId";

            List<Product> products = new List<Product>();

            try
            {
                connection.Open();

                // Fetch products
                List<(int productId, string productName, decimal price, string description, int quantity, string brand, int categoryId)> tempProducts = new List<(int, string, decimal, string, int, string, int)>();

                using (MySqlCommand cmdProducts = new MySqlCommand(queryProducts, connection))
                {
                    cmdProducts.Parameters.AddWithValue("@name", "%" + name + "%");

                    using (MySqlDataReader reader = cmdProducts.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tempProducts.Add(
                                (
                                    reader.GetInt32("Product_ID"),
                                    reader.GetString("Product_Name"),
                                    reader.GetDecimal("Price"),
                                    reader.GetString("Description"),
                                    reader.GetInt32("Quantity"),
                                    reader.GetString("Brand"),
                                    reader.GetInt32("Category_ID")
                                )
                            );
                        }
                    }
                }

                // Fetch category names and create products
                foreach (var tempProduct in tempProducts)
                {
                    string categoryName = "Unknown Category";

                    using (MySqlCommand cmdCategoryName = new MySqlCommand(queryCategoryName, connection))
                    {
                        cmdCategoryName.Parameters.AddWithValue("@categoryId", tempProduct.categoryId);
                        object result = cmdCategoryName.ExecuteScalar();
                        if (result != null)
                        {
                            categoryName = result.ToString();
                        }
                    }

                    Product product = new Product
                    {
                        productId = tempProduct.productId,
                        productName = tempProduct.productName,
                        descriptionProduct = tempProduct.description,
                        price = tempProduct.price,
                        quantity = tempProduct.quantity,
                        brand = tempProduct.brand,
                        categoryName = categoryName
                    };

                    products.Add(product);
                }

                // Display products
                if (products.Count > 0)
                {
                    Console.WriteLine($"Search results for products containing '{name}':");
                    DisplayProductsByPage(products,connection);
                    return 2;
                }
                else
                {
                    Console.WriteLine("No products found with the given name.");
                    return 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return -1; // Optionally handle or log the exception
            }
            finally
            {
                connection.Close();
            }
        }

        public override void Update(MySqlConnection connection, int id)
        {
            List<string> update = new List<string>();
            Console.WriteLine("Update Product (or press Enter to skip)");
            Console.Write("Enter new product name: ");
            string newProductName = Console.ReadLine();
            Console.Write("Enter new product description: ");
            string newDescriptionProduct = Console.ReadLine();                
            decimal newPrice;
            bool isPriceValid;
            do
            {
                Console.Write("Enter new product price: ");
                string newPriceInput = Console.ReadLine();
                isPriceValid = decimal.TryParse(newPriceInput, out newPrice);
                if (newPrice > 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("The price must be greater than 0. Please try again.");
                }
            } while (true);
            bool isQuantityValid;
            int newQuantity;
            do
            {
                Console.Write("Enter new product quantity: ");
                string newQuantityInput = Console.ReadLine();              
                isQuantityValid = int.TryParse(newQuantityInput, out newQuantity);
                if (newQuantity > 0)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("The quantity must be greater than 0. Please try again.");
                }
            } while (true);
            Console.Write("Enter new product brand: ");
            string newBrand = Console.ReadLine();
            int newCategoryId = 0;

            do
            {
                Console.Write("Enter new category ID: ");
                string check = Console.ReadLine();
                if (string.IsNullOrEmpty(check)) 
                {
                    newCategoryId = 0;
                    break;
                }
                if (!int.TryParse(check, out newCategoryId))
                {
                    Console.WriteLine("Id Invalid. Please enter a valid number ");
                }
                else
                {
                    if (validation.CheckCategoryExist(newCategoryId, connection))
                    {
                        break;
                    }
                }
            } while (true);
           
            if (!string.IsNullOrEmpty(newProductName))
            {
                update.Add("Product_name = @name");
            }
            if (!string.IsNullOrEmpty(newDescriptionProduct))
            {
                update.Add("Description = @description");
            }
            if (isPriceValid)
            {
                update.Add("Price = @price");
            }
            if (isQuantityValid)
            {
                update.Add("Quantity = @quantity");
            }
            if (!string.IsNullOrEmpty(newBrand))
            {
                update.Add("Brand = @brand");
            }
            if (newCategoryId!=0)
            {
                update.Add("Category_ID = @categoryId");
            }
            if (update.Count > 0)
            {
                string query = "UPDATE product SET " + string.Join(", ", update) + " WHERE Product_ID = @id";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    if (!string.IsNullOrEmpty(newProductName))
                    {
                        cmd.Parameters.AddWithValue("@name", newProductName);
                    }
                    if (!string.IsNullOrEmpty(newDescriptionProduct))
                    {
                        cmd.Parameters.AddWithValue("@description", newDescriptionProduct);
                    }
                    if (isPriceValid)
                    {
                        cmd.Parameters.AddWithValue("@price", newPrice);
                    }
                    if (isQuantityValid)
                    {
                        cmd.Parameters.AddWithValue("@quantity", newQuantity);
                    }
                    if (!string.IsNullOrEmpty(newBrand))
                    {
                        cmd.Parameters.AddWithValue("@brand", newBrand);
                    }
                    if (newCategoryId!=0)
                    {
                        cmd.Parameters.AddWithValue("@categoryId", newCategoryId);
                    }

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        Console.WriteLine("Product updated successfully!");
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
                            Console.WriteLine("+----------------------------------------------+");
                            Console.WriteLine($"ID: {reader["Product_ID"]}");
                            Console.WriteLine($"Name: {reader["Product_Name"]}");
                            Console.WriteLine($"Price: {reader["Price"]}");
                            Console.WriteLine($"Description: {reader["Description"]}");
                            Console.WriteLine($"Quantity: {reader["Quantity"]}");
                            Console.WriteLine($"Brand: {reader["Brand"]}");
                            Console.WriteLine($"Category: {reader["Category_Name"]}");
                            Console.WriteLine("+----------------------------------------------+");
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
                Console.WriteLine("List product is empty");
            }
            else
            {
                //chinh sua phan trang muon test
                int pageSize = 5;
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
                Console.WriteLine("+-----+--------------------+--------+----------+--------------------+------------------+");
                Console.WriteLine("| ID  | Name               | Price  | Quantity | Brand              | Category Name    |");
                Console.WriteLine("+-----+--------------------+--------+----------+--------------------+------------------+");
                for (int i = start; i < end; i++)
                {
                    Product product = products[i];
                    Console.WriteLine($"| {product.productId,-3} | {product.productName,-18} | {product.price,6:F2} | {product.quantity,8} | {product.brand,-18} | {product.categoryName,-16} |");
                }
                Console.WriteLine("+-----+--------------------+--------+----------+--------------------+------------------+");
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
                        Console.WriteLine("+----+--------------------+");
                        Console.WriteLine("| ID | Category Name      |");
                        Console.WriteLine("+----+--------------------+");
                        while (reader.Read())
                        {
                            int categoryId = reader.GetInt32("Category_ID");
                            string categoryName = reader.GetString("Category_Name");

                            // Display category directly
                            Console.WriteLine($"| {categoryId,-3} | {categoryName,-18} |");
                        }
                        Console.WriteLine("+----+--------------------+");
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

        public void CreateCategory(MySqlConnection connection,int categoryId)
        {        
            // Prepare your SQL command to insert a new category
            string sql = "INSERT INTO Category (Category_Id,Category_Name) VALUES (@categoryId,@categoryName)";
            // Example category name, replace with actual data
            string categoryNameAdd;
            do
            {
                Console.Write("Enter category name: ");
                categoryNameAdd = Console.ReadLine();
                if (!validation.CheckCategoryNameExist(categoryNameAdd, connection))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Category name does not exist");
                }
            } while (true);
            // Create a command object
            connection.Open();
            MySqlCommand command = new MySqlCommand(sql, connection);
            // Add parameters to your command
            command.Parameters.AddWithValue("@categoryId", categoryId);
            command.Parameters.AddWithValue("@categoryName", categoryNameAdd);
            // Execute the command
            command.ExecuteNonQuery();
            // Close the connection
            connection.Close();
        }
    }
}
