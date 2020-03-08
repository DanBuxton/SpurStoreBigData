using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpurStoreBigData
{
    public class Date : IComparable
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

        public int CompareTo(object obj)
        {
            if (!(obj is Date))
            {
                return 0;
            }
            else
            {
                Date y = (Date)obj;
                return (Year == y.Year ? (Week > y.Week ? 1 : -1) : (Year > y.Year ? 1 : -1));
            }
        }

        public DateTime GetDateTime()
        {
            DateTime jan1 = new DateTime(Year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            // Use first Thursday in January to get first week of the year as
            // it will never be in Week 52/53
            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = Week;
            // As we're adding days to a date in Week 1,
            // we need to subtract 1 in order to get the right date for week #1
            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            // Using the first Thursday as starting week ensures that we are starting in the right year
            // then we add number of weeks multiplied with days
            var result = firstThursday.AddDays(weekNum * 7);

            // Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
            return result.AddDays(-3);
        }
    }
}
