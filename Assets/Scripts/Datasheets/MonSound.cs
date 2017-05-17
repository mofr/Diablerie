using System.Collections.Generic;

[System.Serializable]
public class MonSound
{
    public static List<MonSound> sheet = Datasheet.Load<MonSound>("data/global/excel/MonSounds.txt");
    static Dictionary<string, MonSound> map = new Dictionary<string, MonSound>();

    public static MonSound Find(string id)
    {
        if (id == null)
            return null;

        return map.GetValueOrDefault(id);
    }

    static MonSound()
    {
        foreach(var sound in sheet)
        {
            if (sound.id == null)
                continue;

            sound.death = SoundInfo.Find(sound._deathSound);
            sound.hit = SoundInfo.Find(sound._hitSound);
            sound.attack1 = SoundInfo.Find(sound._attack1);
            sound.attack2 = SoundInfo.Find(sound._attack2);
            sound.weapon1 = SoundInfo.Find(sound._weapon1);
            sound.weapon2 = SoundInfo.Find(sound._weapon2);
            map.Add(sound.id, sound);
        }
    }
    
    public string id;
    public string _attack1;
    public string _weapon1;
    public int attack1Delay;
    public int weapon1Delay;
    public int attack1Prob;
    public int weapon1Volume;
    public string _attack2;
    public string _weapon2;
    public int attack2Delay;
    public int weapon2Delay;
    public int attack2Prob;
    public int weapon2Volume;
    public string _hitSound;
    public string _deathSound;
    public string _hitDelay;
    public string _deathDelay;
    public string _skill1;
    public string _skill2;
    public string _skill3;
    public string _skill4;
    public string _footstep;
    public string _footstepLayer;
    public string _fsCnt;
    public string _fsOff;
    public string _fsPrb;
    public string _neutral;
    public string _neuTime;
    public string _init;
    public string _taunt;
    public string _flee;
    public string _cvtMo1;
    public string _cvtSk1;
    public string _cvtTgt1;
    public string _cvtMo2;
    public string _cvtSk2;
    public string _cvtTgt2;
    public string _cvtMo3;
    public string _cvtSk3;
    public string _cvtTgt3;
    public string eol;

    [System.NonSerialized]
    public SoundInfo attack1;

    [System.NonSerialized]
    public SoundInfo attack2;

    [System.NonSerialized]
    public SoundInfo weapon1;

    [System.NonSerialized]
    public SoundInfo weapon2;

    [System.NonSerialized]
    public SoundInfo hit;

    [System.NonSerialized]
    public SoundInfo death;
}
