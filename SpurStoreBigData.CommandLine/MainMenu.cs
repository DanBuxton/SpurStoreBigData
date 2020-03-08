using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpurStoreBigData.CommandLine
{
    class MainMenu : AbstractMenu
    {
        CancellationTokenSource cts;

        private Core Core { get; set; } = Core.Instance;

        protected override void Menu()
        {
            SetupConsole();

            ChangeDataFolder();

            Console.ReadKey();

            while (true)
            {
                Console.Clear();

                OutputMenuItems();

                MenuSelection();

                Console.Write("Press any key to continue");
                Console.ReadKey();
            }
        }

        protected override void MenuSelection()
        {
            switch (MenuResponse())
            {
                case 1: // List all stores
                    try
                    {
                        foreach (var store in Core.GetStores())
                        {
                            Console.WriteLine(store);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 2: // List all suppliers (Name)
                    try
                    {
                        foreach (var s in Core.GetSupplierNames())
                        {
                            Console.WriteLine(s);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 3: // List all suppliers (Type)
                    try
                    {
                        foreach (var t in Core.GetSupplierTypes())
                        {
                            Console.WriteLine(t);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 4: // Cost of all orders
                    try
                    {
                        Console.WriteLine("{0:C}", Core.GetTotalCostOfAllOrders());
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException.ToString());
                    }
                    break;
                case 5: // Cost of all orders for a store
                    try
                    {
                        Console.WriteLine("{0:C}", Core.GetTotalCostOfAllOrdersForAStore("Der1"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 6: // Cost of all orders in a week
                    try
                    {
                        Console.WriteLine("{0:C}", Core.GetTotalCostOfAllOrdersInAWeek(1, 2013));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 7: // Cost of all orders in a week for a store
                    try
                    {
                        Console.WriteLine("{0:C}", Core.GetTotalCostOfAllOrdersInAWeekForAStore(1, 2013, "deR1"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException.Message);
                    }
                    break;
                case 8: // Cost of all oders to a supplier
                    try
                    {
                        Console.WriteLine("{0:C}", Core.GetTotalCostOfAllOrdersForASupplier("Blue Diamond"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 9: // Cost of all orders from a supplier type
                    try
                    {
                        Console.WriteLine("{0:C}", Core.GetTotalCostOfAllOrdersForASupplierType("Groceries"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 10: // Cost of all orders in a week from a supplier type
                    try
                    {
                        Console.WriteLine("{0:C}", Core.GetTotalCostOfAllOrdersForASupplierTypeInAWeek("Groceries", 1, 2013));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 11: // Cost of all orders for a suplier type for a store
                    try
                    {
                        Console.WriteLine("{0:C}", Core.GetTotalCostOfAllOrdersForASupplierForAStore("Blue Diamond", "DER1"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 12: // Cost of all orders in a week for a supplier type for a store
                    try
                    {
                        Console.WriteLine("{0:C}", Core.GetTotalCostOfAllOrdersInAWeekForASupplierForAStore(1, 2013, "Blue Diamond", "DER1"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 13: // Change data path
                    ChangeDataFolder();
                    break;
                case 14: // Reload data
                    TryLoadData();
                    break;
                case 15: // Exit
                    Environment.Exit(0);
                    break;
                default:
                    break;
            }
        }

        private void ChangeDataFolder()
        {
            // F:\BSc Software Engineering\Year 2\Task-based Software Engineering\Data
            // C:\temp\Data

            do
            {
                Console.Write("Data Folder path > ");
                string path = Console.ReadLine();

                Core.FolderPath = path;

            } while (!TryLoadData());
        }

        private bool TryLoadData()
        {
            cts = new CancellationTokenSource();

            bool result = false;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            try
            {
                IOException e = Core.ReloadData(cts);

                if (e != null) throw e;

                result = true;
            }
            catch (IOException e)
            {
                HandleException(e);
            }

            stopWatch.Stop();
            Console.WriteLine("TimeToLoad: " + stopWatch.Elapsed.TotalSeconds); // For testing purposes

            return result;
        }

        private void HandleException(Exception e)
        {
            Console.WriteLine(e.Message);
        }

        protected override int MenuResponse()
        {
            int result = -1;

            while (!MenuItems.Keys.Contains(result))
            {
                try
                {
                    Console.Write("> ");
                    result = Convert.ToInt32(Console.ReadLine());

                    if (!MenuItems.Keys.Contains(result))
                    {
                        Console.WriteLine("Must be between 1 and {0}", MenuItems.Count);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Must be a whole number");

                    result = -1;
                }
            }

            return result;
        }

        private void OutputMenuItems()
        {
            foreach (var item in MenuItems)
            {
                if (item.Value == null)
                {
                    Console.WriteLine();
                    continue;
                }
                Console.WriteLine("{0, -2:d} - {1:s}", item.Key, item.Value);
            }
        }

        private void SetupConsole()
        {
            Console.Title = Core.Title;

            MenuItems.Add(1, "List all stores");
            MenuItems.Add(MenuItems.Keys.Last() + 1, "List all suppliers");
            MenuItems.Add(MenuItems.Keys.Last() + 1, "List all supplier types\n");

            MenuItems.Add(MenuItems.Keys.Last() + 1, "Total cost of all orders");
            MenuItems.Add(MenuItems.Keys.Last() + 1, "Total cost of all orders for a store");
            MenuItems.Add(MenuItems.Keys.Last() + 1, "Total cost of all orders in a week");
            MenuItems.Add(MenuItems.Keys.Last() + 1, "Total cost of all orders in a week for a store");
            MenuItems.Add(MenuItems.Keys.Last() + 1, "Total cost of all orders to a supplier\n");
            MenuItems.Add(MenuItems.Keys.Last() + 1, "Cost of all orders from a supplier type");
            MenuItems.Add(MenuItems.Keys.Last() + 1, "Cost of all orders in a week from a supplier type");
            MenuItems.Add(MenuItems.Keys.Last() + 1, "Cost of all orders for a supplier type for a store");
            MenuItems.Add(MenuItems.Keys.Last() + 1, "Cost of all orders in a week for a supplier type for a store\n");

            // Graphs
            //MenuItems.Add(MenuItems.Keys.Last() + 1, "Cost of all orders in a week from a supplier type");
            //MenuItems.Add(MenuItems.Keys.Last() + 1, "Cost of all orders for a supplier type for a store");
            //MenuItems.Add(MenuItems.Keys.Last() + 1, "Cost of all orders in a week for a supplier type for a store\n");

            MenuItems.Add(MenuItems.Keys.Last() + 1, "Change data path");
            MenuItems.Add(MenuItems.Keys.Last() + 1, "Reload all data\n");
            MenuItems.Add(MenuItems.Keys.Last() + 1, "Exit");
        }
    }
}
