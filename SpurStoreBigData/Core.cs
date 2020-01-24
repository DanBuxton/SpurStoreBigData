using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpurStoreBigData
{
    public static class Core
    {
        public static ConcurrentDictionary<string, Store> Stores { get; private set; } = new ConcurrentDictionary<string, Store>();
        public static ConcurrentStack<Date> Dates { get; private set; } = new ConcurrentStack<Date>();
        public static ConcurrentBag<Order> Orders { get; private set; } = new ConcurrentBag<Order>();

        public static string FolderPath { get; set; } = "Data";
        public static string StoreCodesFile { get; } = "StoreCodes.csv";
        public static string StoreDataFolder { get; } = "StoreData";

        public static Store[] GetStores()
        {
            try
            {
                return Stores.Values.ToArray();
            }
            catch (Exception)
            {
                return new Store[] { };
            }
        }

        public static string[] GetTotalCostOfAllOrders()
        {
            try
            {
                List<string> result = null;

                return result.ToArray();
            }
            catch (Exception)
            {
                return new string[] { };
            }
        }

        public static void ReloadData() => LoadData();

        public static void LoadData()
        {
            try
            {
                string storeCodesFilePath = FolderPath + @"\" + StoreCodesFile;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                string[] storeCodesData = File.ReadAllLines(storeCodesFilePath);
                Parallel.ForEach(storeCodesData, storeData =>
                {
                    string[] storeDataSplit = storeData.Split(',');
                    Store store = new Store(storeDataSplit[0], storeDataSplit[1]);
                    if (!Stores.ContainsKey(store.StoreCode))
                        Stores.TryAdd(store.StoreCode, store);

                    //storeDataSplit[0] = store code
                    //storeDataSplit[1] = store location
                });
                //foreach (var storeData in storeCodesData)
                //{
                //    string[] storeDataSplit = storeData.Split(',');
                //    Store store = new Store(storeDataSplit[0], storeDataSplit[1]);
                //    if (!Stores.ContainsKey(store.StoreCode))
                //        Stores.TryAdd(store.StoreCode, store);

                //    //storeDataSplit[0] = store code
                //    //storeDataSplit[1] = store location
                //}

                string[] fileNames = Directory.GetFiles(FolderPath + @"\" + StoreDataFolder);

                Parallel.ForEach(fileNames, filePath =>
                {
                    string fileNameExt = Path.GetFileName(filePath);
                    string fileName = Path.GetFileNameWithoutExtension(filePath);

                    string[] fileNameSplit = fileName.Split('_');
                    Store store = Stores[fileNameSplit[0]];
                    Date date = new Date(Convert.ToInt32(fileNameSplit[1]), Convert.ToInt32(fileNameSplit[2]));
                    Dates.Append(date);

                    string[] orderData = File.ReadAllLines(FolderPath + @"\" + StoreDataFolder + @"\" + fileNameExt);
                    foreach (var orderInfo in orderData)
                    {
                        string[] orderSplit = orderInfo.Split(',');
                        Order order = new Order(store, date, orderSplit[0], orderSplit[1], Convert.ToDouble(orderSplit[2]));
                        Orders.Add(order);
                    }
                });

                stopWatch.Stop();
                Console.WriteLine("TimeToLoad: " + stopWatch.Elapsed.TotalSeconds);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new IOException("Unable to locate directory (" + FolderPath + ")", e);
            }
            catch (FileNotFoundException e)
            {
                throw new IOException("Unable to locate StoreCodes.csv at (" + FolderPath + ")", e);
            }
            catch (PathTooLongException e)
            {
                throw new IOException("File path (" + FolderPath + ") is too long", e);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new IOException(e.Message, e);
            }
            catch (Exception e)
            {
                throw new IOException("Error loading store data", e);
            }
        }
    }
}
