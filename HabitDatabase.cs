using Microsoft.Data.Sqlite;

/*
 *
 * All functions used to operate on the database are here.
 *
 * The database is stored in "habit-logger.sqlite" file by default.
 *
 * Every habit has its own table in it.
 *
 */

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
        List<string> queries = ["CREATE TABLE IF NOT EXISTS habits (Id INTEGER PRIMARY KEY, Name TEXT, Unit TEXT)"];

        queries.Add($"INSERT INTO habits (Name, Unit) VALUES (\"{name}\", \"{unit}\")");

        // Create habit table
        queries.Add($"CREATE TABLE IF NOT EXISTS \"{name}\" (Id INTEGER PRIMARY KEY, Amount REAL, Time TEXT)");

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

    // Just for testing purposes
    public static void CreateSampleData()
    {
        Create("Water drunk", "litres (l)");
        for (int i = 0; i < 1000; i++)
        {
            double randomNumber = new Random().NextDouble() * 5;
            InsertData("Water drunk", randomNumber);
        }
    }
}