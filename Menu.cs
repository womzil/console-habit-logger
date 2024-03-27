using Spectre.Console;

namespace habit_logger
{
    internal class Menu
    {
        public static void MainMenu()
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
                    .AddChoices([
                        "Editor", "Create habit", "Create sample data", "Settings", "Exit"
                    ]));

            switch (option)
            {
                case "Editor":
                    Editor("habits");
                    break;

                case "Create habit":
                    AddHabit();
                    break;

                case "Create sample data":
                    CreateSampleData();
                    break;

                case "Settings":
                    Settings();
                    break;

                case "Exit":
                    if (AnsiConsole.Confirm("Are you sure you want to leave?"))
                        AnsiConsole.MarkupLine("Leaving...");
                    else MainMenu();
                    break;
            }
        }

        public static void Settings()
        {
            Utils.ConsoleClear();
            Console.WriteLine("To be added.");
            Utils.PressAnyKeyToContinue();
        }

        public static void Editor(string table = "habits")
        {
            Utils.ConsoleClear();

            // Whether this function is used to edit habits or record of a specific habit
            bool editorOfHabits = table == "habits";

            long page = 1;
            List<int> ids = HabitDatabase.ReadAll(table, page);

            int maxNumberOfLinesOnScreen = Console.WindowHeight;
            long maxPages =
                (long)Math.Ceiling((double)HabitDatabase.NumberOfRows(table) / (maxNumberOfLinesOnScreen - 5));

            if (maxPages < 1)
                maxPages = 1;

            int numberOfOptions = ids.Count - 1;
            int option = numberOfOptions;
            bool exit = false;

            if (ids.Count == 0)
            {
                Console.WriteLine($"Please, create your first record for habit {table} first.");
                AddHabitRecord(table);
                exit = true;
            }
            else
            {
                EditorFooter(option, ids, page, maxPages, editorOfHabits);
            }

            while (!exit)
            {
                ConsoleKey key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        Console.SetCursorPosition(0, option);
                        Console.Write("( )");
                        option--;
                        if (option < 0)
                            option = numberOfOptions;

                        Console.SetCursorPosition(0, option);
                        Console.Write("(X)");
                        continue;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        Console.SetCursorPosition(0, option);
                        Console.Write("( )");
                        option++;
                        if (option > numberOfOptions)
                            option = 0;

                        Console.SetCursorPosition(0, option);
                        Console.Write("(X)");
                        continue;

                    case ConsoleKey.LeftArrow:
                        if (page - 1 > 0)
                            page--;
                        else
                            page = maxPages;

                        Utils.ConsoleClear();
                        ids = HabitDatabase.ReadAll(table, page);
                        EditorFooter(option, ids, page, maxPages, editorOfHabits);

                        numberOfOptions = ids.Count - 1;
                        option = numberOfOptions;
                        break;

                    case ConsoleKey.RightArrow:
                        if (page + 1 > maxPages)
                            page = 1;
                        else
                            page++;

                        Utils.ConsoleClear();
                        ids = HabitDatabase.ReadAll(table, page);
                        EditorFooter(option, ids, page, maxPages, editorOfHabits);

                        numberOfOptions = ids.Count - 1;
                        option = numberOfOptions;
                        break;

                    case ConsoleKey.Enter:
                        if (editorOfHabits)
                        {
                            string readName = HabitDatabase.ReadById(ids[option]);
                            Editor(readName);
                        }

                        break;

                    case ConsoleKey.A:
                    case ConsoleKey.Add:
                        if (editorOfHabits)
                            AddHabit();
                        else
                            AddHabitRecord(table);

                        exit = true;
                        break;

                    case ConsoleKey.E:
                        if (editorOfHabits)
                            EditHabit(ids[option]);
                        else
                            EditHabitRecord(ids[option], table);

                        exit = true;
                        break;

                    case ConsoleKey.R:
                        Utils.ConsoleClear();
                        Console.WriteLine("Are you sure? Press \"Enter\" to confirm or any other key to cancel.");

                        if (Console.ReadKey(true).Key == ConsoleKey.Enter)
                        {
                            if (editorOfHabits)
                            {
                                string removeName = HabitDatabase.ReadById(ids[option]);
                                HabitDatabase.RemoveHabit(removeName);
                            }
                            else
                                HabitDatabase.RemoveRecord(table, ids[option]);
                        }

                        exit = true;
                        break;

                    case ConsoleKey.Escape:
                    case ConsoleKey.Backspace:
                        exit = true;
                        break;
                }
            }
        }

        public static void EditorFooter(int option, List<int> ids, long page, long maxPages, bool editorOfHabits)
        {
            if (option > ids.Count)
                option = ids.Count - 1;

            Console.WriteLine();
            Console.WriteLine($"Page {page}/{maxPages}");
            Console.WriteLine();
            Console.WriteLine(editorOfHabits
                ? "Escape - Exit, A - Add a habit, R - Remove chosen habit, E - Edit chosen habit, Enter - View habit"
                : "Escape - Exit, A - Add a record, R - Remove chosen record, E - Edit chosen record");

            Console.SetCursorPosition(1, option);
            Console.Write("X");
        }

        public static void AddHabit()
        {
            string? habitName;
            string? habitUnit;

            Utils.ConsoleClear();

            do
            {
                Console.Write("Please, enter habit name: ");
                habitName = Console.ReadLine();

                if (string.IsNullOrEmpty(habitName))
                {
                    Console.WriteLine("Incorrect habit name!");
                }
            } while (string.IsNullOrEmpty(habitName));

            do
            {
                Console.Write("Please, enter habit unit: ");
                habitUnit = Console.ReadLine();

                if (string.IsNullOrEmpty(habitUnit))
                {
                    Console.WriteLine("Incorrect habit unit!");
                }
            } while (string.IsNullOrEmpty(habitUnit));

            HabitDatabase.CreateHabit(habitName, habitUnit);
            Utils.PressAnyKeyToContinue();
        }

        public static void AddHabitRecord(string habit)
        {
            bool correctData;
            double amount;

            Utils.ConsoleClear();

            do
            {
                Console.Write("Please, enter the amount: ");
                correctData = double.TryParse(Console.ReadLine(), out amount);

                if (!correctData)
                {
                    Console.WriteLine("Incorrect number!");
                }
            } while (!correctData);

            HabitDatabase.CreateRecord(habit, amount);
            Utils.PressAnyKeyToContinue();
        }

        public static void EditHabit(int id)
        {
            string? habitName;
            string? habitUnit;

            Utils.ConsoleClear();

            do
            {
                Console.Write("Please, enter habit name: ");
                habitName = Console.ReadLine();

                if (string.IsNullOrEmpty(habitName))
                {
                    Console.WriteLine("Incorrect habit name!");
                }
            } while (string.IsNullOrEmpty(habitName));

            do
            {
                Console.Write("Please, enter habit unit: ");
                habitUnit = Console.ReadLine();

                if (string.IsNullOrEmpty(habitUnit))
                {
                    Console.WriteLine("Incorrect habit unit!");
                }
            } while (string.IsNullOrEmpty(habitUnit));

            HabitDatabase.Edit(id, habitName, habitUnit);
            Utils.PressAnyKeyToContinue();
        }

        public static void EditHabitRecord(int id, string habit)
        {
            bool correctData;
            double amount;

            Utils.ConsoleClear();

            do
            {
                Console.Write("Please, enter the amount: ");
                correctData = double.TryParse(Console.ReadLine(), out amount);

                if (!correctData)
                {
                    Console.WriteLine("Incorrect number!");
                }
            } while (!correctData);

            HabitDatabase.EditRecord(id, habit, amount);
            Utils.PressAnyKeyToContinue();
        }

        public static void CreateSampleData()
        {
            Utils.ConsoleClear();

            Console.WriteLine("Do you really want to create sample data?");
            Console.WriteLine(
                "It will create 1000 random records of a habit named \"Drunk water\", with the unit \"(l) Litres\".");
            Console.WriteLine("Press \"Enter\" to confirm or any other key to cancel.");

            ConsoleKey key = Console.ReadKey().Key;
            if (key == ConsoleKey.Enter)
            {
                Utils.ConsoleClear();
                HabitDatabase.CreateSampleData();
                Console.WriteLine();
                Console.WriteLine("Press any key to return to the main menu.");
                Console.ReadKey();
            }
        }
    }
}