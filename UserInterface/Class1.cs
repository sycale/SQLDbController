using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ConfigManager;
using DbUtils;

namespace UserUtils {
    public class UserInterface {

        static dynamic config;

        static List<string> tables = DataManager.GetAllTables ();

        static void PrintDbSize () {
            Console.WriteLine (tables.Count);
        }

        static void UpdateTables () {
            tables = DataManager.GetAllTables ();
        }

        static void HandleError (string text) {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine (text);
            Console.ForegroundColor = ConsoleColor.Gray;
            DataManager.AddLog (text);
        }

        static void DisplayList<T> (List<T> listToDisplay) {
            if (!Convert.ToBoolean (listToDisplay.Count - 1)) {
                throw new Exception ($"table {listToDisplay[0]} is empty");
            } else {
                foreach (var val in listToDisplay.Select ((value, index) => new { value, index })) {
                    Console.WriteLine ($"{val.index}. {val.value}");
                }
            }
        }

        static void DisplayTables () {
            DisplayList<string> (tables);
        }
        public static void DisplayMainMenu () {
            Console.Clear ();
            UpdateTables ();
            try {
                config = Config.DetectConfig ();

            } catch (Exception e) {
                HandleError (e.Message);
            }
            mainMenu:
                Console.WriteLine ("0. See existing tables");
            Console.WriteLine ("1. Create a new table");
            Console.WriteLine ("2. Quit the program");
            int command = Convert.ToInt32 (Console.ReadLine ());
            switch (command) {
                case 0:
                    DisplayList<string> (tables);
                    Console.WriteLine ($"{tables.Count}. Return");
                    int choice = Convert.ToInt32 (Console.ReadLine ());
                    if (choice == tables.Count) {
                        DisplayMainMenu ();
                    } else {
                        DisplayTableAbilities (choice);
                    }
                    break;
                case 1:
                    HandleCreateTable ();
                    break;
                case 2:
                    System.Environment.Exit (1);
                    break;
                default:
                    Console.WriteLine ("you've picked a wrong number, try again");
                    goto mainMenu;

            }
        }

        static void handleListToXml (string table) {
            var branchesXml = DataManager.GetTableValues (table).Select (i => new XElement ("row",
                new XAttribute ("value", i)));
            var bodyXml = new XElement (table, branchesXml);
            File.WriteAllText (Path.Combine (config.dest, "file.xml"), bodyXml.ToString ());
        }

        static void DisplayDataInTable (string table) {
            try {
                List<string> tempList = DataManager.GetTableValues (table);
                DisplayList<string> (tempList);
            } catch (System.Data.SqlClient.SqlException e) {
                HandleError (e.Message);
            } catch (Exception e) {
                HandleError (e.Message);
            }
        }

        static void HandleCreateTable () {
            Console.WriteLine ("Let me know the table name");
            string tableName = Console.ReadLine ();
            try {
                DataManager.CreateTable (tableName);
            } catch (SqlException e) {
                HandleError (e.Message);
            }
            DisplayMainMenu ();
        }

        static void DisplayTableAbilities (int number) {
            Console.Clear ();
            if (number > tables.Count - 1 || number < 0) throw new Exception ($"your index {number} is out of range {tables.Count - 1}");
            ChooseStamp:
                Console.Clear ();
            Console.WriteLine ($"You are expecting abilities for a {tables[number]} table");
            Console.WriteLine ("1. Check content inside this table");
            Console.WriteLine ("2. Insert Value");
            Console.WriteLine ("3. Delete value");
            Console.WriteLine ("4. Convert content into xml doc");
            Console.WriteLine ("5. Return to a main menu");
            int choice = Convert.ToInt32 (Console.ReadLine ());

            switch (choice) {
                case 1:
                    try {
                        DisplayDataInTable (tables[number]);
                    } catch (Exception e) {
                        HandleError (e.Message);
                    }
                    Console.WriteLine ("Press any key to continue");

                    Console.ReadKey (true);

                    goto ChooseStamp;
                case 2:
                    try {
                        string dataToInsert = Console.ReadLine ();
                        DataManager.InsertValue (tables[number], dataToInsert);
                    } catch (SqlException e) {
                        HandleError (e.Message);
                    } catch (Exception e) {
                        HandleError (e.Message);
                    }
                    Console.WriteLine ("Press any key to continue");

                    Console.ReadKey (true);
                    goto ChooseStamp;
                case 3:
                    try {
                        string dataToInsert = Console.ReadLine ();
                        DataManager.DeleteValue (tables[number], dataToInsert);
                    } catch (SqlException e) {
                        HandleError (e.Message);
                    } catch (Exception e) {
                        HandleError (e.Message);
                    }
                    Console.WriteLine ("Press any key to continue");

                    Console.ReadKey (true);
                    goto ChooseStamp;
                case 4:
                    try {
                        handleListToXml (tables[number]);
                        Console.WriteLine ("Saving xml file into path, that is declared in config");
                    } catch (SqlException e) {
                        HandleError (e.Message);
                    } catch (Exception e) {
                        HandleError (e.Message);
                    }
                    Console.WriteLine ("Press any key to continue");

                    Console.ReadKey (true);
                    goto ChooseStamp;
                case 5:
                    DisplayMainMenu ();
                    break;
                default:
                    Console.WriteLine ("wrong choice, try again");
                    goto ChooseStamp;
            }
        }
    }
}