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

                ConsoleKey key = Console.ReadKey().Key;
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

                Utils.ConsoleClear();

                switch (i)
                {
                    case 0:
                        ListOfHabits();
                        break;

                    case 4:
                        CreateSampleData();
                        break;

                    case 5:
                        Console.WriteLine("Are you sure you want to leave?");
                        Console.WriteLine("Press \"Enter\" to confirm or any other key to cancel.");

                        key = Console.ReadKey().Key;
                        if (key == ConsoleKey.Enter)
                            exit = true;

                        break;
                }
            }
        }

        public static void ListOfHabits()
        {
            HabitDatabase.Read();
            Console.WriteLine();
            Console.WriteLine("Press any key to continue.");

            int numberOfOptions = Console.CursorTop - 3;
            int option = numberOfOptions;
            bool exit = false;

            Console.SetCursorPosition(1, option);
            Console.Write("X");

            while (!exit)
            {
                ConsoleKey key = Console.ReadKey().Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        Console.Write("X");
                        option--;
                        if (option < 0)
                            option = numberOfOptions - 1;

                        Console.SetCursorPosition(1, option);
                        continue;
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        option++;
                        if (option >= numberOfOptions)
                            option = 0;

                        Console.SetCursorPosition(1, option);
                        continue;
                    case ConsoleKey.Escape:
                        exit = true;
                        break;
                }
            }
        }

        public static void CreateSampleData()
        {
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
