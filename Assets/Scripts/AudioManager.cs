using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void Play(string soundId)
    {
        Play(SoundInfo.Find(soundId));
    }

    public AudioSource Play(SoundInfo sound)
    {
        if (sound == null)
            return null;

        var gameObject = new GameObject("Sound " + sound.sound);
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.minDistance = 1.5f;
        Play(sound, audioSource);
        if (!sound.loop)
            Object.Destroy(gameObject, sound.clip != null ? sound.clip.length + 0.1f : 0);
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
