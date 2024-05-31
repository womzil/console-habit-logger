using Spectre.Console;

namespace ConsoleHabitLogger;

public class Validation
{
    public static int SelectId(bool habit)
    {
        string habitOrActivity = habit ? Localization.GetString("habit") : Localization.GetString("activity");

        int id = 0;
        string input = AnsiConsole.Ask<string>(string.Format(Localization.GetString("enter_id"), habitOrActivity));

        while (!int.TryParse(input, out id) || habit ? !Database.Operations.HabitExists(id) : !Database.Operations.ActivityExists(id))
        {
            AnsiConsole.MarkupLine(Localization.GetString("wrong_id"));
            input = AnsiConsole.Ask<string>(string.Format(Localization.GetString("enter_id"), habitOrActivity));
        }

        return id;
    }
}