using System.Collections.Generic;

[System.Serializable]
public class StateInfo
{
    public static List<StateInfo> sheet = Datasheet.Load<StateInfo>("data/global/excel/States.txt");
    private static Dictionary<string, StateInfo> byCode = new Dictionary<string, StateInfo>();

    static StateInfo()
    {
        foreach(var row in sheet)
        {
            byCode.Add(row.code, row);
        }
    }

    public static StateInfo FindByCode(string code)
    {
        return byCode.GetValueOrDefault(code);
    }

    public string code;
    public int id;
    public int group = -1;
    public bool remhit = false;
    public bool nosend = false;
    public bool transform = false;
    public bool aura = false;
    public bool curable = false;
    public bool curse = false;
    public bool active = false;
    public bool immed = false;
    public bool restrict = false;
    public bool disguise = false;
    public bool blue = false;
    public bool attblue = false;
    public bool damblue = false;
    public bool armblue = false;
    public bool rfblue = false;
    public bool rlblue = false;
    public bool rcblue = false;
    public bool stambarblue = false;
    public bool rpblue = false;
    public bool attred = false;
    public bool damred = false;
    public bool armred = false;
    public bool rfred = false;
    public bool rlred = false;
    public bool rcred = false;
    public bool rpred = false;
    public bool exp = false;
    public bool plrstaydeath = false;
    public bool monstaydeath = false;
    public bool bossstaydeath = false;
    public bool hide = false;
    public bool shatter = false;
    public bool udead = false;
    public bool life = false;
    public bool green = false;
    public bool pgsv = false;
    public bool nooverlays = false;
    public bool noclear = false;
    public bool bossinv = false;
    public bool meleeonly = false;
    public bool notondead = false;
    public string overlay1;
    public string overlay2;
    public string overlay3;
    public string overlay4;
    public string pgsvoverlay;
    public string castoverlay;
    public string removerlay;
    public string stat;
    public int setfunc;
    public int remfunc;
    public string missile;
    public string skill;
    public string itemtype;
    public string itemtrans;
    public int colorpri;
    public int colorshift;
    public int lightr;
    public int lightg;	
    public int lightb;
    public string onsound;
    public string offsound;
    public int gfxtype;
    public int gfxclass;
    public string cltevent;
    public int clteventfunc;
    public int cltactivefunc;
    public int srvactivefunc;
    public string eol;
}
