using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpurStoreBigData
{
    public class Date
    {
        public int Week { get; set; }
        public int Year { get; set; }

        public Date(int week, int year)
        {
            Week = week;
            Year = year;
        }

        public override string ToString()
        {
            return string.Format("{0, 2}/{1, -2}", Week, Year);
        }

        public override bool Equals(object obj)
        {
            return obj is Date d ? Week == d.Week && Year == d.Year : false;
        }

        public override int GetHashCode()
        {
            return Week * 27 +
                Year * 27;
        }
    }
}
