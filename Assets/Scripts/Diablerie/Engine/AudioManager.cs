using System.Collections;
using System.Collections.Generic;
using Diablerie.Engine.Datasheets;
using Diablerie.Engine.LibraryExtensions;
using UnityEngine;

namespace Diablerie.Engine
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance => _instance;
        
        private static AudioManager _instance;
        Dictionary<LevelInfo, AudioSource> songs = new Dictionary<LevelInfo, AudioSource>();
        const float CrossfadeDuration = 10;
        Coroutine eventsCoroutine;
        AudioSource ambient;
        private Dictionary<SoundInfo, float> lastStartedMap = new Dictionary<SoundInfo, float>();

        public static void Initialize()
        {
            new GameObject("AudioManager", typeof(AudioManager));
        }
        
        private void Awake()
        {
            Debug.Assert(_instance == null);
            _instance = this;
            Level.OnLevelChange += OnLevelChange;
            DontDestroyOnLoad(this);
        }

        private void OnLevelChange(Level level, Level previous)
        {
            var crossfadeDuration = CrossfadeDuration;
            if (previous == null || level.info.act != previous.info.act)
            {
                crossfadeDuration = 0;
            }
            AudioSource song;
            if (previous != null)
            {
                song = songs.GetValueOrDefault(previous.info);
                if (song != null)
                    AudioFader.Fade(song, previous.info.soundEnv.song.volume, 0, crossfadeDuration);
            }
            song = songs.GetValueOrDefault(level.info);
            if (song == null)
            {
                song = Play(level.info.soundEnv.song);
                song.volume = 0;
                songs[level.info] = song;
            }
            AudioFader.Fade(song, 0, level.info.soundEnv.song.volume, crossfadeDuration);

            if (ambient == null)
                ambient = Create("Ambient sound");
            Play(level.info.soundEnv.dayAmbience, ambient);

            if (eventsCoroutine != null)
                StopCoroutine(eventsCoroutine);
            if (level != null)
                eventsCoroutine = StartCoroutine(PlayEnvEvents());
            else
                eventsCoroutine = null;
        }

        IEnumerator PlayEnvEvents()
        {
            while(isActiveAndEnabled)
            {
                yield return new WaitForSeconds(Level.current.info.soundEnv.eventDelay);
                Play(Level.current.info.soundEnv.dayEvent);
            }
        }

        public void Play(string soundId)
        {
            Play(SoundInfo.Find(soundId));
        }

        public AudioSource Create(string name)
        {
            var gameObject = new GameObject(name);
            gameObject.transform.SetParent(transform, true);
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.minDistance = 1.5f;
            audioSource.dopplerLevel = 0.0f;
            return audioSource;
        }

        public AudioSource Play(SoundInfo sound, float delay = 0, float volume = -1)
        {
            if (sound == null)
                return null;

            var audioSource = Create("Sound " + sound.sound);
            Play(sound, audioSource, delay: delay, volume: volume);
            if (!sound.loop)
                Destroy(audioSource.gameObject, sound.clip != null ? sound.clip.length + 0.1f : 0);
            return audioSource;
        }

        public AudioSource Play(SoundInfo sound, Vector3 position, float delay = 0, float volume = -1)
        {
            if (sound == null)
                return null;

            AudioSource audioSource = Play(sound, delay: delay, volume: volume);
            audioSource.transform.position = position;
            audioSource.spatialBlend = 1;
            return audioSource;
        }

        public AudioSource Play(SoundInfo sound, Transform parent, float delay = 0, float volume = -1)
        {
            if (sound == null)
                return null;

            AudioSource audioSource = Play(sound, delay: delay, volume: volume);
            audioSource.transform.SetParent(parent, false);
            audioSource.spatialBlend = 1;
            return audioSource;
        }

        public void Play(SoundInfo sound, AudioSource audioSource, float delay = 0, float volume = -1)
        {
            if (sound == null)
                return;

            float lastStarted;
            if (lastStartedMap.TryGetValue(sound, out lastStarted))
            {
                if (Time.time - lastStarted < sound.compoundDuration)
                    return;
            }
            lastStartedMap[sound] = Time.time;

            if (sound.variations != null)
            {
                sound = sound.variations[Random.Range(0, sound.variations.Length)];
                if (sound.clip == null)
                    return;
            }

            audioSource.clip = sound.clip;
            audioSource.loop = sound.loop;

            if (volume >= 0)
                audioSource.volume = volume;
            else
                audioSource.volume = sound.volume;

            if (delay > 0)
                audioSource.PlayDelayed(delay);
            else
                audioSource.Play();
        }
    }
}
