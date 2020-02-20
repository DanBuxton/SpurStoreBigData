using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using static SpurStoreBigData.Core;

namespace SpurStoreBigData.CommandLine
{
    class Program
    {

        static CancellationTokenSource cts = new CancellationTokenSource();

        public static Core Core { get; set; } = Instance;

        private static Dictionary<int, string> MenuItems { get; set; } = new Dictionary<int, string>();

        static void Main(string[] args)
        {
            SetupConsole();

            ChangeDataFolder();

            Menu();
        }

        private static void Menu()
        {
            while (true)
            {
                Console.Clear();

                OutputMenuItems();

                MenuSelection();

                Console.Write("Press any key to continue");
                Console.ReadKey();
            }
        }

        private static void MenuSelection()
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
                        Console.WriteLine(e.Message);
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
                        Console.WriteLine("{0:C}", Core.GetTotalCostOfAllOrdersInAWeek(1));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    break;
                case 7: // Cost of all orders in a week for a store
                    try
                    {
                        Console.WriteLine("{0:C}", Core.GetTotalCostOfAllOrdersInAWeekForAStore(1, "deR1"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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
                    break;
                case 10: // Cost of all orders in a week from a supplier type
                    break;
                case 11: // Cost of all orders for a suplier type for a store
                    break;
                case 12: // Cost of all orders in a week for a supplier type for a store
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

        private static void ChangeDataFolder()
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

        private static bool TryLoadData()
        {
            cts = new CancellationTokenSource();

            bool result = false;

            try
            {
                Core.ReloadData(cts);

                result = true;
            }
            catch (IOException e)
            {
                HandleException(e);
            }

            return result;
        }

        private static void HandleException(IOException e)
        {
            Console.WriteLine(e.Message);
        }

        private static int MenuResponse()
        {
            int result = -1;

            while (result < 1)
            {
                try
                {
                    Console.Write("> ");
                    result = Convert.ToInt32(Console.ReadLine());

                    if (result > MenuItems.Count)
                    {
                        Console.WriteLine("Must be between 1 and {0}", MenuItems.Count);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Too big or small");

                    result = -1;
                }
            }

            return result;
        }

        private static void OutputMenuItems()
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

        private static void SetupConsole()
        {
            Console.Title = "Spur Ltd Big Data";

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
