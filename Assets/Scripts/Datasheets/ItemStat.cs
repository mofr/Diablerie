using System.Collections.Generic;

[System.Serializable]
[Datasheet.Record]
public class ItemStat
{
    public static List<ItemStat> sheet = Datasheet.Load<ItemStat>("data/global/excel/ItemStatCost.txt");
    static Dictionary<string, ItemStat> map = new Dictionary<string, ItemStat>();

    public static ItemStat Find(string code)
    {
        if (code == null)
            return null;
        return map.GetValueOrDefault(code);
    }

    static ItemStat()
    {
        foreach(var prop in sheet)
        {
            prop.descPositive = Translation.Find(prop.descStrPositive);
            prop.descNegative = Translation.Find(prop.descStrNegative);
            prop.desc2 = Translation.Find(prop.descStr2);
            map.Add(prop.code, prop);
        }
    }

    public string code;
    public int id;
    public int sendOther;
    public bool signed;
    public int sendBits;
    public int sendParamBits;
    public bool updateAnimRate;
    public bool saved;
    public bool csvSigned;
    public int csvBits;
    public int csvParam;
    public bool fCallback;
    public bool fMin;
    public int minAccr;
    public int encode;
    public int add;
    public int multiply;
    public int divide;
    public int valShift;
    public int _1_09_SaveBits;
    public int _1_09_SaveAdd;
    public int saveBits;
    public int saveAdd;
    public int saveParamBits;
    public int keepzero;
    public int op;
    public int opParam;
    public string opBase;
    public string opStat1;
    public string opStat2;
    public string opStat3;
    public bool direct;
    public string maxStat;
    public int itemSpecific;
    public int damageRelated;
    public string itemEvent1;
    public int itemEventFunc1;
    public string itemEvent2;
    public int itemEventFunc2;
    public int descPriority;
    public int descFunc;
    public int descVal;
    public string descStrPositive;
    public string descStrNegative;
    public string descStr2;
    public int dgrp;
    public int dgrpfunc;
    public int dgrpval;
    public string dgrpStrPositive;
    public string dgrpStrNegative;
    public string dgrpStr2;
    public int stuff;
    public string eol;

    [System.NonSerialized]
    public string descPositive;

    [System.NonSerialized]
    public string descNegative;

    [System.NonSerialized]
    public string desc2;
}
