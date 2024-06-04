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
        public string userName {  get; set; }
        public string password { get; set; }
        public string phoneNumber {  get; set; }
        public string email { get; set; }
        public string address {  get; set; }
        public string name {  get; set; }
        public void CreateAccountEmloyee()
        {
            throw new NotImplementedException();
        }

        public void Login()
        {
            throw new NotImplementedException();
        }

        public void Register()
        {
            throw new NotImplementedException();
        }

        public void UpdateInformationCustomer(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateInformationEmployee(int id)
        {
            throw new NotImplementedException();
        }
    }
}
