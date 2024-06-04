using PC_Part_Store.Interface;

namespace PC_Part_Store.Implement
{
    internal class Product : Super<Product>, IProduct
    {
        public int idProduct {  get; set; }
        public string nameProduct {  get; set; }
        public string descriptionProduct { get; set; }
        public float price { get; set; }
        public int quantity { get; set; }
        public string brand {  get; set; }
        public string category {  get; set; }
        public override void Add()
        {
            throw new NotImplementedException();
        }

        public void AddToCart(int idProduct)
        {
            throw new NotImplementedException();
        }

        public override void Remove()
        {
            throw new NotImplementedException();
        }

        public void SeaProductByCategory(string nameCategory)
        {
            throw new NotImplementedException();
        }

        public void SearchProductById(int idFind)
        {
            throw new NotImplementedException();
        }

        public void SearchProductByName(string name)
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        public void ViewAllProduct()
        {
            throw new NotImplementedException();
        }
    }
}
