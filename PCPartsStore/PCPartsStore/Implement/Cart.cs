﻿using MySql.Data.MySqlClient;
using PC_Part_Store.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace PC_Part_Store.Implement
{
    public class Cart : Super<Cart>, ICart
    {
        public int orderId { get; set; }
        public int quantity { get; set; }
        public int idCart { get; set; }
        public int idCustomer { get; set; }
        public override void Add(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public override void Remove(MySqlConnection connection, int id)
        {
            throw new NotImplementedException();
        }

        public override void Update(MySqlConnection connection, int id)
        {
            throw new NotImplementedException();
        }
        public void UpdateProductToCart(int customerId, MySqlConnection connection)
        {
            try
            {
                connection.Open();
                using (MySqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string getCartIdQuery = "SELECT Cart_ID FROM cart WHERE Customer_Id=@customerId";
                        int cartId;
                        using (MySqlCommand getCartIdCmd = new MySqlCommand(getCartIdQuery, connection, transaction))
                        {
                            getCartIdCmd.Parameters.AddWithValue("@customerId", customerId);
                            var result = getCartIdCmd.ExecuteScalar();
                            if (result != null)
                            {
                                cartId = Convert.ToInt32(result);
                            }
                            else
                            {
                                Console.WriteLine("You have not created a shopping cart yet");
                                return;
                            }
                        }
                        //ViewCart(Program.customerIdCurrent, connection);
                        Console.Write("Enter the product id you want to update: ");
                        int productId = int.Parse(Console.ReadLine());
                        //lay so luong hang hien tai trong cart details
                        string queryCheckProductInCart = "SELECT Amount FROM cart_Detail WHERE Cart_ID = @cartId AND Product_ID = @productId;";
                        int currentQuantity;
                        using (MySqlCommand cmdCheckProductInCart = new MySqlCommand(queryCheckProductInCart, connection, transaction))
                        {
                            cmdCheckProductInCart.Parameters.AddWithValue("@cartId", cartId);
                            cmdCheckProductInCart.Parameters.AddWithValue("@productId", productId);
                            var productResult = cmdCheckProductInCart.ExecuteScalar();
                            if (productResult != null)
                            {
                                currentQuantity = Convert.ToInt32(productResult);
                            }
                            else
                            {
                                Console.WriteLine("Product does not exist in the cart.");
                                return;
                            }
                        }
                        Console.WriteLine("1.Update amount");
                        Console.WriteLine("2.Remove product to cart");
                        Console.WriteLine("3.Cancel update");
                        Console.Write("Choose an option: ");
                        int option = int.Parse(Console.ReadLine());
                        switch (option)
                        {
                            case 1:
                                {
                                    Console.Write("Enter the new amount: ");
                                    int newAmount = int.Parse(Console.ReadLine());
                                    //kiem tra so luong san pham hien tai trong kho
                                    string queryCheckAmountProduct = "SELECT Quantity FROM product WHERE Product_ID = @productId";
                                    using (MySqlCommand cmdCheckAmountProduct = new MySqlCommand(queryCheckAmountProduct, connection, transaction))
                                    {
                                        cmdCheckAmountProduct.Parameters.AddWithValue("@productId", productId);
                                        var quantityResult = cmdCheckAmountProduct.ExecuteScalar();
                                        if (quantityResult != null)
                                        {
                                            currentQuantity = Convert.ToInt32(quantityResult);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Product does not exist.");
                                            return;
                                        }
                                    }
                                    int quantityChange = newAmount - currentQuantity;
                                    //kiem tra so luong san pham moi co lon hon so luong san pham trong kho khong
                                    if (quantityChange > 0 && currentQuantity < quantityChange)
                                    {
                                        throw new Exception("Not enough stock available.");
                                    }
                                    string queryUpdateAmount = "UPDATE cart_Detail SET Amount = @newAmount WHERE Cart_ID = @cartId AND Product_ID = @productId";
                                    using (MySqlCommand cmdUpdateAmount = new MySqlCommand(queryUpdateAmount, connection, transaction))
                                    {
                                        cmdUpdateAmount.Parameters.AddWithValue("@newAmount", newAmount);
                                        cmdUpdateAmount.Parameters.AddWithValue("@cartId", cartId);
                                        cmdUpdateAmount.Parameters.AddWithValue("@productId", productId);
                                        cmdUpdateAmount.ExecuteNonQuery();
                                    }
                                    /*cap nhat so luong thuc te cua san pham trong kho
                                    string updateProductStockQuery = "UPDATE products SET stock_quantity = stock_quantity - @quantityChange WHERE product_id = @productId;";
                                    using (MySqlCommand updateProductStockCmd = new MySqlCommand(updateProductStockQuery, connection, transaction))
                                    {
                                        updateProductStockCmd.Parameters.AddWithValue("@quantityChange", quantityChange);
                                        updateProductStockCmd.Parameters.AddWithValue("@productId", productId);
                                        updateProductStockCmd.ExecuteNonQuery();
                                    }*/
                                    Console.WriteLine("Product quantity updated successfully.");
                                    break;
                                }
                            case 2:
                                {
                                    //xoa san pham trong gio hang
                                    string queryRemoveProductInCart = "DELETE FROM Cart_Detail WHERE cart_ID = @cartId AND Product_ID = @productId";
                                    using (MySqlCommand cmdReomveProductInCart = new MySqlCommand(queryRemoveProductInCart, connection, transaction))
                                    {
                                        cmdReomveProductInCart.Parameters.AddWithValue("@cartId", cartId);
                                        cmdReomveProductInCart.Parameters.AddWithValue("@productId", productId);
                                        cmdReomveProductInCart.ExecuteNonQuery();
                                    }
                                    /*cap nhat so luong trong kho
                                    string queryReStoreQuantity = "UPDATE product SET quantity=quatity-@currentQuantity WHERE product_id = @productId";
                                    using (MySqlCommand cmdReStoreQuantity = new MySqlCommand(queryReStoreQuantity, connection, transaction))
                                    {
                                        cmdReStoreQuantity.Parameters.AddWithValue("@currentQuantity", currentQuantity);
                                        cmdReStoreQuantity.Parameters.AddWithValue("@productId", productId);
                                        cmdReStoreQuantity.ExecuteNonQuery();
                                    }*/
                                    Console.WriteLine("Remove product succesful.");
                                    break;
                                }
                            case 3:
                                {
                                    Console.WriteLine("Update cancelled.");
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine("Invalid option.");
                                    break;
                                }
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine("An error occurred: " + ex.Message);
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

        public void ViewCart(int customerId, MySqlConnection connection)
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
                        Console.WriteLine($"Cart Details for Customer ID: {customerId}");
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
                        Console.WriteLine($"Total Cost: {totalCost:F2}");
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

    }
}
