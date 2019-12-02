using System.Collections.Generic;
using Diablerie.Engine.IO.D2Formats;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class SoundEnvironment
    {
        public static List<SoundEnvironment> sheet;

        public static SoundEnvironment Find(int index)
        {
            return sheet[index];
        }

        public static void Load()
        {
            sheet = Datasheet.Load<SoundEnvironment>("data/global/excel/SoundEnviron.txt");
            foreach(var env in sheet)
            {
                env.song = SoundInfo.sheet[env.songId];
                env.dayAmbience = SoundInfo.sheet[env.dayAmbienceId];
                env.nightAmbience = SoundInfo.sheet[env.nightAmbienceId];
                env.dayEvent = SoundInfo.sheet[env.dayEventId];
                env.nightEvent = SoundInfo.sheet[env.nightEventId];
                env.eventDelay = env._eventDelay / 25f;
            }
        }

        public string handle;
        public int index;
        public int songId;
        public int dayAmbienceId;
        public int nightAmbienceId;
        public int dayEventId;
        public int nightEventId;
        public int _eventDelay;
        public bool indoors;
        public int material1;
        public int material2;
        public int EAXEnviron;
        public int EAXEnvSize;
        public int EAXEnvDiff;
        public int EAXRoomVol;
        public int EAXRoomHF;
        public int EAXDecayTime;
        public int EAXDecayHF;
        public int EAXReflect;
        public int EAXReflectDelay;
        public int EAXReverb;
        public int EAXRevDelay;
        public int EAXRoomRoll;
        public int EAXAirAbsorb;

        [System.NonSerialized]
        public SoundInfo song;
        [System.NonSerialized]
        public SoundInfo dayAmbience;
        [System.NonSerialized]
        public SoundInfo nightAmbience;
        [System.NonSerialized]
        public SoundInfo dayEvent;
        [System.NonSerialized]
        public SoundInfo nightEvent;
        [System.NonSerialized]
        public float eventDelay;
    }
}
