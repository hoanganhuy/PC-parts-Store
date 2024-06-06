using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store.Interface
{
    internal interface IProduct
    {
        public void ViewAllProduct(MySqlConnection connection);
        public void SearchProductById(int idFind, MySqlConnection connection);
        public void SearchProductByName(string name, MySqlConnection connection);
        public void SeaProductByCategory(int categoryId, MySqlConnection connection);
        public void AddToCart(int idProduct, MySqlConnection connection);

    }
}
