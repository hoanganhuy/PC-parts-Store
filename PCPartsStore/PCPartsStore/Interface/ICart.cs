using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store.Interface
{
    internal interface ICart
    {
        public void ViewCart(int idCart);
        public void UpdateProduct(int id);
        public void Pay();
    }
}
