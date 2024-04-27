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

namespace ConsoleHabitLogger.Database.AccessType;

public static class Sqlite
{
    static readonly string ConnectionString = Program.Config["database:connection_string"];

    public static void CreateHabit(string tableName, string unit)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // CreateHabit table of habits
        List<string> queries = ["CREATE TABLE IF NOT EXISTS habits (ID INTEGER PRIMARY KEY, Name TEXT, Unit TEXT)"];

        queries.Add($"INSERT INTO habits (Name, Unit) SELECT '{tableName}', '{unit}' WHERE NOT EXISTS (SELECT 1 FROM sqlite_master WHERE type='table' AND name='{tableName}')");

        // CreateHabit habit table
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

    public static void Edit(int id, string newTableName, string unit)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        List<string> queries = [];

        queries.Add($"UPDATE habits SET Name = '{newTableName}', Unit = '{unit}' WHERE ID = {id}");
        queries.Add($"ALTER TABLE '{ReadById(id)}' RENAME TO '{newTableName}'");

        for (int i = 0; i < queries.Count; i++)
        {
            using (SqliteCommand command = new SqliteCommand(queries[i], connection))
            {
                command.ExecuteNonQuery();
                Console.WriteLine(i == 0 ? "Table of habits edited successfully." : $"Habit table \"{newTableName}\" with unit \"{unit}\" edited successfully.");
            }
        }

        connection.Close();
    }

    public static void RemoveHabit(string tableName)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(ConnectionString);
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

        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        string removeFromHabitList = $"DELETE FROM '{tableName}' WHERE ID='{id}'";

        using (SqliteCommand command = new SqliteCommand(removeFromHabitList, connection))
        {
            command.ExecuteNonQuery();
        }

        Console.WriteLine($"Removed record with ID {id} from habit {tableName}.");
        connection.Close();
    }

    public static void CreateRecord(string name, double amount)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        string query = $"INSERT INTO '{name}' (Amount, Time) VALUES ('{amount}', datetime())";

        using (SqliteCommand command = new SqliteCommand(query, connection))
        {
            command.ExecuteNonQuery();
            Console.WriteLine($"Added new entry with value of '{amount}' to the habit '{name}' successfully.");
        }

        connection.Close();
    }

    public static void EditRecord(int id, string tableName, double amount)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(ConnectionString);
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

        using SqliteConnection connection = new SqliteConnection(ConnectionString);
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

        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        int columnCount;
        List<int> ids = new List<int>();
        int maxNumberOfLinesOnScreen = Console.WindowHeight - 5;

        string query = $"SELECT * FROM '{tableName}' LIMIT {maxNumberOfLinesOnScreen} OFFSET {maxNumberOfLinesOnScreen * (page - 1)}";
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

        using SqliteConnection connection = new SqliteConnection(ConnectionString);
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

    // Just for testing purposes
    public static void CreateSampleData()
    {
        CreateHabit("Water drunk", "litres (l)");
        for (int i = 0; i < 1000; i++)
        {
            double randomNumber = new Random().NextDouble() * 5;
            Console.Write($"{i + 1}. ");
            CreateRecord("Water drunk", randomNumber);
        }
    }
}