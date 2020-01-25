using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static SpurStoreBigData.Core;

namespace SpurStoreBigData.CommandLine
{
    class Program
    {
        private static Dictionary<int, string> MenuItems { get; set; } = new Dictionary<int, string>();

        static void Main(string[] args)
        {
            SetupConsole();

            Menu();
        }

        private static void Menu()
        {
            ChangeDataFolder();

            while (true)
            {
                Console.Clear();

                OutputMenuItems();

                switch (MenuResponse())
                {
                    case 1:
                        try
                        {
                            foreach (var store in GetStores())
                            {
                                Console.WriteLine(store);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 2:
                        break;
                    case 3:
                        break;
                    case 4:
                        try
                        {
                            Console.WriteLine("{0:C}", GetTotalCostOfAllOrders());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 9:
                        break;
                    case 10:
                        break;
                    case 11:
                        break;
                    case 12:
                        break;
                    case 13:
                        ChangeDataFolder();
                        break;
                    case 14:
                        TryLoadData();
                        break;
                    case 15:
                        Environment.Exit(0);
                        break;
                    default:
                        break;
                }

                Console.Write("Press any key to continue");
                Console.ReadKey();
            }
        }

        private static void ChangeDataFolder()
        {
            // F:\BSc Software Engineering\Year 2\Task-based Software Engineering\Data

            do
            {
                Console.Write("Data Folder path > ");
                string path = Console.ReadLine();

                FolderPath = path;

            } while (!TryLoadData());
        }

        private static bool TryLoadData()
        {
            bool result = false;

            try
            {
                ReloadData();
                result = true;
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }

            return result;
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
