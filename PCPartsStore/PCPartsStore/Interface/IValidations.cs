using MySql.Data.MySqlClient;
using PC_Part_Store.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPartsStore.Interface
{
    public interface IValidations
    {
        public bool AccountExistCheck(string username, string password);
        bool ProductExistCheck(int productId, MySqlConnection connection);
        bool ProductRemainingCheck(int productId);
        bool CustomerUsernameDuplicateCheck(string username);
        bool EmployeeUsernameDuplicateCheck(string username);
        bool UsernameFormCheck(string username);
    }
}
