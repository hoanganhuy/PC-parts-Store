﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store.Interface
{
    public interface IProduct
    {
        public void ViewAllProduct(MySqlConnection connection);
        public void SearchProductById(int idFind, MySqlConnection connection);
        public void SearchProductByName(string name, MySqlConnection connection);
        public void SeaProductByCategory(int categoryId, MySqlConnection connection);
        public void AddToCart(int productId,int customerId,int amount, MySqlConnection connection);
        public void viewProductDetails(int productId,MySqlConnection connection);

    }
}
