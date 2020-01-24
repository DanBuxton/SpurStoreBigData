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
        private static Dictionary<int, string> Items { get; set; } = new Dictionary<int, string>();

        static void Main(string[] args)
        {
            SetupConsole();

            Menu();
        }

        private static void ChangeDataFolder()
        {
            // F:\BSc Software Engineering\Year 2\Task-based Software Engineering\Data
            Console.Write("Data Folder path > ");
            string path = Console.ReadLine();

            while (path == "")
            {
                Console.Write("Data Folder path > ");
                path = Console.ReadLine();
            }

            FolderPath = path;
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
                            ReloadData();

                            foreach (var store in Stores.OrderBy(s => s.Key))
                            {
                                Console.WriteLine(store.Value);
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
            foreach (var item in Items)
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

            Items.Add(1, "List all stores");
            Items.Add(Items.Keys.Last() + 1, "List all suppliers");
            Items.Add(Items.Keys.Last() + 1, "List all supplier types\n");

            Items.Add(Items.Keys.Last() + 1, "Total cost of all orders");
            Items.Add(Items.Keys.Last() + 1, "Total cost of all orders for a store");
            Items.Add(Items.Keys.Last() + 1, "Total cost of all orders in a week");
            Items.Add(Items.Keys.Last() + 1, "Total cost of all orders in a week for a store");
            Items.Add(Items.Keys.Last() + 1, "Total cost of all orders to a supplier");
            Items.Add(Items.Keys.Last() + 1, "Cost of all orders from a supplier type");
            Items.Add(Items.Keys.Last() + 1, "Cost of all orders in a week from a supplier type");
            Items.Add(Items.Keys.Last() + 1, "Cost of all orders for a supplier type for a store");
            Items.Add(Items.Keys.Last() + 1, "Cost of all orders in a week for a supplier type for a store\n");

            // Graphs
            //Items.Add(Items.Keys.Last() + 1, "Cost of all orders in a week from a supplier type");
            //Items.Add(Items.Keys.Last() + 1, "Cost of all orders for a supplier type for a store");
            //Items.Add(Items.Keys.Last() + 1, "Cost of all orders in a week for a supplier type for a store\n");

            Items.Add(Items.Keys.Last() + 1, "Change data path");
            Items.Add(Items.Keys.Last() + 1, "Reload all data\n");
            Items.Add(Items.Keys.Last() + 1, "Exit");
        }
    }
}
