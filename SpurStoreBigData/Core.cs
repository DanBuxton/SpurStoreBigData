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
    public class Core
    {
        private ConcurrentDictionary<string, Store> stores = new ConcurrentDictionary<string, Store>();
        private ConcurrentStack<Date> dates = new ConcurrentStack<Date>();
        private ConcurrentBag<Order> orders = new ConcurrentBag<Order>();
        private ConcurrentDictionary<string, Supplier> suppliers = new ConcurrentDictionary<string, Supplier>();

        public string FolderPath { get; set; }
        public static string StoreCodesFile { get; private set; } = "StoreCodes.csv";
        public static string StoreDataFolder { get; private set; } = "StoreData";

        /// <summary>
        /// Get all stores from the available data. 
        /// </summary>
        /// <returns>All stores as <code>Store[]</code>. </returns>
        public Store[] GetStores()
        {
            try
            {
                return stores.Values.OrderBy(s => s.StoreCode).ToArray();
            }
            catch (ArgumentNullException e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }

        /// <summary>
        /// Get all supplier types from the available data. 
        /// </summary>
        /// <returns>All stores as <code>Supplier[]</code>. </returns>
        public string[] GetSupplierNames()
        {
            //string[] result = null;

            try
            {
                return suppliers.Keys
                    .OrderBy(s=>s)
                    .ToArray();

                //result = new string[suppliers.Count];
                //for (int i = 0; i < result.Length; i++)
                //{
                //    var s = suppliers.Values.OrderBy(v => v.Name).ToArray();
                //    result[i] = s[i].Name;
                //}
            }
            catch (Exception e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }

        /// <summary>
        /// Get all supplier types from the available data. 
        /// </summary>
        /// <returns>All supplier types as <code>string[]</code>. </returns>
        public string[] GetSupplierTypes()
        {
            try
            {
                return suppliers.Values.GroupBy(s=>s.Type).Select(s=>s.Key).OrderBy(s => s).ToArray();
            }
            catch (Exception e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }

        /// <summary>
        /// Get the total cost of all orders from the available data. 
        /// </summary>
        /// <returns>Cost of all orders or <code>NaN</code> if an issue arises. </returns>
        public double GetTotalCostOfAllOrders()
        {
            try
            {
                double result = 0.0;

                foreach (var o in orders) result += o.Cost;

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }

        /// <summary>
        /// Reload data from files via <code>FolderPath</code> property. 
        /// </summary>
        /// <returns>Custom <code>IOException</code> if issue found, else <code>null</code>. </returns>
        public IOException ReloadData(CancellationTokenSource cts) => LoadData(cts);

        private IOException LoadData(CancellationTokenSource cts)
        {
            IOException e = null;

            return Task<IOException>.Factory.StartNew(() =>
            {
                try
                {
                    stores = new ConcurrentDictionary<string, Store>();
                    dates = new ConcurrentStack<Date>();
                    orders = new ConcurrentBag<Order>();
                    suppliers = new ConcurrentDictionary<string, Supplier>();

                    string storeCodesFilePath = FolderPath + @"\" + StoreCodesFile;

                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    //foreach (var storeData in storeCodesData) // Take 0.01s
                    //{
                    //    string[] storeDataSplit = storeData.Split(',');

                    //    if (!stores.ContainsKey(storeDataSplit[0]))
                    //        stores.TryAdd(storeDataSplit[0], new Store(storeDataSplit[0], storeDataSplit[1]));

                    //    //storeDataSplit[0] = store code
                    //    //storeDataSplit[1] = store location
                    //}

                    string[] storeCodesData = File.ReadAllLines(storeCodesFilePath);
                    foreach (var storeData in storeCodesData) // Take 0.01s
                    {
                        string[] storeDataSplit = storeData.Split(',');

                        if (!stores.ContainsKey(storeDataSplit[0]))
                            stores.TryAdd(storeDataSplit[0], new Store(storeDataSplit[0], storeDataSplit[1]));
                    }

                    string[] fileNames = Directory.GetFiles(FolderPath + @"\" + StoreDataFolder);
                    Parallel.ForEach(fileNames, filePath =>
                    {
                        string fileNameExt = Path.GetFileName(filePath);
                        string fileName = Path.GetFileNameWithoutExtension(filePath);

                        string[] fileNameSplit = fileName.Split('_');
                        Store store = stores[fileNameSplit[0]];
                        Date date = new Date(Convert.ToInt32(fileNameSplit[1]), Convert.ToInt32(fileNameSplit[2]));
                        dates.Push(date);

                        string[] orderData = File.ReadAllLines(FolderPath + @"\" + StoreDataFolder + @"\" + fileNameExt);
                        foreach (var orderInfo in orderData)
                        {
                            string[] orderSplit = orderInfo.Split(',');

                            Supplier supplier = suppliers.GetOrAdd(orderSplit[0], new Supplier(orderSplit[0], orderSplit[1]));
                            Order o = new Order(store, date, supplier, Convert.ToDouble(orderSplit[2]));
                            orders.Add(o);
                        }
                    });

                    //foreach(var filePath in fileNames)
                    //Parallel.ForEach(fileNames, filePath =>
                    //{
                    //    string fileNameExt = Path.GetFileName(filePath);
                    //    string fileName = Path.GetFileNameWithoutExtension(filePath);

                    //    string[] fileNameSplit = fileName.Split('_');
                    //    Store store = stores[fileNameSplit[0]];
                    //    Date date = new Date(Convert.ToInt32(fileNameSplit[1]), Convert.ToInt32(fileNameSplit[2]));
                    //    dates.Push(date);

                    //    string[] orderData = File.ReadAllLines(FolderPath + @"\" + StoreDataFolder + @"\" + fileNameExt);
                    //    foreach (var orderInfo in orderData)
                    //    {
                    //        string[] orderSplit = orderInfo.Split(',');
                    //        Order order = new Order(store, date, orderSplit[0], orderSplit[1], Convert.ToDouble(orderSplit[2]));
                    //        orders.Add(order);

                    //        Supplier s = new Supplier(order.SupplierName, order.SupplierType);
                    //        if (!suppliers.Contains(s)) // O(n^2)
                    //        {
                    //            suppliers.Append(s);
                    //        }
                    //    }
                    //});
                    stopWatch.Stop();
                    Console.WriteLine("TimeToLoad: " + stopWatch.Elapsed.TotalSeconds); // For testing purposes. 
                }
                catch (DirectoryNotFoundException ex)
                {
                    e = new IOException("Unable to locate directory '" + FolderPath + "'", ex);
                }
                catch (FileNotFoundException ex)
                {
                    e = new IOException("Unable to locate StoreCodes.csv at '" + FolderPath + "'", ex);
                }
                catch (PathTooLongException ex)
                {
                    e = new IOException("File path '" + FolderPath + "' is too long", ex);
                }
                catch (UnauthorizedAccessException ex)
                {
                    e = new IOException("Unable to access the directory and/or files", ex);
                }
                catch (NotSupportedException ex)
                {
                    e = new IOException("The path '" + FolderPath + "' is not supported", ex);
                }
                catch (Exception ex)
                {
                    e = new IOException("Error loading store data", ex);
                }

                return e;
            }, cts.Token).Result;
        }
    }
}
