using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store.Interface
{
    internal interface IAccount
    {
        public void Login(MySqlConnection connection);
        public void Register(MySqlConnection connection);
        public void UpdateInformationCustomer(int id, MySqlConnection connection);
        public void UpdateInformationEmployee(int id, MySqlConnection connection);
        public void CreateAccountEmloyee(MySqlConnection connection);
        public void RemoveAccountEmployee(int id, MySqlConnection connection);
    }
}
