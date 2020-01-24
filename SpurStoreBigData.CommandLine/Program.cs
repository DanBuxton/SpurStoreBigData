using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpurStoreBigData.CommandLine
{
    class Program
    {
        private static string FolderPath = "Data";
        private static readonly string StoreCodesFile = "StoreCodes.csv";
        private static readonly string StoreDataFolder = "StoreData";

        static void Main(string[] args)
        {
            Console.Title = "Spur Ltd Big Data";



            LoadData();
        }

        private static void LoadData()
        {
            string storeCodesFilePath = Directory.GetCurrentDirectory() + @"\" + FolderPath + @"\" + StoreCodesFile;
            //FolderPath = @"F:\BSc Software Engineering\Year 2\Task-based Software Engineering\Data\";
            //storeCodesFilePath = FolderPath + @"\" + StoreCodesFile;

            Dictionary<string, Store> stores = new Dictionary<string, Store>();
            HashSet<Date> dates = new HashSet<Date>();
            List<Order> orders = new List<Order>();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            string[] storeCodesData = File.ReadAllLines(storeCodesFilePath);
            foreach (var storeData in storeCodesData)
            {
                string[] storeDataSplit = storeData.Split(',');
                Store store = new Store { StoreCode = storeDataSplit[0], StoreLocation = storeDataSplit[1] };
                if (!stores.ContainsKey(store.StoreCode))
                    stores.Add(store.StoreCode, store);

                //storeDataSplit[0] = store code
                //storeDataSplit[1] = store location
            }

            string[] fileNames = Directory.GetFiles(FolderPath + @"\" + StoreDataFolder);

            Parallel.ForEach(fileNames, filePath =>
            {
                string fileNameExt = Path.GetFileName(filePath);
                string fileName = Path.GetFileNameWithoutExtension(filePath);

                string[] fileNameSplit = fileName.Split('_');
                Store store = stores[fileNameSplit[0]];
                Date date = new Date { Week = Convert.ToInt32(fileNameSplit[1]), Year = Convert.ToInt32(fileNameSplit[2]) };
                dates.Add(date);
                //fileNameSplit[0] = store code
                //fileNameSplit[1] = week number
                //fileNameSplit[2] = year

                string[] orderData = File.ReadAllLines(FolderPath + @"\" + StoreDataFolder + @"\" + fileNameExt);
                foreach (var orderInfo in orderData)
                {
                    string[] orderSplit = orderInfo.Split(',');
                    Order order = new Order
                    {
                        Store = store,
                        Date = date,
                        SupplierName = orderSplit[0],
                        SupplierType = orderSplit[1],
                        Cost = Convert.ToDouble(orderSplit[2])
                    };
                    orders.Add(order);
                    //orderSplit[0] = supplier name
                    //orderSplit[1] = supplier type
                    //orderSplit[2] = cost
                }
            });

            //foreach (var filePath in fileNames)
            //{
            //    string fileNameExt = Path.GetFileName(filePath);
            //    string fileName = Path.GetFileNameWithoutExtension(filePath);

            //    string[] fileNameSplit = fileName.Split('_');
            //    Store store = stores[fileNameSplit[0]];
            //    Date date = new Date { Week = Convert.ToInt32(fileNameSplit[1]), Year = Convert.ToInt32(fileNameSplit[2]) };
            //    dates.Add(date);
            //    //fileNameSplit[0] = store code
            //    //fileNameSplit[1] = week number
            //    //fileNameSplit[2] = year

            //    string[] orderData = File.ReadAllLines(FolderPath + @"\" + StoreDataFolder + @"\" + fileNameExt);
            //    foreach (var orderInfo in orderData)
            //    {
            //        string[] orderSplit = orderInfo.Split(',');
            //        Order order = new Order
            //        {
            //            Store = store,
            //            Date = date,
            //            SupplierName = orderSplit[0],
            //            SupplierType = orderSplit[1],
            //            Cost = Convert.ToDouble(orderSplit[2])
            //        };
            //        orders.Add(order);
            //        //orderSplit[0] = supplier name
            //        //orderSplit[1] = supplier type
            //        //orderSplit[2] = cost
            //    }
            //}

            stopWatch.Stop();
            Console.WriteLine("TimeToLoad: " + stopWatch.Elapsed.TotalSeconds);
        }
    }
}
