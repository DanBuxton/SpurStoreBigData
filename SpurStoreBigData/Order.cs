using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpurStoreBigData
{
    public class Order
    {
        public Store Store { get; protected set; }
        public Date Date { get; protected set; }
        public Supplier Supplier { get; protected set; }
        public double Cost { get; protected set; }

        public Order(Store store, Date date, Supplier supplier, double cost)
        {
            Store = store;
            Date = date;
            Supplier = supplier;
            Cost = cost;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2:s} {3, 10:s} {4:C} cost", Store, Date, Supplier.Name, Supplier.Type, Cost);
        }
    }
}
