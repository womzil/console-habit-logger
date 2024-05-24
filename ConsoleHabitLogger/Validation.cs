using Spectre.Console;

namespace ConsoleHabitLogger;

public class Validation
{
    public static int SelectId(bool habit)
    {
        string habitOrActivity = habit ? "habit" : "activity";

        int habitId = int.Parse(AnsiConsole.Ask<string>($"Please, enter {habitOrActivity}'s ID: "));
        do
        {
            AnsiConsole.MarkupLine("[red]Wrong ID![/]");
            habitId = int.Parse(AnsiConsole.Ask<string>($"Please, enter {habitOrActivity}'s ID: "));
        } while (Database.Operations.HabitExists(habitId));

        return habitId;
    }
}