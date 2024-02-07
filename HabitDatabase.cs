using Microsoft.Data.Sqlite;

/*
 * All functions used to operate on the database are here.
 *
 * The database is stored in "habit-logger.sqlite" file by default.
 *
 * Every habit has its own table in it.
 */

namespace habit_logger;

public static class HabitDatabase
{
    static string _connectionString = "Data Source=habit-logger.sqlite;";

    public static void Create()
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(_connectionString);
        connection.Open();

        string createTableSql = "CREATE TABLE IF NOT EXISTS habits (Id INTEGER PRIMARY KEY, Name TEXT)";
        using (SqliteCommand command = new SqliteCommand(createTableSql, connection))
        {
            command.ExecuteNonQuery();
            Console.WriteLine("Table created successfully.");
        }

        connection.Close();
    }
}