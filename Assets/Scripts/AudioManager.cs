using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    Dictionary<LevelInfo, AudioSource> songs = new Dictionary<LevelInfo, AudioSource>();
    const float CrossfadeDuration = 10;
    Coroutine eventsCoroutine;
    AudioSource ambient;

    private void Awake()
    {
        instance = this;
        Level.OnLevelChange += OnLevelChange;
    }

    private void OnLevelChange(Level level, Level previous)
    {
        AudioSource song;
        if (previous != null)
        {
            song = songs.GetValueOrDefault(previous.info);
            if (song != null)
                AudioFader.Fade(song, previous.info.soundEnv.song.volume, 0, CrossfadeDuration);
        }
        song = songs.GetValueOrDefault(level.info);
        if (song == null)
        {
            song = Play(level.info.soundEnv.song);
            song.volume = 0;
            songs[level.info] = song;
        }
        AudioFader.Fade(song, 0, level.info.soundEnv.song.volume, previous == null ? 0 : CrossfadeDuration);

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
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.minDistance = 1.5f;
        return audioSource;
    }

    public AudioSource Play(SoundInfo sound)
    {
        if (sound == null)
            return null;

        var audioSource = Create("Sound " + sound.sound);
        Play(sound, audioSource);
        if (!sound.loop)
            Object.Destroy(audioSource.gameObject, sound.clip != null ? sound.clip.length + 0.1f : 0);
        return audioSource;
    }

    public AudioSource Play(SoundInfo sound, Vector3 position)
    {
        if (sound == null)
            return null;

        AudioSource audioSource = Play(sound);
        audioSource.transform.position = position;
        audioSource.spatialBlend = 1;
        return audioSource;
    }

    public AudioSource Play(SoundInfo sound, Transform parent)
    {
        if (sound == null)
            return null;

        AudioSource audioSource = Play(sound);
        audioSource.transform.SetParent(parent, false);
        audioSource.spatialBlend = 1;
        return audioSource;
    }

    public void Play(SoundInfo sound, AudioSource audioSource)
    {
        if (sound == null)
            return;

        if (sound.variations != null)
        {
            sound = sound.variations[Random.Range(0, sound.variations.Length)];
            if (sound.clip == null)
                return;
        }

        audioSource.clip = sound.clip;
        audioSource.loop = sound.loop;
        audioSource.volume = sound.volume;
        audioSource.Play();
    }
}
