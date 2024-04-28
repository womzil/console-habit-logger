namespace ConsoleHabitLogger.Database;

public class Operations
{
    public static readonly string ConnectionString = Program.Config["database:connection_string"];
    public static readonly string TypeOfAccess = Program.Config["database:type"];

    public static void CreateHabit(string name, string unit)
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            AccessType.Sqlite.CreateHabit(name, unit);
    }
    
    public static void CreateRecord(int habitId, string amount, string description = "")
    {
        if (TypeOfAccess.ToLower() == "sqlite")
            AccessType.Sqlite.CreateRecord(habitId, amount, description);
    }

    public static void GenerateSampleData()
    {
        Random random = new Random();
        CreateHabit($"Cycling #{random.Next()}", "kilometers");
        CreateRecord(1, "15.5", "test description");
    }
}