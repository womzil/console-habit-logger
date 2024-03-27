using System.Globalization;

namespace ConsoleHabitLogger;

internal static class Program
{
    static void Main()
    {
        CultureInfo ci = CultureInfo.InstalledUICulture;
        string languageCode = ci.TwoLetterISOLanguageName;

        Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(languageCode);
        Localization.Initiate(languageCode);

        while (true)
            Menu.MainMenu();
    }
}