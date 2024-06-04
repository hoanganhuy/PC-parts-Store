using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store.Interface
{
    internal interface IAccount
    {
        public void Login();
        public void Register();
        public void UpdateInformationCustomer(int id);
        public void UpdateInformationEmployee(int id);
        public void CreateAccountEmloyee();
        public void RemoveAccountEmployee(int id);
    }
}
