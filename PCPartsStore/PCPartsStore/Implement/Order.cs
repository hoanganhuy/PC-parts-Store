using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto;
using PC_Part_Store.Interface;
using System.Data;
using System.Data.Common;
using System.Globalization;
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
                        string queryCartItems = "SELECT cd.product_ID, cd.amount,p.product_name, p.price FROM cart_Detail cd JOIN product p ON cd.product_ID = p.product_ID WHERE cd.cart_ID = @cartId;";
                        List<(int productId,string productName, int amount, decimal price)> cartItems = new List<(int,string, int, decimal)>();
                        decimal totalOrderPrice = 0;
                        using (MySqlCommand cmdCartItems = new MySqlCommand(queryCartItems, connection, transaction))
                        {
                            cmdCartItems.Parameters.AddWithValue("@cartId", cartId);
                            using (MySqlDataReader reader = cmdCartItems.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int productId = reader.GetInt32("product_ID");
                                    string productName = reader.GetString("product_name");
                                    int amount = reader.GetInt32("amount");
                                    decimal price = reader.GetDecimal("price");
                                    decimal totalPrice = amount * price;

                                    totalOrderPrice += totalPrice;
                                    Console.WriteLine($"{productId} | {productName} | {amount} | {price:F2} | {totalPrice:F2}");

                                    // Add to cartItems list for later update
                                    cartItems.Add((productId,productName, amount, price));
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
                        string queryCreateOrder = "INSERT INTO orders(Customer_ID,Customer_name, Customer_phone_number, Customer_email, Customer_Address,Total_Price,Verified, Accepted,rejected)" + "VALUES (@customerId, @customerName, @customerPhoneNumber, @customerEmail, @customerAddress, @totalPrice, false, false, false)";
                        int orderId;       
                        using (MySqlCommand cmdCreateOrder = new MySqlCommand(queryCreateOrder, connection, transaction))
                        {
                            cmdCreateOrder.Parameters.AddWithValue("@customerId", customerId);
                            cmdCreateOrder.Parameters.AddWithValue("@customerName", customerName);
                            cmdCreateOrder.Parameters.AddWithValue("@customerPhoneNumber", customerPhoneNumber);
                            cmdCreateOrder.Parameters.AddWithValue("@customerEmail", customerEmail);
                            cmdCreateOrder.Parameters.AddWithValue("@customerAddress", customerAddress);
                            cmdCreateOrder.Parameters.AddWithValue("@totalPrice", totalOrderPrice);
                            cmdCreateOrder.ExecuteNonQuery();
                            orderId = (int)cmdCreateOrder.LastInsertedId;
                        }

                        // create order details
                        string queryCreateOrderDetail = "INSERT INTO order_detail (order_id, product_id,product_name, amount, price) VALUES (@orderId, @productId,@productName,@amount, @price)";
                        foreach (var item in cartItems)
                        {
                            using (MySqlCommand cmdCreateOrderDetail = new MySqlCommand(queryCreateOrderDetail, connection, transaction))
                            {
                                cmdCreateOrderDetail.Parameters.AddWithValue("@orderId", orderId);
                                cmdCreateOrderDetail.Parameters.AddWithValue("@productId", item.productId);
                                cmdCreateOrderDetail.Parameters.AddWithValue("@productName", item.productName);
                                cmdCreateOrderDetail.Parameters.AddWithValue("@amount", item.amount);
                                cmdCreateOrderDetail.Parameters.AddWithValue("@price", item.price);
                                cmdCreateOrderDetail.ExecuteNonQuery();
                            }
                        }
                        // delete cart detail
                        string queryDeleteCartItems = "DELETE FROM cart_detail WHERE cart_id = @cartId";
                        using (MySqlCommand cmdDeleteCartItems = new MySqlCommand(queryDeleteCartItems, connection, transaction))
                        {
                            cmdDeleteCartItems.Parameters.AddWithValue("@cartId", cartId);
                            cmdDeleteCartItems.ExecuteNonQuery();
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
        public int ViewOrder(MySqlConnection connection, int customerId)
        {
            try
            {
                connection.Open();
                string query = "SELECT order_id,total_price,created_at,accepted,rejected From orders where customer_id=@customerId";
                using(MySqlCommand cmd=new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@customerId", customerId);
                    using(MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("Order Id | order date | total price | acceptes | rejected");
                        bool hasOrders = false;
                        while (reader.Read())
                        {
                            hasOrders= true;
                            int orderId = reader.GetInt32("order_id");
                            DateTime orderDate = reader.GetDateTime("created_at");
                            decimal totalPrice = reader.GetDecimal("total_price");
                            bool accepted = reader.GetBoolean("accepted");
                            bool rejected = reader.GetBoolean("rejected");
                            Console.WriteLine($"{orderId} | {orderDate} | {totalPrice} | {accepted} | {rejected}");
                        }
                        if (!hasOrders)
                        {
                            Console.WriteLine("No orders found for this customer.");
                            return 0;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving order information: " + ex.Message);
                throw;
            } finally
            {
                connection.Close();
            }
        }
        public void ViewOrderDetails(int orderId, MySqlConnection connection)
        {
            try
            {
                connection.Open();
                string queryOrders = "SELECT total_price, created_at, Accepted,customer_name,customer_address,customer_phone_number,customer_email,rejected FROM orders WHERE order_id = @orderId";
                using (MySqlCommand cmdOrders = new MySqlCommand(queryOrders, connection))
                {
                    cmdOrders.Parameters.AddWithValue("@orderId", orderId);
                    using (MySqlDataReader readerOrders = cmdOrders.ExecuteReader())
                    {
                        if (readerOrders.Read())
                        {
                            DateTime orderDate = readerOrders.GetDateTime("created_at");
                            decimal totalPrice = readerOrders.GetDecimal("total_price");
                            bool accepted = readerOrders.GetBoolean("Accepted");
                            customerName =readerOrders.GetString("customer_name");
                            customerAddress = readerOrders.GetString("customer_address");
                            customerPhoneNumber = readerOrders.GetString("customer_phone_number");
                            customerEmail = readerOrders.GetString("customer_email");
                            bool rejected = readerOrders.GetBoolean("rejected");

                            Console.WriteLine($"Order ID: {orderId}");
                            Console.WriteLine($"Order Date: {orderDate}");
                            Console.WriteLine($"Customer Name: {customerName}");
                            Console.WriteLine($"Customer Address: {customerAddress}");
                            Console.WriteLine($"Customer Phone Number: {customerPhoneNumber}");
                            Console.WriteLine($"Cusromer Email:{customerEmail}");
                            Console.WriteLine($"Total Price: {totalPrice}");
                            Console.WriteLine($"Accepted: {accepted}");
                            Console.WriteLine($"Rejected: {rejected}");                                                
                        }
                    }
                    Console.WriteLine("Order Details:");
                    Console.WriteLine("Product ID | Product Name | Quantity | Unit Price | Total Price");
                    // Truy vấn chi tiết đơn hàng
                    string queryOrderDetails = "SELECT product_id,product_name,amount,price from order_detail where order_id = @orderId";
                    using (MySqlCommand cmdOrderDetails = new MySqlCommand(queryOrderDetails, connection))
                    {
                        cmdOrderDetails.Parameters.AddWithValue("@orderId", orderId);

                        using (MySqlDataReader readerOrderDetails = cmdOrderDetails.ExecuteReader())
                        {
                            while (readerOrderDetails.Read())
                            {
                                int productId = readerOrderDetails.GetInt32("product_id");
                                string productName = readerOrderDetails.GetString("product_name");
                                int quantity = readerOrderDetails.GetInt32("amount");
                                decimal unitPrice = readerOrderDetails.GetDecimal("price");
                                decimal totalPriceDetail = quantity * unitPrice;

                                Console.WriteLine($"{productId} | {productName} | {quantity} | {unitPrice:F2} | {totalPriceDetail:F2}");
                            }
                            //readerOrderDetails.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving order information: " + ex.Message);
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
                string queryGetOrder = "SELECT Order_ID,Customer_name,total_price,Verified,Accepted,Rejected FROM orders ";
                Console.WriteLine("Orders List:");
                Console.WriteLine("Order ID | Customer Name | Total Price | Verified | Accepted");
                using (MySqlCommand cmdGetOrder = new MySqlCommand(queryGetOrder, connection))
                {
                    using (MySqlDataReader reader = cmdGetOrder.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int orderId = reader.GetInt32("Order_Id");
                            string customerNameOrder = reader.GetString("customer_Name");
                            decimal totalPrice = reader.GetDecimal("total_price");
                            bool verified = reader.GetBoolean("Verified");
                            bool accepted = reader.GetBoolean("Accepted");
                            bool rejected = reader.GetBoolean("Rejected");
                            Console.WriteLine($"{orderId} | {customerNameOrder} | {totalPrice} | {verified} | {accepted} | {rejected}");

                        }
                    }
                }
                Console.Write("Enter the Order ID to view details: ");
                int selectedOrderId = int.Parse(Console.ReadLine());
                //danh dau don hang da xem
                string queryVerified = "UPDATE orders set verified=true where order_Id=@orderID";
                using (MySqlCommand cmdVerified = new MySqlCommand(queryVerified, connection))
                {
                    cmdVerified.Parameters.AddWithValue("@orderId", selectedOrderId);
                    cmdVerified.ExecuteNonQuery();
                }
                //hien thi thong tin khach hang và đơn hàng 
                connection.Close();
                ViewOrderDetails(selectedOrderId, connection);
                connection.Open();
                //kiem tra don hang da thanh toan chua
                string queryCheckStatus = "SELECT accepted, rejected FROM orders WHERE order_ID = @orderId";
                bool checkAccepted = false;
                bool checkRejected = false;
                using (MySqlCommand cmdCheckStatus = new MySqlCommand(queryCheckStatus, connection))
                {
                    cmdCheckStatus.Parameters.AddWithValue("@orderId", selectedOrderId);
                    using (MySqlDataReader reader = cmdCheckStatus.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            checkAccepted = reader.GetBoolean("accepted");
                            checkRejected = reader.GetBoolean("rejected");
                        }
                    }
                }

                if (checkAccepted)
                {
                    Console.WriteLine("Order has been paid.");
                    return;
                }
                else if (checkRejected)
                {
                    Console.WriteLine("Order has been rejected.");
                    return;
                }
                //lua chon co thanh toan don hang hay khong
                Console.Write("Do you want to accept this order as paid? (1.yes/2.no): ");
                int confirmation = int.Parse(Console.ReadLine());
                if (confirmation == 1)
                {
                    string queryAcceptOrder = "UPDATE orders SET accepted = true WHERE order_Id = @orderId";
                    using (MySqlCommand cmdAcceptOrder = new MySqlCommand(queryAcceptOrder, connection))
                    {
                        cmdAcceptOrder.Parameters.AddWithValue("@orderId", selectedOrderId);
                        cmdAcceptOrder.ExecuteNonQuery();
                        Console.WriteLine("Order has been marked as paid.");
                    }
                }
                else
                {
                    Console.WriteLine("Order payment has not been accepted.");
                    // Đánh dấu đơn hàng là bị từ chối
                    string queryRejectOrder = "UPDATE orders SET rejected = true WHERE order_Id = @orderId";
                    using (MySqlCommand cmdRejectOrder = new MySqlCommand(queryRejectOrder, connection))
                    {
                        cmdRejectOrder.Parameters.AddWithValue("@orderId", selectedOrderId);
                        cmdRejectOrder.ExecuteNonQuery();
                    }
                    // Hoàn lại số lượng sản phẩm vào kho
                    string queryOrderDetails = "SELECT product_id, amount FROM order_detail WHERE order_id = @orderId";
                    List<(int productId, int amount)> orderItems = new List<(int, int)>();
                    using (MySqlCommand cmdOrderDetails = new MySqlCommand(queryOrderDetails, connection))
                    {
                        cmdOrderDetails.Parameters.AddWithValue("@orderId", selectedOrderId);
                        using (MySqlDataReader readerOrderDetails = cmdOrderDetails.ExecuteReader())
                        {
                            while (readerOrderDetails.Read())
                            {
                                int productId = readerOrderDetails.GetInt32("product_id");
                                int amount = readerOrderDetails.GetInt32("amount");
                                orderItems.Add((productId, amount));
                            }
                        }
                    }
                    foreach (var item in orderItems)
                    {
                        string queryRestoreQuantity = "UPDATE product SET quantity = quantity + @amount WHERE product_id = @productId";
                        using (MySqlCommand cmdRestoreQuantity = new MySqlCommand(queryRestoreQuantity, connection))
                        {
                            cmdRestoreQuantity.Parameters.AddWithValue("@amount", item.amount);
                            cmdRestoreQuantity.Parameters.AddWithValue("@productId", item.productId);
                            cmdRestoreQuantity.ExecuteNonQuery();
                        }
                    }

                    Console.WriteLine("Product quantities have been restored.");
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
