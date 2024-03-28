
using Microsoft.Extensions.Configuration;

namespace ConsoleHabitLogger;

public class Program
{
    public static IConfiguration Config = new ConfigurationBuilder()
        .AddXmlFile("config.xml")
        .Build();

    static void Main()
    {
        Localization.Initiate();

        if (string.IsNullOrEmpty(Config["database:connection_string"]))
            throw new InvalidDataException("Connection type cannot be empty. Please, check the application config.");

        while (true)
            Menu.MainMenu();
    }
}