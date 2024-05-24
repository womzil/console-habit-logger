using Microsoft.Data.Sqlite;

namespace ConsoleHabitLogger.Database.AccessType;

public static class Sqlite
{
    public static void ExecuteQueries(string[] queries)
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

    public static List<List<string>> ExecuteQueriesWithReturn(string[] queries, int numberOfColumns = 3)
    {
        SQLitePCL.Batteries.Init();

        using SqliteConnection connection = new SqliteConnection(Operations.ConnectionString);
        connection.Open();

        List<List<string>> rows = [];

        foreach (string query in queries)
        {
            using (SqliteDataReader reader = new SqliteCommand(query, connection).ExecuteReader())
            {
                while (reader.Read())
                {
                    List<string> row = new List<string>(numberOfColumns);

                    for (int i = 0; i < numberOfColumns; i++)
                    {
                        row.Add(reader.GetString(i));
                    }

                    rows.Add(row);
                }
            }
        }

        connection.Close();

        return rows;
    }
}