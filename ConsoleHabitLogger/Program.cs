using System.Xml;
using Microsoft.Extensions.Configuration;

namespace ConsoleHabitLogger;

public class Program
{
    private static readonly string ConfigLocation = "config.xml";

    public static IConfiguration Config = new ConfigurationBuilder()
        .AddXmlFile(ConfigLocation)
        .Build();

    static void Main()
    {
        Localization.Initiate();

        if (string.IsNullOrEmpty(Config["database:connection_string"]))
            throw new InvalidDataException("Connection type cannot be empty. Please, check the application config.");

        try
        {
            Menu.Main.Open();
        }
        catch (Exception error)
        {
            Console.WriteLine(error);
            Console.ReadKey();
        }
    }

    public static void SaveConfiguration()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null));
        XmlElement rootElement = xmlDoc.CreateElement("configuration");
        xmlDoc.AppendChild(rootElement);

        foreach (KeyValuePair<string, string> VARIABLE in Config.AsEnumerable())
        {
            Console.WriteLine("Key = {0}, Value = {1}", VARIABLE.Key, VARIABLE.Value);
        }

        foreach (KeyValuePair<string, string> data in Config.AsEnumerable())
        {
            if (!string.IsNullOrEmpty(data.Value))
            {
                int colonCount = data.Key.ToCharArray().Count(c => c == ':');
                if (colonCount > 0)
                {
                }
                XmlElement element = xmlDoc.CreateElement(data.Key);
                element.InnerText = data.Value;
                rootElement.AppendChild(element);
            }
        }

        xmlDoc.Save(ConfigLocation);

        Console.ReadKey();
    }
}