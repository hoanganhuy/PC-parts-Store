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
        public bool verifyed {  get; set; }
        public bool accepted {  get; set; }

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
                        string queryGetCartId = "SELECT cartId FROM cart WHERE customer_id = @customerId";
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

                        // Cập nhật thông tin khách hàng nếu cần
                        Console.Write("Do you want to update your personal information? (1.yes/2.no): ");
                        int updateInfo = int.Parse(Console.ReadLine());
                        if (updateInfo == 1)
                        {
                            Account account = new Account();
                            account.UpdateInformationCustomer(customerId, connection);
                        }

                        // Truy xuất thông tin khách hàng
                        string queryCustomer = "SELECT name, email, address, phone FROM customers WHERE customerId = @customerId;";
                        string customerName = "", customerEmail = "", customerAddress = "", customerPhoneNumber = "";
                        using (MySqlCommand cmdCustomer = new MySqlCommand(queryCustomer, connection, transaction))
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
                        using (MySqlCommand cmdCartItems = new MySqlCommand(queryCartItems, connection, transaction))
                        {
                            cmdCartItems.Parameters.AddWithValue("@cartId", cartId);
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

                        // đồng ý thanh toán 
                        Console.Write("Do you want to proceed with the payment? (1.yes/2.no): ");
                        int confirmation = int.Parse(Console.ReadLine());
                        if (confirmation != 1)
                        {
                            Console.WriteLine("Payment cancelled.");
                            return;
                        }

                        // Create Order
                        string queryCreateOrder = "INSERT INTO orders (customerId, customerName, customerPhoneNumber, customerEmail, customerAddress, verified, accepted) VALUES (@customerId, @customerName, @customerPhoneNumber, @customerEmail, @customerAddress, false, false);";
                        using (MySqlCommand cmdCreateOrder = new MySqlCommand(queryCreateOrder, connection, transaction))
                        {
                            cmdCreateOrder.Parameters.AddWithValue("@customerId", customerId);
                            cmdCreateOrder.Parameters.AddWithValue("@customerName", customerName);
                            cmdCreateOrder.Parameters.AddWithValue("@customerPhoneNumber", customerPhoneNumber);
                            cmdCreateOrder.Parameters.AddWithValue("@customerEmail", customerEmail);
                            cmdCreateOrder.Parameters.AddWithValue("@customerAddress", customerAddress);
                            cmdCreateOrder.ExecuteNonQuery();
                        }
                        // Update Product Quantities
                        using (MySqlCommand cmdGetCartItems = new MySqlCommand(queryCartItems, connection, transaction))
                        {
                            cmdGetCartItems.Parameters.AddWithValue("@cartId", cartId);
                            using (MySqlDataReader reader = cmdGetCartItems.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int productId = reader.GetInt32("productId");
                                    int amount = reader.GetInt32("amount");
                                    string queryUpdateQuantity = "UPDATE product SET quantity = quantity - @amount WHERE productId = @productId;";
                                    using (MySqlCommand cmdUpdateQuantity = new MySqlCommand(queryUpdateQuantity, connection, transaction))
                                    {
                                        cmdUpdateQuantity.Parameters.AddWithValue("@amount", amount);
                                        cmdUpdateQuantity.Parameters.AddWithValue("@productId", productId);
                                        cmdUpdateQuantity.ExecuteNonQuery();
                                    }
                                }
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
            throw new NotImplementedException();
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
                    using(MySqlCommand cmdAccepted = new MySqlCommand( queryAccepted, connection))
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
