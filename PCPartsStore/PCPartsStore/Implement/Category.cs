using MySql.Data.MySqlClient;
using PC_Part_Store.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCPartsStore.Implement
{
    internal class Category : Super<Category>
    {
        public int idCategory {  get; set; }
        public string nameCategory {  get; set; }
        public override void Add(MySqlConnection connection)
        {
            Console.WriteLine("Enter name category:");
            nameCategory = Console.ReadLine();
            string query = "insert into categories(categoryName) values (@categoryName)";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("categoryName", nameCategory);
                cmd.ExecuteNonQuery();
            }
        }

        public override void Remove(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }

        public override void Update(MySqlConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
