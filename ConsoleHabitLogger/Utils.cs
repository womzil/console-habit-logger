namespace ConsoleHabitLogger;

static class Utils
{
    // Safe alternative to use instead of Console.Clear() to fix problems with Visual Studio Code debugger.
    public static void ConsoleClear()
    {
        try
        {
            Console.Clear();
        }
        catch (IOException)
        {
            Console.WriteLine("IOException - you can ignore it if you're using Visual Studio Code debugger.\n");
        }
    }

    public static Spectre.Console.Color GetRandomColor()
    {
        byte[] randomBytes = new byte[3];
        Random random = new Random();
        random.NextBytes(randomBytes);
        return new Spectre.Console.Color(randomBytes[0], randomBytes[1], randomBytes[2]);
    }

    public static int NumberOfPages(int habitId)
    {
        int numberOfPages = (int)Math.Ceiling(double.Parse(Database.Operations.GetAmountOfRows(habitId)) / 10);

        if (numberOfPages <= 0) numberOfPages = 1;

        return numberOfPages;
    }
}