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
                new Text(Localization.GetString("program_features")),
                new Text("")
            ));

            string editor = Localization.GetString("editor");
            string sampleData = Localization.GetString("sample_data");
            string settings = Localization.GetString("settings");
            string exit = Localization.GetString("exit");

            string option = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(Localization.GetString("selection"))
                    .PageSize(10)
                    .MoreChoicesText($"[grey]({Localization.GetString("more_options")})[/]")
                    .AddChoices(
                        editor, sampleData, settings, exit
                    ));

            if (option == editor)
                Editor.OpenHabitEditor();
            else if (option == sampleData)
                SampleData.Open();
            else if (option == settings)
                Settings.Open();
            else if (option == exit)
                if (AnsiConsole.Confirm(Localization.GetString("exit_confirmation")))
                {
                    AnsiConsole.WriteLine(Localization.GetString("leaving"));
                    return;
                }
        }
    }
}