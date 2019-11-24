using System.Collections.Generic;

[System.Serializable]
[Datasheet.Record]
public class ExpTable
{
    public static int MaxLevel = 99;
    
    public uint maxLevel;
    public uint amazon;
    public uint sorceress;
    public uint necromancer;
    public uint paladin;
    public uint barbarian;
    public uint druid;
    public uint assassin;
    
    static List<ExpTable> sheet = Datasheet.Load<ExpTable>("data/global/excel/experience.txt", headerLines: 2);

    public static uint GetExperienceRequired(int level)
    {
        level = (level - 1) % MaxLevel;
        return sheet[level].amazon;
    }
}
