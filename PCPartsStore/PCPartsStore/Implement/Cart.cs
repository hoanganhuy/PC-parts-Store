using PC_Part_Store.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store.Implement
{
    public class Cart : Super<Cart>, ICart
    {
        public int idCart {  get; set; }
        public int idCustomer {  get; set; }
        public int idProduct {  get; set; }

        public override void Add()
        {
            throw new NotImplementedException();
        }

        public void Pay()
        {
            throw new NotImplementedException();
        }

        public override void Remove()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public void UpdateProduct(int id)
        {
            throw new NotImplementedException();
        }

        public void ViewCart(int idCart)
        {
            throw new NotImplementedException();
        }
    }
}
