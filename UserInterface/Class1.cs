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
            try {
                config = Config.DetectConfig ();

            } catch (Exception e) {
                HandleError (e.Message);
            }
            Console.WriteLine (config.dest);
            DisplayList<string> (tables);
            Console.WriteLine ("Put a table name to continue proccessing table");
            DisplayTableAbilities (Convert.ToInt32 (Console.ReadLine ()));
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

        static void DisplayTableAbilities (int number) {
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
                    DisplayDataInTable (tables[number]);
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
                    Console.ReadLine ();
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
                    Console.ReadLine ();
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
                    Console.ReadLine ();
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