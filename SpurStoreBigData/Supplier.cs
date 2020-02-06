using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpurStoreBigData
{
    public class Supplier
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public Supplier(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public override string ToString()
        {
            return string.Format("{0, 15s} : {1, -5}", Name, Type);
        }
    }
}
