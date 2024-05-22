using System.Runtime.ExceptionServices;
using Microsoft.Data.Sqlite;

namespace ConsoleHabitLogger.Database.AccessType;

public static class Sqlite
{
    private static void ExecuteQueries(string[] queries)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(Operations.ConnectionString);
        connection.Open();

        foreach (string query in queries)
        {
            using (SqliteCommand command = new SqliteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        connection.Close();
    }

    private static List<List<string>> ExecuteQueriesWithReturn(string[] queries)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(Operations.ConnectionString);
        connection.Open();

        List<List<string>> rows = [];

        foreach (string query in queries)
        {
            using (SqliteDataReader reader = new SqliteCommand(query, connection).ExecuteReader())
            {
                if (reader.Read())
                {
                    List<string> row = [];

                    for (int i = 0; i < 4; i++)
                    {
                        row[i] = reader.GetString(i);
                    }

                    rows.Add(row);
                }
            }
        }

        connection.Close();

        return rows;
    }

    public static void CreateHabit(string name, string unit)
    {
        ExecuteQueries([
            // Create habits table if it doesn't exist
            "CREATE TABLE IF NOT EXISTS habits (id INTEGER PRIMARY KEY, name TEXT, unit TEXT)",
            // Add new habit
            $"INSERT INTO habits (name, unit) SELECT '{name}', '{unit}'"
        ]);
    }

    public static void CreateRecord(int habitId, string amount, string description)
    {
        ExecuteQueries([
            // Create a table for activity records if it doesn't exist
            "CREATE TABLE IF NOT EXISTS activity_records (id INTEGER PRIMARY KEY, habit_id INTEGER, amount TEXT, description TEXT, time_created TEXT)",
            // Add new record
            $"INSERT INTO activity_records (habit_id, amount, description, time_created) VALUES ({habitId}, '{amount}', '{description}', datetime())"
        ]);
    }

    public static List<List<string>> ReadHabits(int numberOfHabits, int startIndex)
    {
        return ExecuteQueriesWithReturn([
            $"SELECT * FROM habits LIMIT {numberOfHabits} OFFSET {startIndex}"
        ]);
    }

    public static void Edit(int id, string newTableName, string unit)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(Operations.ConnectionString);
        connection.Open();

        List<string> queries = [];

        queries.Add($"UPDATE habits SET Name = '{newTableName}', Unit = '{unit}' WHERE ID = {id}");
        queries.Add($"ALTER TABLE '{ReadById(id)}' RENAME TO '{newTableName}'");

        for (int i = 0; i < queries.Count; i++)
        {
            using (SqliteCommand command = new SqliteCommand(queries[i], connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine(i == 0
                    ? "Table of habits edited successfully."
                    : $"Habit table \"{newTableName}\" with unit \"{unit}\" edited successfully.");
            }
        }

        connection.Close();
    }

    public static void RemoveHabit(string tableName)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(Operations.ConnectionString);
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

    public static void RemoveRecord(string tableName, int id)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(Operations.ConnectionString);
        connection.Open();

        string removeFromHabitList = $"DELETE FROM '{tableName}' WHERE ID='{id}'";

        using (SqliteCommand command = new SqliteCommand(removeFromHabitList, connection))
        {
            command.ExecuteNonQuery();
        }

        Console.WriteLine($"Removed record with ID {id} from habit {tableName}.");
        connection.Close();
    }

    public static void EditRecord(int id, string tableName, double amount)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(Operations.ConnectionString);
        connection.Open();

        string query = $"UPDATE {tableName} SET Amount = '{amount}' WHERE ID = {id}";

        using (SqliteCommand command = new SqliteCommand(query, connection))
        {
            command.ExecuteNonQuery();
            Console.WriteLine($"Habit record in \"{tableName}\" edited to {amount} successfully.");
        }

        connection.Close();
    }

    public static string ReadById(int tableId)
    {
        SQLitePCL.Batteries.Init();

        string returnString = "";

        using SqliteConnection connection = new SqliteConnection(Operations.ConnectionString);
        connection.Open();

        string readTableName = $"SELECT Name FROM habits WHERE ID = '{tableId}'";

        using (SqliteCommand command = new SqliteCommand(readTableName, connection))
        {
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    returnString = reader.GetString(0);
                }
            }
        }

        connection.Close();
        return returnString;
    }

    public static List<int> ReadAll(string tableName = "habits", long page = 1)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(Operations.ConnectionString);
        connection.Open();

        int columnCount;
        List<int> ids = new List<int>();
        int maxNumberOfLinesOnScreen = Console.WindowHeight - 5;

        string query =
            $"SELECT * FROM '{tableName}' LIMIT {maxNumberOfLinesOnScreen} OFFSET {maxNumberOfLinesOnScreen * (page - 1)}";
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
                        ids.Add(reader.GetInt32(0));

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
                    Console.WriteLine("No data found.");
                }
            }
        }

        connection.Close();
        return ids;
    }

    public static long NumberOfRows(string tableName = "habits")
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(Operations.ConnectionString);
        connection.Open();

        long rowsCount = 0;

        string query = $"SELECT COUNT(*) FROM '{tableName}'";

        using (SqliteCommand command = new SqliteCommand(query, connection))
        {
            // ExecuteScalar returns the first column of the first row in the result set
            // or a null reference if the result set is empty.
            rowsCount = (long)command.ExecuteScalar();
        }

        connection.Close();
        return rowsCount;
    }
}