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

    public static void Create(string tableName, string unit)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Create table of habits
        List<string> queries = ["CREATE TABLE IF NOT EXISTS habits (ID INTEGER PRIMARY KEY, Name TEXT, Unit TEXT)"];

        queries.Add($"INSERT INTO habits (Name, Unit) SELECT '{tableName}', '{unit}' WHERE NOT EXISTS (SELECT 1 FROM sqlite_master WHERE type='table' AND name='{tableName}')");

        // Create habit table
        queries.Add($"CREATE TABLE IF NOT EXISTS '{tableName}' (ID INTEGER PRIMARY KEY, Amount REAL, Time TEXT)");

        for (int i = 0; i < queries.Count; i++)
        {
            using (SqliteCommand command = new SqliteCommand(queries[i], connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine(i == 0 ? "Table of habits created successfully." : $"Habit table '{tableName}' created successfully.");
            }
        }

        connection.Close();
    }

    public static void Remove(string tableName)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        string removeTable = $"DROP TABLE '{tableName}'";
        string removeFromHabitList = $"DELETE FROM habits WHERE Name='{tableName}'";

        using (SqliteCommand command = new SqliteCommand(removeTable, connection))
        {
            command.ExecuteNonQuery();
        }

        using (SqliteCommand command = new SqliteCommand(removeFromHabitList, connection))
        {
            command.ExecuteNonQuery();
        }

        Console.WriteLine($"Removed habit {tableName}.");
        connection.Close();
    }

    public static void InsertData(string name, double amount)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        // Create table of habits
        string query = $"INSERT INTO '{name}' (Amount, Time) VALUES ('{amount}', datetime())";

        using (SqliteCommand command = new SqliteCommand(query, connection))
        {
            command.ExecuteNonQuery();
            Console.WriteLine($"Added new entry with value of '{amount}' to the habit '{name}' successfully.");
        }

        connection.Close();
    }

    public static void ReadAll(string tableName = "habits")
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        int columnCount;

        string query = $"SELECT * FROM '{tableName}'";
        string columnQuery = $"PRAGMA table_info('{tableName}')";

        using (SqliteCommand schemaCommand = new SqliteCommand(columnQuery, connection))
        {
            using (SqliteDataReader schemaReader = schemaCommand.ExecuteReader())
            {
                columnCount = 0;

                // Iterate through the schema to count columns
                while (schemaReader.Read())
                {
                    columnCount++;
                }
            }
        }

        using (SqliteCommand command = new SqliteCommand(query, connection))
        {
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Console.Write("( ) ");

                        for (int i = 0; i < columnCount; i++)
                        {
                            Console.Write($"{reader.GetName(i)}: {reader.GetString(i)}");
                            if (i + 1 != columnCount)
                                Console.Write(" | ");
                        }

                        Console.WriteLine();
                    }
                }
                else
                {
                    Utils.ConsoleClear();
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
            Console.Write($"{i + 1}. ");
            InsertData("Water drunk", randomNumber);
        }
    }
}