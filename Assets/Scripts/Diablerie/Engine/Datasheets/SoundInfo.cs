using System.Collections.Generic;
using System.IO;
using Diablerie.Engine.Entities;
using Diablerie.Engine.IO.D2Formats;
using Diablerie.Engine.LibraryExtensions;
using UnityEngine;

namespace Diablerie.Engine.Datasheets
{
    [System.Serializable]
    [Datasheet.Record]
    public class SoundInfo
    {
        public static List<SoundInfo> sheet = Datasheet.Load<SoundInfo>("data/global/excel/Sounds.txt");
        static Dictionary<string, SoundInfo> map = new Dictionary<string, SoundInfo>();

        public static SoundInfo itemPickup;
        public static SoundInfo itemFlippy;
        public static SoundInfo cursorButtonClick;

        public static SoundInfo Find(string soundCode)
        {
            if (soundCode == null)
                return null;

            return map.GetValueOrDefault(soundCode);
        }

        public static SoundInfo GetHitSound(int hitClass, Character hitCharacter)
        {
            if (hitClass == 10 && hitCharacter != null)
                return SoundInfo.Find("impact_arrow_1");
            if (hitClass == 32)
                return SoundInfo.Find("impact_fire_1");
            if (hitClass == 48)
                return SoundInfo.Find("impact_cold_1");
            if (hitClass == 64)
                return SoundInfo.Find("impact_lightning_1");
            if (hitClass == 80)
                return SoundInfo.Find("impact_poison_1");
            if (hitClass == 96)
                return SoundInfo.Find("impact_stun_1");
            if (hitClass == 112)
                return SoundInfo.Find("impact_bash");
            if (hitClass == 176)
                return SoundInfo.Find("impact_goo_1");
            return null;
        }

        static SoundInfo()
        {
            for(int i = 0; i < sheet.Count; ++i)
            {
                var sound = sheet[i];
                if (sound.sound == null)
                    continue;

                GatherVariations(sound, i);
                sound.volume = sound._volume / 255f;
                sound.fadeInDuration = sound._fadeIn / 25f;
                sound.fadeOutDuration = sound._fadeOut / 25f;
                sound.compoundDuration = sound.compound / 25f;
                map.Add(sound.sound, sound);
            }

            itemPickup = Find("item_pickup");
            itemFlippy = Find("item_flippy");
            cursorButtonClick = Find("cursor_button_click");
        }

        static void GatherVariations(SoundInfo sound, int index)
        {
            if (sound.groupSize > 0)
            {
                sound.variations = new SoundInfo[sound.groupSize];
                for (int i = 0; i < sound.groupSize; ++i)
                    sound.variations[i] = sheet[i + index];
            }
        }

        public string FindFile()
        {
            var filename = @"data\global\sfx\" + _filename;
            if (Mpq.fs.HasFile(filename))
                return filename;

            filename = @"data\global\music\" + _filename;
            if (Mpq.fs.HasFile(filename))
                return filename;
            
            filename = @"data\local\sfx\" + _filename;
            if (Mpq.fs.HasFile(filename))
                return filename;
    
            return null;
        }

        public AudioClip clip
        {
            get {
                if (audioClip != null)
                    return audioClip;

                var filename = FindFile();
                if (filename == null)
                    return null;

                Stream wavStream = Mpq.fs.OpenFile(filename);
                var clip = Wav.Load(sound, stream, wavStream);
                if (!stream)
                {
                    wavStream.Close();
                    audioClip = clip;
                }
    
                return clip;
            }
        }
    
        public string sound;
        public uint index;
        public string _filename;
        public uint _volume;
        public uint groupSize;
        public bool loop;
        public uint _fadeIn;
        public uint _fadeOut;
        public bool deferInst;
        public bool stopInst;
        public uint duration;
        public int compound;
        public uint reverb;
        public uint falloff;
        public bool cache;
        public bool asyncOnly;
        public int priority;
        public bool stream;
        public bool stereo;
        public bool tracking;
        public bool solo;
        public bool musicVol;
        public int block1;
        public int block2;
        public int block3;

        [System.NonSerialized]
        AudioClip audioClip;

        [System.NonSerialized]
        public float volume;

        [System.NonSerialized]
        public float fadeOutDuration;

        [System.NonSerialized]
        public float fadeInDuration;

        [System.NonSerialized]
        public SoundInfo[] variations;
    
        [System.NonSerialized]
        public float compoundDuration;
    }
}
