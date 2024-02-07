namespace habit_logger;

internal static class Program
{
    static void Main()
    {
        Console.WriteLine("Welcome to the habit logger created by womzil.");
        Console.WriteLine("Please, note that VS Code debug terminal is not supported.");
        Console.WriteLine();
        Console.WriteLine("You can use this program to log any habit/activity of choice.");
        Console.WriteLine("");

        HabitDatabase.CreateSampleData();
    }
}