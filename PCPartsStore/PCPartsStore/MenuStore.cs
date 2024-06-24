using PC_Part_Store.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store
{
    public class MenuStore
    {
        public void MenuLogin()
        {
            Console.WriteLine("Welcome to PC STORE");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Exit");
        }
        public void CustomerMenu()
        {
            Console.WriteLine("1. Previous page");
            Console.WriteLine("2. Next page");
            Console.WriteLine("3. Go to page");
            Console.WriteLine("4. View details product");
            Console.WriteLine("5. Search product");
            Console.WriteLine("6. View cart");
            Console.WriteLine("7. Update infomation customer");
            Console.WriteLine("8. Add product to cart");
            Console.WriteLine("9. Quit");
        }
        public void EmployeeMenu()
        {
            Console.WriteLine("Employee Dashboard");
            Console.WriteLine("1. Approve application");
            Console.WriteLine("2. Product management");
            Console.WriteLine("3. Back");
            Console.WriteLine("4. Quit");
        }
        public void AdminMenu()
        {
            Console.WriteLine("Admin Menu");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. View Employee Details");
            Console.WriteLine("3. Delete Employee by ID");
            Console.WriteLine("4. Update Employee Information by ID");
            Console.WriteLine("5. Search Employee");
            Console.WriteLine("6. Back");
            Console.WriteLine("7. Quit");
        }
        
        public void SearchMenu()
        {        
            Console.WriteLine("Search Menu:");
            Console.WriteLine("1. Search by Name");
            Console.WriteLine("2. Search by Category"); ;
            Console.WriteLine("3. Quit");
        }
        public void SearchMenuOption()
        {
            Console.WriteLine("1. Next page");
            Console.WriteLine("2. Previous page");
            Console.WriteLine("3. Go to page");
            Console.WriteLine("4. View details product");
            Console.WriteLine("5. Add product to cart");
            Console.WriteLine("6. Quit");
        }
    }
}

