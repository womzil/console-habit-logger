using System.Globalization;

namespace habit_logger;

internal static class Program
{
    static void Main()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
        Localization.Initiate("en");

        Menu.MainMenu();
    }
}