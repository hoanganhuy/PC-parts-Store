using MySql.Data.MySqlClient;
using PC_Part_Store.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store.Implement
{
    public class Cart : Super<Cart>, ICart
    {
        public int orderId {  get; set; }
        public int quantity {  get; set; }
        public int idCart {  get; set; }
        public int idCustomer {  get; set; }
        public override void Add(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }
        public void Pay(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public override void Remove(MySqlConnection connection,int id)
        {
            throw new NotImplementedException();
        }

        public override void Update(MySqlConnection connection, int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateProduct(int id, MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public void ViewCart(int idCart, MySqlConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
