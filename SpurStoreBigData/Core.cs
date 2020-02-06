using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpurStoreBigData
{
    public static class Core
    {
        private static ConcurrentDictionary<string, Store> Stores { get; set; } = new ConcurrentDictionary<string, Store>();
        private static ConcurrentStack<Date> Dates { get; set; } = new ConcurrentStack<Date>();
        private static ConcurrentBag<Order> Orders { get; set; } = new ConcurrentBag<Order>();

        public static bool LoadCompleted { get; private set; }

        public static int StoresCount { get { return Stores.Count; } }
        public static int DatesCount { get { return Dates.Count; } }
        public static int OrdersCount { get { return Orders.Count; } }

        public static string FolderPath { get; set; }
        public static string StoreCodesFile { get; private set; } = "StoreCodes.csv";
        public static string StoreDataFolder { get; private set; } = "StoreData";

        /// <summary>
        /// Get all stores from the available data. 
        /// </summary>
        /// <returns>All stores as <code>Store[]</code>. </returns>
        public static Store[] GetStores()
        {
            try
            {
                return Stores.Values.OrderBy(s => s.StoreCode).ToArray();
            }
            catch (ArgumentNullException e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }

        /// <summary>
        /// Get the total cost of all orders from the available data. 
        /// </summary>
        /// <returns>Cost of all orders or <code>NaN</code> if an issue arises. </returns>
        public static double GetTotalCostOfAllOrders()
        {
            try
            {
                double result = 0.0;

                foreach (var o in Orders) result += o.Cost;

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }

        /// <summary>
        /// Reload data from files via <code>FolderPath</code> property. 
        /// NEED A BETTER WAY OF DOING THIS. 
        /// </summary>
        public static void ReloadData()
        {
            IOException ioE = null;

            LoadCompleted = false;

            Task.Run(() =>
            {
                //CancellationToken ct = Task.Factory.CancellationToken;

                try
                {
                    LoadData();
                }
                catch (IOException e)
                {
                    ioE = e;
                }

                LoadCompleted = true;
            });

            // USE THREADS
            while (!LoadCompleted) ; // Need a better way of doing this. 

            if (ioE != null)
            {
                throw ioE;
            }

            //IOException ioE = null;
            //Task.Factory.StartNew(async () =>
            //{
            //    try
            //    {
            //        await LoadData();
            //    }
            //    catch (DirectoryNotFoundException e)
            //    {
            //        ioE = new IOException("Unable to locate directory (" + FolderPath + ")", e);
            //    }
            //    catch (FileNotFoundException e)
            //    {
            //        ioE = new IOException("Unable to locate StoreCodes.csv at (" + FolderPath + ")", e);
            //    }
            //    catch (PathTooLongException e)
            //    {
            //        ioE = new IOException("File path (" + FolderPath + ") is too long", e);
            //    }
            //    catch (UnauthorizedAccessException e)
            //    {
            //        ioE = new IOException("Unable to access the directory and/or files", e);
            //    }
            //    catch (Exception e)
            //    {
            //        ioE = new IOException("Error loading store data", e);
            //    }
            //});

            //if (ioE != null)
            //{
            //    throw ioE;
            //}
        }

        private static /*Task*/ void LoadData()
        {
            try
            {
                Stores = new ConcurrentDictionary<string, Store>();
                Dates = new ConcurrentStack<Date>();
                Orders = new ConcurrentBag<Order>();

                string storeCodesFilePath = FolderPath + @"\" + StoreCodesFile;

                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                string[] fileNames = Directory.GetFiles(FolderPath + @"\" + StoreDataFolder);
                string[] storeCodesData = File.ReadAllLines(storeCodesFilePath);
                foreach (var storeData in storeCodesData) // Take 0.01s
                {
                    string[] storeDataSplit = storeData.Split(',');
                    Store store = new Store(storeDataSplit[0], storeDataSplit[1]);
                    if (!Stores.ContainsKey(store.StoreCode))
                        Stores.TryAdd(store.StoreCode, store);

                    //storeDataSplit[0] = store code
                    //storeDataSplit[1] = store location
                }

                //foreach(var filePath in fileNames)
                Parallel.ForEach(fileNames, filePath =>
                {
                    string fileNameExt = Path.GetFileName(filePath);
                    string fileName = Path.GetFileNameWithoutExtension(filePath);

                    string[] fileNameSplit = fileName.Split('_');
                    Store store = Stores[fileNameSplit[0]];
                    Date date = new Date(Convert.ToInt32(fileNameSplit[1]), Convert.ToInt32(fileNameSplit[2]));
                    Dates.Push(date);

                    string[] orderData = File.ReadAllLines(FolderPath + @"\" + StoreDataFolder + @"\" + fileNameExt);
                    foreach (var orderInfo in orderData)
                    {
                        string[] orderSplit = orderInfo.Split(',');
                        Order order = new Order(store, date, orderSplit[0], orderSplit[1], Convert.ToDouble(orderSplit[2]));
                        Orders.Add(order);
                    }
                });

                stopWatch.Stop();
                Console.WriteLine("TimeToLoad: " + stopWatch.Elapsed.TotalSeconds); // For testing purposes. 
            }
            catch (DirectoryNotFoundException e)
            {
                throw new IOException("Unable to locate directory '" + FolderPath + "'", e);
            }
            catch (FileNotFoundException e)
            {
                throw new IOException("Unable to locate StoreCodes.csv at '" + FolderPath + "'", e);
            }
            catch (PathTooLongException e)
            {
                throw new IOException("File path '" + FolderPath + "' is too long", e);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new IOException("Unable to access the directory and/or files", e);
            }
            catch (NotSupportedException e)
            {
                throw new IOException("The path '" + FolderPath + "' is not supported", e);
            }
            catch (Exception e)
            {
                throw new IOException("Error loading store data", e);
            }

            //return Task.CompletedTask;
        }
    }
}
