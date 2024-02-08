namespace habit_logger
{
    internal class Menu
    {
        public static void MainMenu()
        {
            bool exit = false;
            int numberOfOptions = 6;
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
                Console.WriteLine($"({x[0]}) View list of habits");
                Console.WriteLine($"({x[1]}) View data of a specific habit");
                Console.WriteLine($"({x[2]}) Create a new habit");
                Console.WriteLine($"({x[3]}) Add a new record to a habit");
                Console.WriteLine($"({x[4]}) Create sample data");
                Console.WriteLine($"({x[5]}) Exit");

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
                        HabitsEditor();
                        break;

                    case 4:
                        CreateSampleData();
                        break;

                    case 5:
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

            HabitDatabase.ReadAll();
            Console.WriteLine();
            Console.WriteLine("Escape - Exit, A - Add a habit, R - Remove chosen habit, E - Edit chosen habit, Enter - View habit");

            int linesUnderOptions = 3;
            int numberOfOptions = Console.CursorTop - linesUnderOptions;
            int option = numberOfOptions;
            int newLines = 0;
            bool exit = false;

            Console.SetCursorPosition(1, option);
            Console.Write("X");

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
                        HabitDataEditor("Water drunk");
                        break;

                    case ConsoleKey.A:
                    case ConsoleKey.Add:
                        newLines++;
                        string? habitName;
                        string? habitUnit;

                        do
                        {
                            Console.SetCursorPosition(0, numberOfOptions + linesUnderOptions + newLines);
                            Console.Write("Please, enter habit name: ");
                            habitName = Console.ReadLine();

                            if (string.IsNullOrEmpty(habitName))
                            {
                                Console.WriteLine("Incorrect habit name!");
                                newLines++;
                            }
                        } while (string.IsNullOrEmpty(habitName));

                        newLines++;
                        do
                        {
                            Console.SetCursorPosition(0, numberOfOptions + linesUnderOptions + newLines);
                            Console.Write("Please, enter habit unit: ");
                            habitUnit = Console.ReadLine();

                            if (string.IsNullOrEmpty(habitUnit))
                            {
                                Console.WriteLine("Incorrect habit unit!");
                                newLines++;
                            }
                        } while (string.IsNullOrEmpty(habitUnit));

                        HabitDatabase.Create(habitName, habitUnit);
                        break;

                    case ConsoleKey.R:
                        newLines++;
                        Console.SetCursorPosition(0, numberOfOptions + linesUnderOptions + newLines);

                        Console.WriteLine("Are you sure? Press \"Enter\" to confirm or any other key to cancel.");
                        if (Console.ReadKey(true).Key == ConsoleKey.Enter) HabitDatabase.Remove("Water drunk");
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

            HabitDatabase.ReadAll(habit);
            Console.WriteLine();
            Console.WriteLine("Escape - Exit, A - Add a record, R - Remove chosen record, E - Edit chosen record");

            int linesUnderOptions = 3;
            int numberOfOptions = Console.CursorTop - linesUnderOptions;
            int option = numberOfOptions;
            bool exit = false;

            Console.SetCursorPosition(1, option);
            Console.Write("X");

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

                    case ConsoleKey.A:
                    case ConsoleKey.Add:
                        bool correctData;
                        double amount;
                        int newLines = 1;

                        do
                        {
                            Console.SetCursorPosition(0, numberOfOptions + linesUnderOptions + newLines);
                            Console.Write("Please, enter the amount: ");
                            correctData = double.TryParse(Console.ReadLine(), out amount);

                            if (!correctData)
                            {
                                Console.WriteLine("Incorrect number!");
                                newLines++;
                            }
                        } while (!correctData);


                        // HabitDatabase.InsertData(option, amount);
                        break;

                    case ConsoleKey.Escape:
                    case ConsoleKey.Backspace:
                        exit = true;
                        break;
                }
            }
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
