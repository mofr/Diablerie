using System.Collections.Generic;

[System.Serializable]
public class Translation
{
    public string key;
    public string value;

    static Dictionary<string, string> map = new Dictionary<string, string>();
    public static Datasheet<Translation> sheet = Datasheet<Translation>.Load("data/local/string.txt", headerLines: 0);

    public static string Find(string key)
    {
        return Find(key, key);
    }

    public static string Find(string key, string defaultValue = null)
    {
        if (key == null)
            return null;
        return map.GetValueOrDefault(key, defaultValue);
    }

    static Translation()
    {
        foreach (var translation in sheet.rows)
        {
            if (!map.ContainsKey(translation.key))
                map.Add(translation.key, translation.value);
        }
    }
}
