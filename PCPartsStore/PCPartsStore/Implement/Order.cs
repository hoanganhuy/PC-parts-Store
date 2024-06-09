using MySql.Data.MySqlClient;
using PC_Part_Store.Interface;

namespace PC_Part_Store.Implement
{
    internal class Order : IOrder
    {
        public int idOrder {  get; set; }
        public int idCustomer {  get; set; }
        public string customerName { get; set; }
        public string customerPhone { get; set; }
        public string customerEmail { get; set; }
        public string customerAddress { get; set; }
        public bool verifyed {  get; set; }
        public bool Accepted {  get; set; }

        public void Pay(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public void VerifyOrder(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public void ViewOrder(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
