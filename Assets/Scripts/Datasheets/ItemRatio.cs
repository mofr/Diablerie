using System.Collections.Generic;

[System.Serializable]
[Datasheet.Record]
public class ItemRatio
{
    public static List<ItemRatio> sheet = Datasheet.Load<ItemRatio>("data/global/excel/ItemRatio.txt");

    static ItemRatio()
    {
        sheet.RemoveAll(ratio => ratio.version == 0);
    }

    public string function;
    public int version;
    public bool uber;
    public bool classSpecific;
    public int unique;
    public int uniqueDivisor;
    public int uniqueMin;
    public int rare;
    public int rareDivisor;
    public int rareMin;
    public int set;
    public int setDivisor;
    public int setMin;
    public int magic;
    public int magicDivisor;
    public int magicMin;
    public int hiQuality;
    public int hiQualityDivisor;
    public int normal;
    public int normalDivisor;
}
