namespace ConsoleHabitLogger.Database;

public class Operations
{
    public static readonly string ConnectionString = Program.Config["database:connection_string"];
    public static readonly string TypeOfAccess = Program.Config["database:type"];

    public static void CreateHabit(string name, string unit)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            AccessType.Sqlite.ExecuteQueries([
                // Create habits table if it doesn't exist
                "CREATE TABLE IF NOT EXISTS habits (id INTEGER PRIMARY KEY, name TEXT, unit TEXT)",
                // Add new habit
                $"INSERT INTO habits (name, unit) SELECT '{name}', '{unit}'"
            ]);
    }

    public static void CreateActivity(int habitId, string amount, string description = "")
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            AccessType.Sqlite.ExecuteQueries([
                // Create a table for activity records if it doesn't exist
                "CREATE TABLE IF NOT EXISTS activity_records (id INTEGER PRIMARY KEY, habit_id INTEGER, amount TEXT, description TEXT, time_created TEXT)",
                // Add new record
                $"INSERT INTO activity_records (habit_id, amount, description, time_created) VALUES ({habitId}, '{amount}', '{description}', datetime())"
            ]);
    }

    public static void RemoveHabit(int id)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            AccessType.Sqlite.ExecuteQueries([$"DELETE FROM habits WHERE id = {id}",
            $"DELETE FROM activity_records WHERE habit_id = {id}"]);
    }

    public static void RemoveActivity(int id)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            AccessType.Sqlite.ExecuteQueries([$"DELETE FROM habits WHERE id = {id}"]);
    }

    public static List<List<string>> ReadHabits(int numberOfHabits, int startIndex)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            return AccessType.Sqlite.ExecuteQueriesWithReturn([
                $"SELECT * FROM habits LIMIT {numberOfHabits} OFFSET {startIndex}"
            ]);
        else
            return [];
    }

    public static List<List<string>> ReadActivities(int numberOfHabits, int startIndex)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            return AccessType.Sqlite.ExecuteQueriesWithReturn([
                $"SELECT * FROM activity_records LIMIT {numberOfHabits} OFFSET {startIndex}"
            ]);
        else
            return [];
    }

    public static string GetAmountOfRows(int habitId)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            return AccessType.Sqlite.ExecuteQueriesWithReturn([
                $"SELECT COUNT(id) FROM activity_records WHERE habit_id IS {habitId}"
            ], 1)[0][0];
        else
            return "0";
    }

    public static bool HabitExists(int id)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
        {
            List<List<string>> returns = AccessType.Sqlite.ExecuteQueriesWithReturn([$"SELECT id FROM habits WHERE id IS {id}"], 1);

            return returns.Count == 0;
        }
        else
            return false;
    }

    public static bool ActivityExists(int id)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
        {
            List<List<string>> returns = AccessType.Sqlite.ExecuteQueriesWithReturn([$"SELECT id FROM activity_records WHERE id IS {id}"], 1);

            return returns.Count == 0;
        }
        else
            return false;
    }

    public static void GenerateSampleData()
    {
        Random random = new Random();
        string habitName = $"Cycling #{random.Next()}";
        CreateHabit(habitName, "kilometers");

        for (int i = 0; i <= 1000; i++)
        {
            CreateActivity(1, "15.5", $"Description #{i}");
        }
    }
}