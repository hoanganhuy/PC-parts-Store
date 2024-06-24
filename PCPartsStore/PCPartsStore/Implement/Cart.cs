using MySql.Data.MySqlClient;
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
                        string getCartIdQuery = "SELECT cartId FROM cart WHERE customerId=@customerId";
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
                        ViewCart(idCart, connection);
                        Console.Write("Enter the product id you want to update: ");
                        int productId = int.Parse(Console.ReadLine());
                        //lay so luong hang hien tai trong cart details
                        string queryCheckProductInCart = "SELECT amount FROM cartDetails WHERE cart_id = @cartId AND product_id = @productId;";
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
                                    string queryCheckAmountProduct = "SELECT quantity FROM products WHERE product_id = @productId";
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
                                    string queryUpdateAmount = "UPDATE cartDetails SET amount = @newAmount WHERE cartId = @cartId AND productId = @productId";
                                    using (MySqlCommand cmdUpdateAmount = new MySqlCommand(queryUpdateAmount, connection, transaction))
                                    {
                                        cmdUpdateAmount.Parameters.AddWithValue("@newQuantity", newAmount);
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
                                    string queryRemoveProductInCart = "DELETE FROM cartDetails WHERE cartId = @cartId AND productId = @productId";
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

        public void ViewCart(int idCart, MySqlConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
