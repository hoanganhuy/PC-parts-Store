using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using PC_Part_Store.Interface;
using PCPartsStore;
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
        Validations validations = new Validations();
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
                        int productId = validations.CheckInt();
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
                        Console.WriteLine("2.Remove product from cart");
                        Console.WriteLine("3.Cancel update");
                        Console.Write("Choose an option: ");
                        int option = validations.CheckInt();
                        switch (option)
                        {
                            case 1:
                                {
                                    int newAmount;
                                    do
                                    {
                                        Console.Write("Enter the new amount: ");
                                        newAmount = int.Parse(Console.ReadLine());
                                        if (newAmount > 0)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Amount Invalid");
                                        }
                                    } while (true);
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
                                    if (newAmount > currentQuantity)
                                    {
                                        Console.WriteLine("Insufficient products in stock");
                                        return;
                                    }
                                    string queryUpdateAmount = "UPDATE cart_Detail SET Amount = @newAmount WHERE Cart_ID = @cartId AND Product_ID = @productId";
                                    using (MySqlCommand cmdUpdateAmount = new MySqlCommand(queryUpdateAmount, connection, transaction))
                                    {
                                        cmdUpdateAmount.Parameters.AddWithValue("@newAmount", newAmount);
                                        cmdUpdateAmount.Parameters.AddWithValue("@cartId", cartId);
                                        cmdUpdateAmount.Parameters.AddWithValue("@productId", productId);
                                        cmdUpdateAmount.ExecuteNonQuery();
                                    }
                                    //cap nhat so luong thuc te cua san pham trong kho
                                    string updateProductStockQuery = "UPDATE product SET quantity = quantity - @quantityChange WHERE product_id = @productId;";
                                    using (MySqlCommand updateProductStockCmd = new MySqlCommand(updateProductStockQuery, connection, transaction))
                                    {
                                        updateProductStockCmd.Parameters.AddWithValue("@quantityChange", newAmount);
                                        updateProductStockCmd.Parameters.AddWithValue("@productId", productId);
                                        updateProductStockCmd.ExecuteNonQuery();
                                    }
                                    Console.WriteLine("Product quantity updated successfully.");
                                    break;
                                }
                            case 2:
                                {
                                    string queryRemoveProductInCart = "DELETE FROM Cart_Detail WHERE cart_ID = @cartId AND Product_ID = @productId";
                                    using (MySqlCommand cmdRemoveProductInCart = new MySqlCommand(queryRemoveProductInCart, connection, transaction))
                                    {
                                        cmdRemoveProductInCart.Parameters.AddWithValue("@cartId", cartId);
                                        cmdRemoveProductInCart.Parameters.AddWithValue("@productId", productId);
                                        cmdRemoveProductInCart.ExecuteNonQuery();
                                    }

                                    // Cập nhật số lượng trong kho
                                    string queryReStoreQuantity = "UPDATE product SET quantity = quantity + @currentQuantity WHERE product_id = @productId";
                                    using (MySqlCommand cmdReStoreQuantity = new MySqlCommand(queryReStoreQuantity, connection, transaction))
                                    {
                                        cmdReStoreQuantity.Parameters.AddWithValue("@currentQuantity", currentQuantity);
                                        cmdReStoreQuantity.Parameters.AddWithValue("@productId", productId);
                                        cmdReStoreQuantity.ExecuteNonQuery();
                                    }

                                    Console.WriteLine("Remove product successful.");
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

        public int ViewCart(int customerId, MySqlConnection connection)
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
                        return 0;
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
                        if (!reader.HasRows)
                        {
                            Console.WriteLine("The cart is empty.");
                            return 0;
                        }
                        Console.WriteLine($"Cart Details for Customer ID: {customerId}");
                        Console.WriteLine("+------------+----------------------+----------+--------+----------+");
                        Console.WriteLine("| Product ID | Product Name         | Price    | Amount | Cost     |");
                        Console.WriteLine("+------------+----------------------+----------+--------+----------+");
                        decimal totalCost = 0;
                        while (reader.Read())
                        {
                            int productId = reader.GetInt32("Product_Id");
                            string productName = reader.GetString("Product_Name");
                            decimal price = reader.GetDecimal("Price");
                            int amount = reader.GetInt32("Amount");
                            decimal cost = price * amount;

                            Console.WriteLine($"| {productId,-10} | {productName,-20} | {price,8:F2} | {amount,6} | {cost,8:F2} |");
                            totalCost += cost;
                        }
                        Console.WriteLine("+------------+----------------------+----------+--------+----------+");
                        Console.WriteLine($"Total Cost: {totalCost:F2}");
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

    }
}
