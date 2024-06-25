using MySql.Data.MySqlClient;
using PC_Part_Store.Interface;
using System.Data;
using System.Data.Common;
using System.Transactions;

namespace PC_Part_Store.Implement
{
    internal class Order : IOrder
    {

        public string customerName { get; set; }
        public string customerPhoneNumber { get; set; }
        public string customerEmail { get; set; }
        public string customerAddress { get; set; }
        public bool verifyed { get; set; }
        public bool accepted { get; set; }

        public void Pay(MySqlConnection connection, int customerId)
        {
            try
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Get Cart ID
                        string queryGetCartId = "SELECT cart_ID FROM cart WHERE Customer_ID = @customerId";
                        int cartId;
                        using (MySqlCommand cmdGetCartId = new MySqlCommand(queryGetCartId, connection, transaction))
                        {
                            cmdGetCartId.Parameters.AddWithValue("@customerId", customerId);
                            var result = cmdGetCartId.ExecuteScalar();
                            if (result != null)
                            {
                                cartId = Convert.ToInt32(result);
                            }
                            else
                            {
                                Console.WriteLine("No active cart found for the customer.");
                                return;
                            }
                        }

                        // Update Customer Information if needed
                        Console.Write("Do you want to update your personal information? (1.yes/2.no): ");
                        int updateInfo = int.Parse(Console.ReadLine());
                        if (updateInfo == 1)
                        {
                            Account account = new Account();
                            connection.Close();
                            account.UpdateInformationCustomer(customerId, connection);
                            connection.Open();
                        }

                        // Retrieve Customer Information
                        string queryCustomer = "SELECT Customer_Name, Email, Address, phone_number FROM customer WHERE customer_ID = @customerId;";
                        string customerName = "", customerEmail = "", customerAddress = "", customerPhoneNumber = "";
                        using (MySqlCommand cmdCustomer = new MySqlCommand(queryCustomer, connection, transaction))
                        {
                            cmdCustomer.Parameters.AddWithValue("@customerId", customerId);
                            using (MySqlDataReader reader = cmdCustomer.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    customerName = reader.GetString("Customer_name");
                                    customerEmail = reader.GetString("email");
                                    customerAddress = reader.GetString("address");
                                    customerPhoneNumber = reader.GetString("phone_number");
                                }
                            }
                        }

                        // Display Customer Information
                        Console.WriteLine("Customer Information:");
                        Console.WriteLine($"Name: {customerName}");
                        Console.WriteLine($"Email: {customerEmail}");
                        Console.WriteLine($"Address: {customerAddress}");
                        Console.WriteLine($"Phone: {customerPhoneNumber}");

                        // Retrieve and Display Order Information
                        Console.WriteLine("Order Information:");
                        Console.WriteLine("Product ID | Quantity | Unit Price | Total Price");
                        string queryCartItems = "SELECT cd.product_ID, cd.amount, p.price FROM cart_Detail cd JOIN product p ON cd.product_ID = p.product_ID WHERE cd.cart_ID = @cartId;";
                        List<(int productId, int amount, decimal price)> cartItems = new List<(int, int, decimal)>();
                        decimal totalOrderPrice = 0;
                        using (MySqlCommand cmdCartItems = new MySqlCommand(queryCartItems, connection, transaction))
                        {
                            cmdCartItems.Parameters.AddWithValue("@cartId", cartId);
                            using (MySqlDataReader reader = cmdCartItems.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int productId = reader.GetInt32("product_ID");
                                    int amount = reader.GetInt32("amount");
                                    decimal price = reader.GetDecimal("price");
                                    decimal totalPrice = amount * price;

                                    totalOrderPrice += totalPrice;
                                    Console.WriteLine($"{productId} | {amount} | {price:F2} | {totalPrice:F2}");

                                    // Add to cartItems list for later update
                                    cartItems.Add((productId, amount, price));
                                }
                            }
                        }

                        Console.WriteLine($"Total Order Price: {totalOrderPrice:F2}");

                        // Confirm Payment
                        Console.Write("Do you want to proceed with the payment? (1.yes/2.no): ");
                        int confirmation = int.Parse(Console.ReadLine());
                        if (confirmation != 1)
                        {
                            Console.WriteLine("Payment cancelled.");
                            return;
                        }

                        // Create Order
                        string queryCreateOrder = "INSERT INTO orders(Customer_ID,Customer_name, Customer_phone_number, Customer_email, Customer_Address,Total_Price,Verified, Accepted)" + "VALUES (@customerId, @customerName, @customerPhoneNumber, @customerEmail, @customerAddress,@totalPrice, false, false)";
                        using (MySqlCommand cmdCreateOrder = new MySqlCommand(queryCreateOrder, connection, transaction))
                        {
                            cmdCreateOrder.Parameters.AddWithValue("@customerId", customerId);
                            cmdCreateOrder.Parameters.AddWithValue("@customerName", customerName);
                            cmdCreateOrder.Parameters.AddWithValue("@customerPhoneNumber", customerPhoneNumber);
                            cmdCreateOrder.Parameters.AddWithValue("@customerEmail", customerEmail);
                            cmdCreateOrder.Parameters.AddWithValue("@customerAddress", customerAddress);
                            cmdCreateOrder.Parameters.AddWithValue("@totalPrice", totalOrderPrice);
                            cmdCreateOrder.ExecuteNonQuery();
                        }

                        // Update Product Quantities
                        foreach (var item in cartItems)
                        {
                            int productId = item.productId;
                            int amount = item.amount;
                            string queryUpdateQuantity = "UPDATE product SET Quantity = Quantity - @Amount WHERE Product_ID = @productId;";
                            using (MySqlCommand cmdUpdateQuantity = new MySqlCommand(queryUpdateQuantity, connection, transaction))
                            {
                                cmdUpdateQuantity.Parameters.AddWithValue("@amount", amount);
                                cmdUpdateQuantity.Parameters.AddWithValue("@productId", productId);
                                cmdUpdateQuantity.ExecuteNonQuery();
                            }
                        }

                        // Commit Transaction
                        transaction.Commit();
                        Console.WriteLine("Payment successful, order created.");
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("An error occurred during payment: " + ex.Message);
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

        //hien thi don hang cua khach hang
        public void ViewOrder(MySqlConnection connection, int customerId)
        {
            try
            {
                connection.Open();

                // Get cart ID for the customer
                string queryGetCartId = "SELECT cart_ID FROM cart WHERE Customer_ID = @customerId";
                int cartId = 0;
                using (MySqlCommand cmdGetCartId = new MySqlCommand(queryGetCartId, connection))
                {
                    cmdGetCartId.Parameters.AddWithValue("@customerId", customerId);
                    var result = cmdGetCartId.ExecuteScalar();
                    if (result != null)
                    {
                        cartId = Convert.ToInt32(result);
                    }
                    else
                    {
                        Console.WriteLine("No active cart found for the customer.");
                        return;
                    }
                }

                // Retrieve order details
                string queryOrder = @"
                SELECT Customer_name, Customer_phone_number, Customer_email, Customer_Address, Total_Price, Verified, Accepted
                FROM orders 
                WHERE Customer_ID = @customerId";

                using (MySqlCommand cmdOrder = new MySqlCommand(queryOrder, connection))
                {
                    cmdOrder.Parameters.AddWithValue("@customerId", customerId);

                    using (MySqlDataReader reader = cmdOrder.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                string customerName = reader.GetString("Customer_name");
                                string customerPhoneNumber = reader.GetString("Customer_phone_number");
                                string customerEmail = reader.GetString("Customer_email");
                                string customerAddress = reader.GetString("Customer_Address");
                                decimal totalPrice = reader.GetDecimal("Total_Price");
                                bool verified = reader.GetBoolean("Verified");
                                bool accepted = reader.GetBoolean("Accepted");                                

                                Console.WriteLine($"Name: {customerName}");
                                Console.WriteLine($"Phone Number: {customerPhoneNumber}");
                                Console.WriteLine($"Email: {customerEmail}");
                                Console.WriteLine($"Address: {customerAddress}");
                                
                                connection.Close();
                                ViewOrderDetails(customerId, connection);
                                Console.WriteLine($"Total Price: {totalPrice}");
                                Console.WriteLine($"Verified: {verified}");
                                Console.WriteLine($"Accepted: {accepted}");
                            }         
                        }
                        else
                        {
                            Console.WriteLine("No orders found for the given customer ID.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot connect to database: " + ex.Message);
            }
            finally
            {
                //connection.Close();
            }
        }
        public void ViewOrderDetails(int customerId, MySqlConnection connection)
        {
            try
            {
                connection.Open();

                // Kiểm tra xem khách hàng có giỏ hàng hay không
                string checkCartQuery = "SELECT Cart_ID FROM cart WHERE Customer_ID = @customerId";
                int cartId = 0;

                using (MySqlCommand checkCartCmd = new MySqlCommand(checkCartQuery, connection))
                {
                    checkCartCmd.Parameters.AddWithValue("@customerId", customerId);
                    object result = checkCartCmd.ExecuteScalar();

                    if (result != null)
                    {
                        cartId = Convert.ToInt32(result);
                    }
                    else
                    {
                        Console.WriteLine("Cart not found for the customer.");
                        return;
                    }
                }

                // Câu truy vấn để lấy thông tin chi tiết sản phẩm từ giỏ hàng, bao gồm giá tiền
                string query = "SELECT cd.Product_Id, p.Product_Name, p.Price, cd.Amount " +
                               "FROM cart_Detail cd " +
                               "JOIN product p ON cd.Product_Id = p.Product_ID " +
                               "WHERE cd.Cart_ID = @cartId";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@cartId", cartId);

                    // Sử dụng MySqlDataReader để đọc các dòng kết quả
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("Order Information:");
                        Console.WriteLine("------------------------------------------");
                        decimal totalCost = 0;
                        while (reader.Read())
                        {
                            int productId = reader.GetInt32("Product_Id");
                            string productName = reader.GetString("Product_Name");
                            decimal price = reader.GetDecimal("Price");
                            int amount = reader.GetInt32("Amount");
                            decimal cost = price * amount;

                            Console.WriteLine($"Product ID: {productId}, Name: {productName}, Price: {price:F2}, Amount: {amount}, Cost: {cost:F2}");
                            totalCost += cost;
                        }
                        Console.WriteLine("------------------------------------------");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }
        public void Accepted(MySqlConnection connection)
        {
            try
            {
                connection.Open();
                //hien thi tat ca don hang
                string queryGetOrder = "SELECT orderId,customerName,verifyed,accepted FROM order ";
                Console.WriteLine("Orders List:");
                Console.WriteLine("Order ID | Customer Name | Verifyed | Accepted");
                using (MySqlCommand cmdGetOrder = new MySqlCommand(queryGetOrder, connection))
                {
                    using (MySqlDataReader reader = cmdGetOrder.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int orderId = reader.GetInt32("orderId");

                            string customerNameOrder = reader.GetString("customerName");
                            string verifyed = reader.GetString("verifyed");
                            string accepted = reader.GetString("accepted");
                            Console.WriteLine($"{orderId} | {customerNameOrder} | {verifyed} | {accepted}");
                        }
                    }
                }
                Console.WriteLine("Enter the Order ID to view details:");
                int selectedOrderId = int.Parse(Console.ReadLine());
                //get Id customer
                string queryGetCustomerId = "SELECT customerId FROM order WHERE orderId=@orderId";
                int customerId;
                using (MySqlCommand cmdGetCustomerId = new MySqlCommand(queryGetCustomerId, connection))
                {
                    cmdGetCustomerId.Parameters.AddWithValue("@orderId", selectedOrderId);
                    var resultCustomer = cmdGetCustomerId.ExecuteScalar();
                    if (resultCustomer != null)
                    {
                        customerId = Convert.ToInt32(resultCustomer);
                    }
                    else
                    {
                        Console.WriteLine("OrderId is does not exist");
                        return;
                    }
                }
                // Get Cart ID
                string queryGetCartId = "SELECT cartId FROM cart WHERE customer_id = @customerId";
                int cartIdSelect;
                using (MySqlCommand cmdGetCartId = new MySqlCommand(queryGetCartId, connection))
                {
                    cmdGetCartId.Parameters.AddWithValue("@customerId", customerId);
                    var result = cmdGetCartId.ExecuteScalar();
                    if (result != null)
                    {
                        cartIdSelect = Convert.ToInt32(result);
                    }
                    else
                    {
                        Console.WriteLine("No active cart found for the customer.");
                        return;
                    }
                }
                string queryCustomer = "SELECT name, email, address, phone FROM customers WHERE customerId = @customerId;";
                string customerName = "", customerEmail = "", customerAddress = "", customerPhoneNumber = "";
                using (MySqlCommand cmdCustomer = new MySqlCommand(queryCustomer, connection))
                {
                    cmdCustomer.Parameters.AddWithValue("@customerId", customerId);
                    using (MySqlDataReader reader = cmdCustomer.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            customerName = reader.GetString("name");
                            customerEmail = reader.GetString("email");
                            customerAddress = reader.GetString("address");
                            customerPhoneNumber = reader.GetString("phone");
                        }
                    }
                }

                // Hiển thị thông tin khách hàng
                Console.WriteLine("Customer Information:");
                Console.WriteLine($"Name: {customerName}");
                Console.WriteLine($"Email: {customerEmail}");
                Console.WriteLine($"Address: {customerAddress}");
                Console.WriteLine($"Phone: {customerPhoneNumber}");

                // Retrieve and Display Order Information
                Console.WriteLine("Order Information:");
                Console.WriteLine("Product ID | Quantity | Unit Price | Total Price");
                string queryCartItems = "SELECT cd.productId, cd.amount, p.price FROM cartDetails cd JOIN product p ON cd.productId = p.productId WHERE cd.cartId = @cartId;";
                decimal totalOrderPrice = 0;
                using (MySqlCommand cmdCartItems = new MySqlCommand(queryCartItems, connection))
                {
                    cmdCartItems.Parameters.AddWithValue("@cartId", cartIdSelect);
                    using (MySqlDataReader reader = cmdCartItems.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int productId = reader.GetInt32("productId");
                            int amount = reader.GetInt32("amount");
                            decimal price = reader.GetDecimal("price");
                            decimal totalPrice = amount * price;

                            totalOrderPrice += totalPrice;
                            Console.WriteLine($"{productId} | {amount} | {price:C} | {totalPrice:C}");
                        }
                    }
                }

                Console.WriteLine($"Total Order Price: {totalOrderPrice:C}");
                Console.Write("Do you want to confirm this order? (1.yes/2.no): ");
                int confirmation = int.Parse(Console.ReadLine());
                if (confirmation != 2)
                {
                    string queryAccepted = "UPDATE order SET accepted = true WHERE orderId=@orderId";
                    using (MySqlCommand cmdAccepted = new MySqlCommand(queryAccepted, connection))
                    {
                        cmdAccepted.Parameters.AddWithValue("@orderId", selectedOrderId);
                        cmdAccepted.ExecuteNonQuery();
                        Console.WriteLine("Order confirmed successfully.");
                    }
                }
                else
                {
                    using (MySqlCommand cmdGetCartItems = new MySqlCommand(queryCartItems, connection))
                    {
                        cmdGetCartItems.Parameters.AddWithValue("@cartId", cartIdSelect);
                        using (MySqlDataReader reader = cmdGetCartItems.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int productId = reader.GetInt32("productId");
                                int amount = reader.GetInt32("amount");
                                string queryUpdateQuantity = "UPDATE product SET quantity = quantity + @amount WHERE productId = @productId;";
                                using (MySqlCommand cmdUpdateQuantity = new MySqlCommand(queryUpdateQuantity, connection))
                                {
                                    cmdUpdateQuantity.Parameters.AddWithValue("@amount", amount);
                                    cmdUpdateQuantity.Parameters.AddWithValue("@productId", productId);
                                    cmdUpdateQuantity.ExecuteNonQuery();
                                }
                            }
                        }
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
    }
}
