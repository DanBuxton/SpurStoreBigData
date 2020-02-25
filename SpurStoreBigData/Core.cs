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
    /// <summary>
    /// The 
    /// </summary>
    public class Core
    {
        /// <summary>
        /// The only instance of the class <see cref="Core"/>
        /// </summary>
        public static Core Instance { get; private set; } = new Core();
        private Core() { }

        private ConcurrentDictionary<string, Store> Stores = new ConcurrentDictionary<string, Store>();
        private ConcurrentStack<Date> Dates = new ConcurrentStack<Date>();
        private ConcurrentBag<Order> Orders = new ConcurrentBag<Order>();
        private ConcurrentDictionary<string, Supplier> Suppliers = new ConcurrentDictionary<string, Supplier>();

        /// <summary>
        /// 
        /// </summary>
        public string FolderPath { get; set; }

        /// <summary>
        /// The filename that contains the store codes
        /// </summary>
        public static string StoreCodesFile { get; private set; } = "StoreCodes.csv";
        /// <summary>
        /// The Folder name that contains all the store data
        /// </summary>
        public static string StoreDataFolder { get; private set; } = "StoreData";

        /// <summary>
        /// Get all stores from the available data
        /// </summary>
        /// <returns>All stores as <c><see cref="Store"/>[]</c> or throws <c>Exception</c> if an issue arises</returns>
        public Store[] GetStores()
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
        /// Get all supplier names from the available data
        /// </summary>
        /// <returns>All supplier names as <c>string[]</c> or throws <c>Exception</c> if an issue arises</returns>
        public string[] GetSupplierNames()
        {
            //string[] result = null;

            try
            {
                return Suppliers.Keys
                    .OrderBy(s => s)
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
        /// Get all supplier types from the available data
        /// </summary>
        /// <returns>All supplier types as <c>string[]</c> or throws <c>Exception</c> if an issue arises</returns>
        public string[] GetSupplierTypes()
        {
            try
            {
                return Suppliers.Values.GroupBy(s => s.Type).Select(s => s.Key).OrderBy(s => s).ToArray();
            }
            catch (Exception e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }

        /// <summary>
        /// Get the total cost of all orders from the available data
        /// </summary>
        /// <returns>Cost of all orders or throws <c>Exception</c> if an issue arises</returns>
        public double GetTotalCostOfAllOrders()
        {
            try
            {
                double result = 0.00;

                foreach (Order o in Orders) result += o.Cost;

                //Orders.AsParallel().ForAll(o => result += o.Cost);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }
        /// <summary>
        /// Get the total cost of all orders for a particular store
        /// </summary>
        /// <param name="storeCode">Store code</param>
        /// <returns>Cost of orders or throws <c>Exception</c> if an issue arises</returns>
        public double GetTotalCostOfAllOrdersForAStore(string storeCode)
        {
            try
            {
                Stopwatch s = new Stopwatch();
                s.Start();
                double result = 0.00;

                foreach (Order o in Orders.AsParallel().Where(o => o.Store.StoreCode == storeCode.ToUpper())) result += o.Cost;

                s.Stop();
                Console.WriteLine(s.Elapsed.TotalSeconds);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }
        /// <summary>
        /// Get the total cost of all orders in a week and year
        /// </summary>
        /// <param name="week">Week number between 1 - 52</param>
        /// <param name="year">Year number E.G. 2014</param>
        /// <returns>Cost of orders or throws <c>Exception</c> if an issue arises</returns>
        public double GetTotalCostOfAllOrdersInAWeek(int week, int year)
        {
            try
            {
                Stopwatch s = new Stopwatch();
                s.Start();
                double result = 0.00;

                foreach (Order o in Orders.AsParallel().Where(o => o.Date.Week.Equals(week)).Where(o=>o.Date.Year.Equals(year)).AsSequential()) result += o.Cost;

                s.Stop();
                Console.WriteLine(s.Elapsed.TotalSeconds);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }
        /// <summary>
        /// Get the total cost of all orders in a week and year for a store
        /// </summary>
        /// <param name="week">Week number between 1 - 52</param>
        /// <param name="year">Year number E.G. 2014</param>
        /// <param name="storeCode">Store code</param>
        /// <returns>Cost of all orders or throws <c>Exception</c> if an issue arises</returns>
        public double GetTotalCostOfAllOrdersInAWeekForAStore(int week, int year, string storeCode)
        {
            try
            {
                Stopwatch s = new Stopwatch();
                s.Start();
                double result = 0.00;

                foreach (Order o in Orders.AsParallel().Where(o => o.Date.Week.Equals(week)).Where(o => o.Date.Year.Equals(year)).Where(o => o.Store.StoreCode.Equals(storeCode.ToUpper())).AsSequential()) result += o.Cost;

                s.Stop();
                Console.WriteLine(s.Elapsed.TotalSeconds);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }
        /// <summary>
        /// Get the total cost of all orders for a supplier
        /// </summary>
        /// <param name="supplierName">The name of supplier</param>
        /// <returns>Cost of all orders or throws <c>Exception</c> if an issue arises</returns>
        public double GetTotalCostOfAllOrdersForASupplier(string supplierName)
        {
            try
            {
                Stopwatch s = new Stopwatch();
                s.Start();
                double result = 0.00;

                foreach (var o in Orders.AsParallel().Where(o => o.Supplier.Name.ToLower().Equals(supplierName.ToLower()))) result += o.Cost;

                s.Stop();
                Console.WriteLine(s.Elapsed.TotalSeconds);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }
        /// <summary>
        /// Get the total cost of all orders for a supplier type
        /// </summary>
        /// <param name="supplierType">The type of supplier</param>
        /// <returns>Cost of all orders or throws <c>Exception</c></c> if an issue arises</returns>
        public double GetTotalCostOfAllOrdersForASupplierType(string supplierType)
        {
            try
            {
                Stopwatch s = new Stopwatch();
                s.Start();
                double result = 0.00;

                foreach (var o in Orders.AsParallel().Where(o => o.Supplier.Type.ToLower().Equals(supplierType.ToLower()))) result += o.Cost;

                s.Stop();
                Console.WriteLine(s.Elapsed.TotalSeconds);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }
        /// <summary>
        /// Get the total cost of all orders for a supplier type in a week and year
        /// </summary>
        /// <param name="supplierType">The type of supplier</param>
        /// <param name="week">Week number between 1 - 52</param>
        /// <param name="year">Year number E.G. 2014</param>
        /// <returns>Cost of all orders or throws <c>Exception</c> if an issue arises</returns>
        public double GetTotalCostOfAllOrdersForASupplierTypeInAWeek(string supplierType, int week, int year)
        {
            try
            {
                Stopwatch s = new Stopwatch();
                s.Start();
                double result = 0.00;

                foreach (var o in Orders.AsParallel().Where(o=>o.Date.Week == week).Where(o=>o.Date.Year == year).Where(o => o.Supplier.Type.ToLower() == supplierType.ToLower())) result += o.Cost;

                s.Stop();
                Console.WriteLine(s.Elapsed.TotalSeconds);

                return result;
            }
            catch (Exception e)
            {
                throw new Exception("Unable to complete that task", e);
            }
        }

        /// <summary>
        /// Reload data from files via <c><see cref="FolderPath"/></c> property
        /// </summary>
        /// <param name="cts">Used to escape the loading process</param>
        /// <returns>Custom <c>IOException</c> if issue found, else <c>null</c></returns>
        public IOException ReloadData(CancellationTokenSource cts) => LoadData(cts).Result;

        private Task<IOException> LoadData(CancellationTokenSource cts)
        {
            IOException e = null;

            return Task<IOException>.Factory.StartNew(() =>
            {
                try
                {
                    Stores = new ConcurrentDictionary<string, Store>();
                    Dates = new ConcurrentStack<Date>();
                    Orders = new ConcurrentBag<Order>();
                    Suppliers = new ConcurrentDictionary<string, Supplier>();

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

                        if (!Stores.ContainsKey(storeDataSplit[0]))
                            Stores.TryAdd(storeDataSplit[0], new Store(storeDataSplit[0], storeDataSplit[1]));
                    }

                    var t1 = Task<IOException>.Factory.StartNew(() =>
                    {
                        try
                        {
                            string[] fileNames = Directory.GetFiles(FolderPath + @"\" + StoreDataFolder);
                            //foreach (var filePath in fileNames)
                            //fileNames.AsParallel().ForAll(filePath =>
                            Parallel.ForEach(fileNames,/*new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount/2 } ,*/ filePath =>
                            {
                                string fileNameExt = Path.GetFileName(filePath);
                                string fileName = Path.GetFileNameWithoutExtension(filePath);

                                string[] fileNameSplit = fileName.Split('_');
                                Store store = Stores[fileNameSplit[0]];
                                Date date = new Date(Convert.ToInt32(fileNameSplit[1]), Convert.ToInt32(fileNameSplit[2]));
                                Dates.Push(date);

                                string[] orderData = File.ReadAllLines(FolderPath + @"\" + StoreDataFolder + @"\" + fileNameExt);
                                //orderData.AsParallel().ForAll(orderInfo =>
                                //Parallel.ForEach(orderData, orderInfo =>
                                foreach (var orderInfo in orderData)
                                {
                                    string[] orderSplit = orderInfo.Split(',');

                                    Supplier supplier = Suppliers.GetOrAdd(orderSplit[0], new Supplier(orderSplit[0], orderSplit[1]));
                                    Order o = new Order(store, date, supplier, Convert.ToDouble(orderSplit[2]));
                                    Orders.Add(o);
                                }//);
                            });
                            return null;
                        }
                        catch (Exception ex)
                        {
                            return GetFriendlyIOException(ex);
                        }
                    });
                    t1.Wait();
                    e = t1.Result;

                    stopWatch.Stop();
                    Console.WriteLine("TimeToLoad: " + stopWatch.Elapsed.TotalSeconds); // For testing purposes
                }
                catch (Exception ex)
                {
                    e = GetFriendlyIOException(ex);
                }

                return e;
            }, cts.Token);
        }

        private IOException GetFriendlyIOException(Exception e)
        {
            IOException result;

            try
            {
                throw e;
            }
            catch (DirectoryNotFoundException ex)
            {
                result = new IOException("Unable to locate directory '" + FolderPath + "'", ex);
            }
            catch (FileNotFoundException ex)
            {
                result = new IOException("Unable to locate StoreCodes.csv at '" + FolderPath + "'", ex);
            }
            catch (PathTooLongException ex)
            {
                result = new IOException("File path '" + FolderPath + "' is too long", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                result = new IOException("Unable to access the directory and/or files", ex);
            }
            catch (NotSupportedException ex)
            {
                result = new IOException("The path '" + FolderPath + "' is not supported", ex);
            }
            catch (Exception ex)
            {
                result = new IOException("Error loading store data", ex);
            }

            return result;
        }
    }
}
