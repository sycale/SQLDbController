﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DbUtils {
    public static class DataManager {

        static Random rnd = new Random ();

        public static Task<int> AddLog (string Message) {
            return Task.Run (() => {
                using (SqlConnection connection = new SqlConnection (GetConnectionString ())) {
                    connection.Open ();
                    string query = $"USE LAB; INSERT INTO logs (id, message) VALUES ({Message.GetHashCode() + rnd.Next()}, '{Message.Replace("'", "''")}');";
                    SqlCommand command = new SqlCommand(query, connection);
                    return command.ExecuteNonQuery();
                }
            });
        }

        public static string GetConnectionString() {
            return "Data Source=localhost;User ID=sa;Password=QwErTy123!@#;Initial Catalog=master";
        }

        public static Task < List < string >> GetTableValues(string table) {
            return Task.Run(() => {
                List < string > str = new List < string > { table };

                using(SqlConnection connection = new SqlConnection(GetConnectionString())) {
                    connection.Open();
                    String query = $"USE LAB; SELECT * FROM {table};";
                    using(SqlCommand cmnd = new SqlCommand(query, connection)) {
                        using(SqlDataReader reader = cmnd.ExecuteReader()) {
                            while (reader.Read()) {
                                str.Add(reader.GetString(1));
                            }
                        }
                    }

                }
                return str;
            });
        }

        public static Task < int > InsertValue(string table, string value) {
            return Task.Run(() => {
                Console.WriteLine(value);
                using(SqlConnection connection = new SqlConnection(GetConnectionString())) {
                    connection.Open();
                    string query = $"USE LAB; INSERT INTO {table} (id, value) VALUES ({value.GetHashCode () + rnd.Next ()}, '{value.Replace("'", "''")}');";
                    SqlCommand command = new SqlCommand (query, connection);
                    return command.ExecuteNonQuery ();
                }
            });
        }

        public static Task<int> DeleteValue (string table, string value) {
            return Task.Run (() => {
                using (SqlConnection connection = new SqlConnection (GetConnectionString ())) {
                    connection.Open ();
                    string query = $"USE LAB; DELETE FROM {table} WHERE value='{value.Replace("'", "''")}';";
                    SqlCommand command = new SqlCommand(query, connection);
                    return command.ExecuteNonQuery();
                }
            });
        }

        public static void CreateTable(string tableName) {
            using(SqlConnection connection = new SqlConnection(GetConnectionString())) {
                connection.Open();
                string query = $"USE LAB; CREATE TABLE {tableName} (id int primary key not null, value varchar (255));";
                SqlCommand command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
        }

        public static Task < List < string >> GetAllTables() {
            return Task.Run(() => {
                List < string > tables = new List < string > ();

                using(SqlConnection connection = new SqlConnection(GetConnectionString())) {
                    connection.Open();

                    string query = "USE LAB; SELECT name FROM sysobjects WHERE xtype = 'U'";
                    using (SqlCommand cmnd = new SqlCommand (query, connection)) {
                        using (SqlDataReader reader = cmnd.ExecuteReader ()) {
                            while (reader.Read ()) {
                                tables.Add (reader.GetString (0));
                            }
                        }
                    }
                }
                return tables;
            });
        }

    }
}