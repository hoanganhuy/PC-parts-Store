using MySql.Data.MySqlClient;
using PC_Part_Store.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store.Implement
{
    public class Account :IAccount
    {
        public int idCustomer {  get; set; }
        public int idEmployee { get; set; }
        public string userName {  get; set; }
        public string password { get; set; }
        public string phoneNumber {  get; set; }
        public string email { get; set; }
        public string address {  get; set; }
        public string name {  get; set; }
        public void CreateAccountEmloyee(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public void Login(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public void Register(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public void RemoveAccountEmployee(int id, MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public void UpdateInformationCustomer(int id, MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public void UpdateInformationEmployee(int id, MySqlConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
