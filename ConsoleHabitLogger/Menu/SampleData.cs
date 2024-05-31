using Spectre.Console;

namespace ConsoleHabitLogger.Menu;

internal class SampleData
{
    static public void Open()
    {
        if (AnsiConsole.Confirm(Localization.GetString("generate_confirm")))
        {
            Database.Operations.GenerateSampleData();

            AnsiConsole.WriteLine(Localization.GetString("generation_done"));
            Console.ReadKey();
        }
    }
}