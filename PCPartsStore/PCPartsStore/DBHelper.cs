using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace Helpers
{
    public static class DBHelper
    {
#if DEBUG
        private static string defaultConnectionString = "Server=localhost;Database=your_database;User ID=your_user;Password=your_password;";
#else
        private static string defaultConnectionString = "Your_Release_Mode_Connection_String_Here";
#endif

        public static string DefaultConnectionString { get { return defaultConnectionString; } }

        public static DataTable ExecuteProcedure(string procName, params object[] parameters)
        {
            if (parameters.Length % 2 != 0)
                throw new ArgumentException("Wrong number of parameters sent to procedure. Expected an even number.");

            DataTable result = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(defaultConnectionString))
            {
                using (MySqlCommand command = new MySqlCommand(procName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    for (int i = 0; i < parameters.Length; i += 2)
                    {
                        command.Parameters.AddWithValue(parameters[i] as string, parameters[i + 1]);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(result);
                    }
                }
            }
            return result;
        }

        public static DataTable ExecuteQuery(string query, params object[] parameters)
        {
            if (parameters.Length % 2 != 0)
                throw new ArgumentException("Wrong number of parameters sent to query. Expected an even number.");

            DataTable result = new DataTable();
            using (MySqlConnection connection = new MySqlConnection(defaultConnectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    for (int i = 0; i < parameters.Length; i += 2)
                    {
                        command.Parameters.AddWithValue(parameters[i] as string, parameters[i + 1]);
                    }

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(result);
                    }
                }
            }
            return result;
        }

        public static int ExecuteNonQuery(string query, params object[] parameters)
        {
            if (parameters.Length % 2 != 0)
                throw new ArgumentException("Wrong number of parameters sent to query. Expected an even number.");

            using (MySqlConnection connection = new MySqlConnection(defaultConnectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    for (int i = 0; i < parameters.Length; i += 2)
                    {
                        command.Parameters.AddWithValue(parameters[i] as string, parameters[i + 1]);
                    }

                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string query, params object[] parameters)
        {
            if (parameters.Length % 2 != 0)
                throw new ArgumentException("Wrong number of parameters sent to query. Expected an even number.");

            using (MySqlConnection connection = new MySqlConnection(defaultConnectionString))
            {
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    for (int i = 0; i < parameters.Length; i += 2)
                    {
                        command.Parameters.AddWithValue(parameters[i] as string, parameters[i + 1]);
                    }

                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
        }
    }
}
