using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Windows.Forms.DataVisualization.Charting;

namespace SpurStoreBigData.GraphicalUserInterface
{
    public partial class MainForm : Form
    {
        delegate void UpdateGraphDelegate();

        private UpdateGraphDelegate updateGraphDelegate;

        public Core Core { get; set; } = Core.Instance;

        readonly List<string> x = new List<string>(); // Date
        readonly List<double> y = new List<double>(); // Cost

        public MainForm(Chart chart = null)
        {
            InitializeComponent();

            if (chart == null)
            {
                chart1.Series["Series1"].LegendText = "Cost over time";
            }
            else
            {
                chart1 = chart;
            }

            chart1.Series["Series1"].Color = Color.Red;

            updateGraphDelegate = new UpdateGraphDelegate(UpdateGraph);
        }

        private void Setup()
        {
            Core.FolderPath = @"C:\temp\Data";
            Task.Factory.StartNew(() => Core.ReloadData(new System.Threading.CancellationTokenSource()))
               .ContinueWith((t) =>
               {
                   var orders = Core.Orders.GroupBy(o => o.Date);

                   var dates = Core.Dates;
                   dates.Sort();

                   foreach (Date date in dates)
                   {
                       x.Add(date.Week + "/" + (date.Year - 2000));
                   }
               })
               .ContinueWith((t) =>
               {
                   Parallel.For(0, Core.Dates.Count, i =>
                   {

                       var date = Core.Dates[i];
                    //var orders = Core.Orders.AsParallel().Where(o => o.Date.Equals(date));

                    y.Add(Core.GetTotalCostOfAllOrdersInAWeek(date.Week, date.Year));
                   });

                //for (int j = 2013; j <= 2014; j++)
                //{
                //    Parallel.For(1, 53, new ParallelOptions() { MaxDegreeOfParallelism = 3 }, i =>
                //    {
                //        y.Add(Core.GetTotalCostOfAllOrdersInAWeek(i, j));
                //    });

                //    //for (int i = 1; i <= 52; i++)
                //    //{
                //    //y.Add(Core.GetTotalCostOfAllOrdersInAWeek(i, j));
                //    //}
                //}
            })
               .ContinueWith(t =>
               {
                   UpdateGraph();
               });
        }

        private void UpdateGraph()
        {
            if (chart1.InvokeRequired)
            {
                chart1.Invoke(updateGraphDelegate);
            }
            else
            {

                chart1.Series["Series1"].Points.DataBindXY(x, y);
                chart1.ChartAreas[0].AxisX.Interval = 4;
                //chart1.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Weeks;
                chart1.ChartAreas[0].AxisY.LabelStyle.Format = "C";
                //chart1.Titles[0].Text = "";
                //chart1.ChartAreas[0].AxisX.LabelStyle.Format = "dd/M/yyyy";
            }
        }
    }
}
