/*
 *
 * All functions used to operate on the database are here.
 *
 * The database is stored in "habit-logger.sqlite" file by default.
 *
 * Every habit has its own table in it.
 *
 */

using Microsoft.Data.Sqlite;

namespace habit_logger;

public static class HabitDatabase
{
    static string _connectionString = "Data Source=habit-logger.sqlite;";

    public static void Create(string name, string unit)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Create table of habits
        List<string> queries = ["CREATE TABLE IF NOT EXISTS habits (ID INTEGER PRIMARY KEY, Name TEXT, Unit TEXT)"];

        queries.Add($"INSERT INTO habits (Name, Unit) VALUES (\"{name}\", \"{unit}\")");

        // Create habit table
        queries.Add($"CREATE TABLE IF NOT EXISTS \"{name}\" (ID INTEGER PRIMARY KEY, Amount REAL, Time TEXT)");

        for (int i = 0; i < queries.Count; i++)
        {
            using (SqliteCommand command = new SqliteCommand(queries[i], connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine(i == 0 ? "Table of habits created successfully." : $"Habit table \"{name}\" created successfully.");
            }
        }

        connection.Close();
    }

    public static void InsertData(string name, double amount)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Create table of habits
        string query = $"INSERT INTO \"{name}\" (Amount, Time) VALUES (\"{amount}\", datetime())";

            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine($"Added new entry with value of \"{amount}\" to the habit \"{name}\" successfully.");
            }

        connection.Close();
    }

    public static void Read()
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        string query = "SELECT Id, Name, Unit FROM habits;";

        using (SqliteCommand command = new SqliteCommand(query, connection))
        {
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int id = reader.GetInt32(0); // Assuming the first column is the Id column
                        string name = reader.GetString(1); // Assuming the second column is the Name column
                        string unit = reader.GetString(2);

                        Console.WriteLine($"( ) ID: {id} | Name: {name} | Unit: {unit}");
                    }
                }
                else
                {
                    Console.WriteLine("Error: No rows found.");
                }
            }
        }

        connection.Close();
    }

    // Just for testing purposes
    public static void CreateSampleData()
    {
        Create("Water drunk", "litres (l)");
        for (int i = 0; i < 1000; i++)
        {
            double randomNumber = new Random().NextDouble() * 5;
            Console.Write($"{i+1}. ");
            InsertData("Water drunk", randomNumber);
        }
    }
}