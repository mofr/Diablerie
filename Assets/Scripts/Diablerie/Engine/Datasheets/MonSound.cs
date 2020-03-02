using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.LibraryExtensions;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class MonSound
    {
        public static List<MonSound> sheet;
        static Dictionary<string, MonSound> map;

        public static MonSound Find(string id)
        {
            if (id == null)
                return null;

            return map.GetValueOrDefault(id);
        }

        public static void Load()
        {
            sheet = Datasheet.Load<MonSound>("data/global/excel/MonSounds.txt");
            map = new Dictionary<string, MonSound>();
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
                sound.taunt = SoundInfo.Find(sound._taunt);
                sound.init = SoundInfo.Find(sound._init);
                sound.neutral = SoundInfo.Find(sound._neutral);
                sound.footstep = SoundInfo.Find(sound._footstep);
                sound.footstepLayer = SoundInfo.Find(sound._footstepLayer);
                sound.attack1Delay = sound._attack1Delay / 25f;
                sound.attack2Delay = sound._attack2Delay / 25f;
                sound.weapon1Delay = sound._weapon1Delay / 25f;
                sound.weapon2Delay = sound._weapon2Delay / 25f;
                sound.weapon1Volume = sound._weapon1Volume / 255f;
                sound.weapon2Volume = sound._weapon2Volume / 255f;
                sound.hitDelay = sound._hitDelay / 25f;
                sound.deathDelay = sound._deathDelay / 25f;
                sound.neutralDelay = sound._neuTime / 25f;
                map.Add(sound.id, sound);
            }
        }
    
        public string id;
        public string _attack1;
        public string _weapon1;
        public int _attack1Delay;
        public int _weapon1Delay;
        public int attack1Prob;
        public int _weapon1Volume;
        public string _attack2;
        public string _weapon2;
        public int _attack2Delay;
        public int _weapon2Delay;
        public int attack2Prob;
        public int _weapon2Volume;
        public string _hitSound;
        public string _deathSound;
        public int _hitDelay;
        public int _deathDelay;
        public string _skill1;
        public string _skill2;
        public string _skill3;
        public string _skill4;
        public string _footstep;
        public string _footstepLayer;
        public int _fsCnt;
        public int _fsOff;
        public int _fsPrb;
        public string _neutral;
        public int _neuTime;
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

        [System.NonSerialized]
        public SoundInfo taunt;

        [System.NonSerialized]
        public SoundInfo init;

        [System.NonSerialized]
        public SoundInfo neutral;

        [System.NonSerialized]
        public SoundInfo footstep;

        [System.NonSerialized]
        public SoundInfo footstepLayer;

        [System.NonSerialized]
        public float attack1Delay;

        [System.NonSerialized]
        public float attack2Delay;

        [System.NonSerialized]
        public float weapon1Delay;

        [System.NonSerialized]
        public float weapon2Delay;

        [System.NonSerialized]
        public float weapon1Volume;

        [System.NonSerialized]
        public float weapon2Volume;

        [System.NonSerialized]
        public float hitDelay;

        [System.NonSerialized]
        public float deathDelay;
        
        [System.NonSerialized]
        public float neutralDelay;
    }
}
