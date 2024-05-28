using Spectre.Console;

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
            AccessType.Sqlite.ExecuteQueries([$"DELETE FROM activity_records WHERE id = {id}"]);
    }
    
    public static void EditHabit(int id, string name, string unit)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            AccessType.Sqlite.ExecuteQueries([
                $"UPDATE habits SET name = '{name}', unit = '{unit}' WHERE id = {id}"
            ]);
    }

    public static void EditActivity(int id, string amount, string description = "")
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            AccessType.Sqlite.ExecuteQueries([
                $"UPDATE activity_records SET amount = '{amount}', description = '{description}' WHERE id = {id}"
            ]);
    }

    public static List<List<string>> ReadHabits(int numberOfHabits, int startIndex)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            return AccessType.Sqlite.ExecuteQueriesWithReturn([
                $"SELECT * FROM habits LIMIT {numberOfHabits} OFFSET {startIndex}"
            ], 3);
        else
            return [];
    }

    public static List<List<string>> ReadActivities(int habitId, int numberOfActivities, int startIndex)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            return AccessType.Sqlite.ExecuteQueriesWithReturn([
                $"SELECT id, amount, description, time_created FROM activity_records WHERE habit_id IS {habitId} LIMIT {numberOfActivities} OFFSET {startIndex}"
            ], 4);
        else
            return [];
    }

    public static string GetHabitName(int id)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            return AccessType.Sqlite.ExecuteQueriesWithReturn([
                $"SELECT name FROM habits WHERE id IS {id}"
            ], 1)[0][0];
        else
            return "0";
    }

    public static string GetAmountOfRows(int habitId = -1)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            if (habitId >= 0)
            {
                return AccessType.Sqlite.ExecuteQueriesWithReturn([
                    $"SELECT COUNT(id) FROM activity_records WHERE habit_id IS {habitId}"
                ], 1)[0][0];
            }
            else
            {
                return AccessType.Sqlite.ExecuteQueriesWithReturn([
                    $"SELECT COUNT(id) FROM habits"
                ], 1)[0][0];
            }
        else
            return "0";
    }

    public static bool HabitExists(int id)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
        {
            List<List<string>> returns = AccessType.Sqlite.ExecuteQueriesWithReturn([$"SELECT id FROM habits WHERE id IS {id}"], 1);

            return returns.Count == 1;
        }
        else
            return false;
    }

    public static bool ActivityExists(int id)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
        {
            List<List<string>> returns = AccessType.Sqlite.ExecuteQueriesWithReturn([$"SELECT id FROM activity_records WHERE id IS {id}"], 1);

            return returns.Count == 1;
        }
        else
            return false;
    }

    public static bool UsesTimeSelector(int id)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
        {
            List<List<string>> returns = AccessType.Sqlite.ExecuteQueriesWithReturn([$"SELECT unit FROM habits WHERE id IS {id}"], 1);

            return returns[0][0] == "time";
        }
        else
            return false;
    }

    public static void GenerateSampleData()
    {
        Random random = new Random();
        string habitName = $"Cycling #{random.Next()}";
        CreateHabit(habitName, "kilometers");
        int habitId = int.Parse(AccessType.Sqlite.ExecuteQueriesWithReturn([$"SELECT id FROM habits WHERE name IS '{habitName}'"], 1)[0][0]);

        AnsiConsole.Progress()
            .Start(ctx => 
            {
                ProgressTask task = ctx.AddTask("[green]Generating sample data...[/]");

                for (int i = 1; i <= 1000; i++)
                {
                    task.Increment(0.1);
                    CreateActivity(habitId, random.Next(2, 200).ToString(), $"Description #{i}");
                }

                if (!ctx.IsFinished)
                {
                    task.Increment(100);
                }
            });

    }
}