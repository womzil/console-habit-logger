using Spectre.Console;

namespace ConsoleHabitLogger;

public class Validation
{
    public static int SelectId(bool habit)
    {
        string habitOrActivity = habit ? "habit" : "activity";

        int id = 0;
        string input = AnsiConsole.Ask<string>($"Please, enter {habitOrActivity}'s ID: ");

        while (!int.TryParse(input, out id) || habit ? !Database.Operations.HabitExists(id) : !Database.Operations.ActivityExists(id))
        {
            AnsiConsole.MarkupLine("[red]Wrong ID![/]");
            input = AnsiConsole.Ask<string>($"Please, enter {habitOrActivity}'s ID: ");
        }

        return id;
    }
}