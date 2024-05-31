using System.Globalization;
using YamlDotNet.Serialization;

namespace ConsoleHabitLogger;

static class Localization
{
    private static Dictionary<string, string> localizedStrings;

    public static void Initiate()
    {
        CultureInfo ci = CultureInfo.InstalledUICulture;
        string languageCode = ci.TwoLetterISOLanguageName;

        if (Program.Config["locale:language"] != null && Program.Config["locale:language"] != "default")
            languageCode = Program.Config["locale:language"]!;

        // Check if a culture with that language code exists
        bool correctLanguageCode = CultureInfo
            .GetCultures(CultureTypes.AllCultures)
            .Any(culture => culture.TwoLetterISOLanguageName == languageCode);

        if (correctLanguageCode)
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(languageCode!);
        else
            languageCode = ci.TwoLetterISOLanguageName;

        string filePath = $"./Locale/{languageCode}.yaml";

        if (File.Exists(filePath))
        {
            string yamlContent = File.ReadAllText(filePath);
            IDeserializer deserializer = new DeserializerBuilder().Build();
            localizedStrings = deserializer.Deserialize<Dictionary<string, string>>(yamlContent);
        }
        else
        {
            // Fallback to English if the language file is missing
            string defaultFilePath = "./Locale/en.yaml";
            string yamlContent = File.ReadAllText(defaultFilePath);
            IDeserializer deserializer = new DeserializerBuilder().Build();
            localizedStrings = deserializer.Deserialize<Dictionary<string, string>>(yamlContent);
        }
    }

    public static string GetString(string key)
    {
        // Return the key itself if the translation is not found
        return localizedStrings.GetValueOrDefault(key, key);
    }
}