﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store.Interface
{
    internal interface ICart
    {
        public int ViewCart(int customerId, MySqlConnection connection);
        public void UpdateProductToCart(int customerId, MySqlConnection connection);

    }
}
