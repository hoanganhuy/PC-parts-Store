using PC_Part_Store.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPartsStore.Interface
{
    public interface IValidation
    {
        bool UsernameFormCheck(string username);
        bool UsernameDuplicateCheck(string username);
        bool AccountExistCheck(string username, string password);
        bool CustomerCheck(int customerId);
        bool EmployeeCheck(int employeeId);
        bool ProductExistCheck (int productId);
        bool ProductIdFormCheck(int productId);
        bool ProductIdExistCheck(int productId);
        bool ProductRemainingCheck(int productId);
        bool OrderDeclineCheck(int orderId);
        //bool ProductInformationCheck(Product product);
    }
}
