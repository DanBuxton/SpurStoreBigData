using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpurStoreBigData
{
    public class Store
    {
        public string StoreCode { get; set; }
        public string StoreLocation { get; set; }

        public Store(string code, string location)
        {
            StoreCode = code;
            StoreLocation = location;
        }

        public override string ToString()
        {
            return string.Format("Code: {0}; Location: {1};", StoreCode, StoreLocation);
        }
    }
}
