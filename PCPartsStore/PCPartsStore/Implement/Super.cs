using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store.Implement
{
    public abstract class Super<Model>
    {
        public abstract void Add(MySqlConnection connection);
        public abstract void Update (MySqlConnection connection,int id);
        public abstract void Remove(MySqlConnection connection,int id);
    }
}
