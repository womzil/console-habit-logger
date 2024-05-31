using Spectre.Console;

namespace ConsoleHabitLogger.Menu;

public class Settings
{
    public static void Open()
    {
        string language = Program.Config["locale:language"]!;
        string databaseEngine = Program.Config["database:type"]!;

        string[] languages = ["en", "es", "pl"];
        string[] databaseEngines = ["SQLite", "Dapper ORM"];

        int languageIndex = 0;
        int databaseEngineIndex = 0;

        Dictionary<string, string> languageCodeToString = new Dictionary<string, string>
        {
            { "en", "English" },
            { "es", "Spanish" },
            { "pl", "Polish" }
        };

        string[] highlightedText = ["[green]", "[/]"];
        int selector = 0;
        string[,] toHighlight = new string[2, 2];

        toHighlight[selector, 0] = highlightedText[0];
        toHighlight[selector, 1] = highlightedText[1];

        while (true)
        {
            Utils.ConsoleClear();
            AnsiConsole.MarkupLine($"Language: {toHighlight[0, 0]}{languageCodeToString[language!]}{toHighlight[0, 1]}");
            AnsiConsole.MarkupLine($"Database engine: {toHighlight[1, 0]}{databaseEngine}{toHighlight[1, 1]}");
            
            ConsoleKey key = AnsiConsole.Console.Input.ReadKey(true)!.Value.Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    toHighlight[selector, 0] = "";
                    toHighlight[selector, 1] = "";
                    selector = selector > 0 ? selector - 1 : toHighlight.GetLength(0) - 1;
                    toHighlight[selector, 0] = highlightedText[0];
                    toHighlight[selector, 1] = highlightedText[1];
                    break;
                case ConsoleKey.DownArrow:
                    toHighlight[selector, 0] = "";
                    toHighlight[selector, 1] = "";
                    selector = selector < toHighlight.GetLength(0) - 1 ? selector + 1 : 0;
                    toHighlight[selector, 0] = highlightedText[0];
                    toHighlight[selector, 1] = highlightedText[1];
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.Spacebar:
                case ConsoleKey.Enter:
                    switch (selector)
                    {
                        case 0:
                            languageIndex = languageIndex + 1 > languages.Length - 1 ? 0 : languageIndex + 1;
                            language = languages[languageIndex];
                            break;
                        case 1:
                            databaseEngineIndex = databaseEngineIndex + 1 > databaseEngines.Length - 1 ? 0 : databaseEngineIndex + 1;
                            databaseEngine = databaseEngines[databaseEngineIndex];
                            break;
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    switch (selector)
                    {
                        case 0:
                            languageIndex = languageIndex - 1 < 0 ? languages.Length - 1 : languageIndex - 1;
                            language = languages[languageIndex];
                            break;
                        case 1:
                            databaseEngineIndex = databaseEngineIndex - 1 < 0 ? databaseEngines.Length - 1 : databaseEngineIndex - 1;
                            databaseEngine = databaseEngines[databaseEngineIndex];
                            break;
                    }
                    break;
                case ConsoleKey.Escape:
                    if (AnsiConsole.Confirm("Are you sure you want to save the changes and exit?"))
                    {
                        AnsiConsole.WriteLine("Leaving...");
                        Program.Config["locale:language"] = language;
                        Program.Config["database:type"] = databaseEngine;
                        Program.SaveConfiguration();
                        Localization.Initiate();
                        return;
                    }
                    break;
            }
        }
    }
}