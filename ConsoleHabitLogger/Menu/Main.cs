using Spectre.Console;

namespace ConsoleHabitLogger.Menu;

internal static class Main
{
    internal static void Open()
    {
        while (true)
        {
            Console.CursorVisible = false;
            Utils.ConsoleClear();

            AnsiConsole.Write(new Rows(
                new Text(Localization.GetString("welcome_message")),
                new Text(Localization.GetString("warning_vscode")),
                new Text(""),
                new Text("You can use this program to log any habit/activity of choice."),
                new Text("")
            ));

            string option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Please, choose what you want to do from the list below.")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal more options)[/]")
                    .AddChoices(
                        "Editor", "Create sample data", "Settings", "Exit"
                    ));

            switch (option)
            {
                case "Editor":
                    Editor.Open();
                    break;

                case "Create sample data":
                    SampleData.Open();
                    break;

                case "Settings":
                    break;

                case "Exit":
                    if (AnsiConsole.Confirm("Are you sure you want to leave?"))
                    {
                        AnsiConsole.WriteLine("Leaving...");
                        return;
                    }

                    break;
            }
        }
    }
}