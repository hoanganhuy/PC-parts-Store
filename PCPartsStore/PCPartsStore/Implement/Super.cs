using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Part_Store.Implement
{
    public abstract class Super<Model>
    {
        public abstract void Add();
        public abstract void Update();
        public abstract void Remove();
    }
}
