﻿using PC_Part_Store.Implement;
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
            Console.WriteLine("7. View information customer");
            Console.WriteLine("8. Add product to cart");
            Console.WriteLine("9. View order");
            Console.WriteLine("10. Back");
        }
        public void EmployeeMenu()
        {
            Console.WriteLine("Employee Dashboard");
            Console.WriteLine("1. Verify Order");
            Console.WriteLine("2. Product management");
            Console.WriteLine("3. Back");
            //Console.WriteLine("4. Back");
        }
        public void ProductManagemnt()
        {
            Console.WriteLine("Product management.");
            Console.WriteLine("1. Next page");
            Console.WriteLine("2. Previous page");
            Console.WriteLine("3. Go to page");
            Console.WriteLine("4. View details product");
            Console.WriteLine("5. Search product");
            Console.WriteLine("6. Add product");
            Console.WriteLine("7. Remove product");
            Console.WriteLine("8. Back");
        }
        public void AdminMenu()
        {
            Console.WriteLine("Admin Menu");
            Console.WriteLine("1. Next page");
            Console.WriteLine("2. Previous page");
            Console.WriteLine("3. Go to page");
            Console.WriteLine("4. Create account Employee");
            Console.WriteLine("5. View Employee Details");
            Console.WriteLine("6. Delete Employee by ID");
            Console.WriteLine("7. Update Employee Information by ID");
            Console.WriteLine("8. Search Employee");
            Console.WriteLine("9. Back");
            //Console.WriteLine("7. Quit");
        }
        
        public void SearchMenu()
        {        
            Console.WriteLine("Search Menu:");
            Console.WriteLine("1. Search by Name");
            Console.WriteLine("2. Search by Category"); ;
            Console.WriteLine("3. Back");
        }
        public void SearchMenuOption()
        {
            Console.WriteLine("1. Next page");
            Console.WriteLine("2. Previous page");
            Console.WriteLine("3. Go to page");
            Console.WriteLine("4. View details product");
            Console.WriteLine("5. Add product to cart");
            Console.WriteLine("6. Back");
        }
        public void SearchMenuEmployeeOption()
        {
            Console.WriteLine("1. Next page");
            Console.WriteLine("2. Previous page");
            Console.WriteLine("3. Go to page");
            Console.WriteLine("4. View information employee");
            Console.WriteLine("5. Back");
        }
        public void SearchMenuEmployee()
        {
            Console.WriteLine("1. Next page");
            Console.WriteLine("2. Previous page");
            Console.WriteLine("3. Go to page");
            Console.WriteLine("4. View details product");
            Console.WriteLine("5. Back");
        }
    }
}

