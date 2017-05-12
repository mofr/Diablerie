using System.Collections.Generic;

[System.Serializable]
public class SkillInfo
{
    public string skill;
    public int id = -1;
    public string charClass;
    public string skillDesc;
    public int srvStartFunc;
    public int srvDoFunc;
    [Datasheet.Sequence(length = 71)]
    public string[] unused;
    public string stsound;
    [Datasheet.Sequence(length = 10)]
    public string[] unused2;
    public string castOverlayId;
    [Datasheet.Sequence(length = 167)]
    public string[] unused3;

    [System.NonSerialized]
    public OverlayInfo castOverlay;

    public static List<SkillInfo> sheet = Datasheet.Load<SkillInfo>("data/global/excel/Skills.txt");
    static Dictionary<string, SkillInfo> map = new Dictionary<string, SkillInfo>();

    static SkillInfo()
    {
        foreach (var row in sheet)
        {
            if (row.id == -1)
                continue;

            row.castOverlay = OverlayInfo.Find(row.castOverlayId);
            map.Add(row.skill, row);
        }
    }

    public static SkillInfo Find(string id)
    {
        return map.GetValueOrDefault(id);
    }

    public void Do(Character character)
    {
        if (srvDoFunc == 27)
        {
            // teleport
            var newPos = IsoInput.mouseTile;
            character.InstantMove(newPos);
        }
    }
}
