using YamlDotNet.Serialization;

namespace habit_logger;

static class Localization
{
    private static Dictionary<string, string> localizedStrings;

    public static void Initiate(string languageCode)
    {
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
        if (localizedStrings.ContainsKey(key))
            return localizedStrings[key];
        else
            // Return the key itself if the translation is not found
            return key;
    }
}