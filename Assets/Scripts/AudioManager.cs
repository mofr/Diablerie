using UnityEngine;

public class AudioManager
{
    public static AudioSource Play(SoundInfo sound)
    {
        if (sound == null || sound.clip == null)
            return null;

        var gameObject = new GameObject("Sound " + sound.sound);
        var audioSource = gameObject.AddComponent<AudioSource>();
        Play(sound, audioSource);
        if (sound != null && sound.clip != null)
            Object.Destroy(gameObject, sound.clip.length);
        return audioSource;
    }

    public static AudioSource Play(SoundInfo sound, Vector3 position)
    {
        if (sound == null || sound.clip == null)
            return null;

        AudioSource audioSource = Play(sound);
        audioSource.transform.position = position;
        audioSource.spatialBlend = 1;
        return audioSource;
    }

    public static void Play(SoundInfo sound, AudioSource audioSource)
    {
        if (sound == null || sound.clip == null)
            return;

        audioSource.clip = sound.clip;
        audioSource.loop = sound.loop;
        audioSource.volume = sound.volume;
        audioSource.Play();
    }
}
