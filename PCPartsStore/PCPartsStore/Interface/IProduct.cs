using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store.Interface
{
    internal interface IProduct
    {
        public void ViewAllProduct();
        public void SearchProductById(int idFind);
        public void SearchProductByName(string name);
        public void SeaProductByCategory(string nameCategory);
        public void AddToCart(int idProduct);

    }
}
