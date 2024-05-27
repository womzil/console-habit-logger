using Spectre.Console;

namespace ConsoleHabitLogger.Menu;

internal class SampleData
{
    static public void Open()
    {
        if (AnsiConsole.Confirm("Are you sure you want to create sample data? It can be removed later manually."))
        {
            Database.Operations.GenerateSampleData();

            AnsiConsole.WriteLine("Done! Press any key to return.");
            Console.ReadKey();
        }
    }
}