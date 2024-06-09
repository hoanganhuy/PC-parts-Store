﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store.Interface
{
    internal interface IOrder
    {
        public void ViewOrder(MySqlConnection connection);
        public void VerifyOrder(MySqlConnection connection);
        public void Pay(MySqlConnection connection);
    }
}
