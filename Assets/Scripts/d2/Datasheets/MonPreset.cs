using System.Collections.Generic;

[System.Serializable]
public class MonPreset
{
    const int ActCount = 5;

    public int act;
    public string place;

    public static Datasheet<MonPreset> sheet = Datasheet<MonPreset>.Load("data/global/excel/MonPreset.txt");
    static List<MonPreset>[] presets = new List<MonPreset>[ActCount + 1];

    static MonPreset()
    {
        for(int act = 0; act < presets.Length; ++act)
        {
            presets[act] = new List<MonPreset>();
        }

        for(int i = 0; i < sheet.rows.Count; ++i)
        {
            MonPreset preset = sheet.rows[i];
            presets[preset.act].Add(preset);
        }
    }

    public static string Find(int act, int id)
    {
        var actPresets = presets[act];
        if (id < actPresets.Count)
        {
            return actPresets[id].place;
        }
        return null;
    }
}
