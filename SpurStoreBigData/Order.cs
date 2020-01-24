using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpurStoreBigData
{
    public class Order
    {
        public Store Store { get; set; }

        public Date Date { get; set; }
        public string SupplierName { get; set; }
        public string SupplierType { get; set; }
        public double Cost { get; set; }

        public Order(Store store, Date date, string supplierName, string supplierType, double cost)
        {
            Store = store;
            Date = date;
            SupplierName = supplierName;
            SupplierType = supplierType;
            Cost = cost;
        }
    }
}
