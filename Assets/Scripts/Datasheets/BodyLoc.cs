using System.Collections.Generic;

[System.Serializable]
[Datasheet.Record]
public class BodyLoc
{
    public static List<BodyLoc> sheet = Datasheet.Load<BodyLoc>("data/global/excel/bodylocs.txt");

    public static int GetIndex(string code)
    {
        for (int i = 0; i < sheet.Count; ++i)
        {
            if (sheet[i].code == code)
                return i;
        }
        return -1;
    }

    static BodyLoc()
    {
        sheet.RemoveAll(loc => loc.code == null);
    }
    
    public string name;
    public string code;
}
