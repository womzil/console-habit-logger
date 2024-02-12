namespace habit_logger
{
    internal class Menu
    {
        public static void MainMenu()
        {
            bool exit = false;
            int numberOfOptions = 4;
            int i = 0;

            Console.CursorVisible = false;

            while (!exit)
            {
                string[] x = new string[numberOfOptions];
                Array.Fill(x, " ");
                x[i] = "X";

                Utils.ConsoleClear();

                Console.WriteLine("Welcome to the habit logger created by womzil.");
                Console.WriteLine("Please, note that VS Code debug terminal is not supported.");
                Console.WriteLine();
                Console.WriteLine("You can use this program to log any habit/activity of choice.");
                Console.WriteLine("");
                Console.WriteLine("Please, choose what you want to do from the list below.");
                Console.WriteLine($"({x[0]}) Habit browser & editor");
                Console.WriteLine($"({x[1]}) Create a new habit");
                Console.WriteLine($"({x[2]}) Create sample data");
                Console.WriteLine($"({x[3]}) Exit");

                ConsoleKey key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        i--;
                        if (i < 0)
                            i = numberOfOptions - 1;
                        continue;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        i++;
                        if (i >= numberOfOptions)
                            i = 0;
                        continue;
                    case ConsoleKey.Enter:
                        break;
                    default:
                        continue;
                }

                switch (i)
                {
                    case 0:
                        Editor("habits");
                        break;

                    case 1:
                        AddHabit();
                        break;

                    case 2:
                        CreateSampleData();
                        break;

                    case 3:
                        Console.WriteLine("Are you sure you want to leave?");
                        Console.WriteLine("Press \"Enter\" to confirm or any other key to cancel.");

                        key = Console.ReadKey(true).Key;
                        if (key == ConsoleKey.Enter)
                            exit = true;

                        break;
                }
            }
        }

        public static void HabitsEditor()
        {
            Utils.ConsoleClear();

            List<int> ids = HabitDatabase.ReadAll();

            int numberOfOptions = Console.CursorTop - 3 < 0 ? Console.CursorTop - 1 : Console.CursorTop - 3;
            int option = numberOfOptions;
            bool exit = false;

            if (ids.Count != 0)
            {
                if (option > ids.Count)
                    option = 1;

                Console.WriteLine();
                Console.WriteLine("Escape - Exit, A - Add a habit, R - Remove chosen habit, E - Edit chosen habit, Enter - View habit");

                Console.SetCursorPosition(1, option);
                Console.Write("X");
            }
            else
            {
                Console.WriteLine("Please, create your first habit first.");
                AddHabit();
                exit = true;
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

                    case ConsoleKey.Enter:
                        string readName = HabitDatabase.ReadById(ids[option]);
                        HabitDataEditor(readName);
                        break;

                    case ConsoleKey.A:
                    case ConsoleKey.Add:
                        AddHabit();
                        Utils.PressAnyKeyToContinue();
                        exit = true;
                        break;

                    case ConsoleKey.E:
                        Utils.ConsoleClear();
                        Console.WriteLine("Not implemented yet!");
                        Utils.PressAnyKeyToContinue();
                        exit = true;
                        break;

                    case ConsoleKey.R:
                        Utils.ConsoleClear();
                        Console.WriteLine("Are you sure? Press \"Enter\" to confirm or any other key to cancel.");

                        string removeName = HabitDatabase.ReadById(ids[option]);
                        if (Console.ReadKey(true).Key == ConsoleKey.Enter) HabitDatabase.RemoveHabit(removeName);
                        exit = true;
                        break;

                    case ConsoleKey.Escape:
                    case ConsoleKey.Backspace:
                        exit = true;
                        break;
                }
            }
        }

        public static void HabitDataEditor(string habit)
        {
            Utils.ConsoleClear();

            long page = 1;
            List<int> ids = HabitDatabase.ReadAll(habit, page);

            int maxNumberOfLinesOnScreen = Console.WindowHeight;
            long maxPages = HabitDatabase.NumberOfRows(habit) / (maxNumberOfLinesOnScreen - 5);

            if (maxPages < 1)
                maxPages = 1;

            int numberOfOptions = Console.CursorTop - 5 < 0 ? Console.CursorTop - 3 : Console.CursorTop - 5;
            int option = numberOfOptions;
            bool exit = false;

            if (ids.Count == 0)
            {
                Console.WriteLine($"Please, create your first record for habit {habit} first.");
                AddHabitRecord(habit);
                exit = true;
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"Page {page}/{maxPages}");
                Console.WriteLine();
                Console.WriteLine("Escape - Exit, A - Add a record, R - Remove chosen record, E - Edit chosen record");
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
                        ids = HabitDatabase.ReadAll(habit, page);
                        break;

                    case ConsoleKey.RightArrow:
                        if (page + 1 > maxPages)
                            page = 1;
                        else
                            page++;

                        Utils.ConsoleClear();
                        ids = HabitDatabase.ReadAll(habit, page);
                        break;

                    case ConsoleKey.A:
                    case ConsoleKey.Add:
                        AddHabitRecord(habit);
                        exit = true;
                        break;

                    case ConsoleKey.E:
                        Utils.ConsoleClear();
                        Console.WriteLine("Not implemented yet!");
                        Utils.PressAnyKeyToContinue();
                        exit = true;
                        break;

                    case ConsoleKey.R:
                        Utils.ConsoleClear();
                        Console.WriteLine("Are you sure? Press \"Enter\" to confirm or any other key to cancel.");

                        if (Console.ReadKey(true).Key == ConsoleKey.Enter) HabitDatabase.RemoveRecord(habit, ids[option]);
                        exit = true;
                        break;

                    case ConsoleKey.Escape:
                    case ConsoleKey.Backspace:
                        exit = true;
                        break;
                }
            }
        }

        public static void Editor(string table = "habits")
        {
            Utils.ConsoleClear();

            // Whether this function is used to edit habits or record of a specific habit
            bool editorOfHabits = table == "habits";

            long page = 1;
            List<int> ids = HabitDatabase.ReadAll(table, page);

            int maxNumberOfLinesOnScreen = Console.WindowHeight;
            long maxPages = (long)Math.Ceiling((double)HabitDatabase.NumberOfRows(table) / (maxNumberOfLinesOnScreen - 5));

            if (maxPages < 1)
                maxPages = 1;

            int numberOfOptions = ids.Count;
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
                        break;

                    case ConsoleKey.RightArrow:
                        if (page + 1 > maxPages)
                            page = 1;
                        else
                            page++;

                        numberOfOptions = ids.Count;
                        option = numberOfOptions;

                        Utils.ConsoleClear();
                        ids = HabitDatabase.ReadAll(table, page);
                        EditorFooter(option, ids, page, maxPages, editorOfHabits);
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
                        Utils.ConsoleClear();
                        Console.WriteLine("Not implemented yet!");
                        Utils.PressAnyKeyToContinue();
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
                option = ids.Count;

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

            HabitDatabase.Create(habitName, habitUnit);
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

            HabitDatabase.InsertData(habit, amount);
            Utils.PressAnyKeyToContinue();
        }

        public static void CreateSampleData()
        {
            Utils.ConsoleClear();

            Console.WriteLine("Do you really want to create sample data?");
            Console.WriteLine("It will create 1000 random records of a habit named \"Drunk water\", with the unit \"(l) Litres\".");
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