using UnityEngine;

namespace Diablerie.Engine
{
    public class AudioFader : MonoBehaviour
    {
        AudioSource audioSource;
        float time = 0;
        float initialVolume;
        float targetVolume;
        float duration = 0;

        public static void Fade(AudioSource audioSource, float initialVolume, float targetVolume, float duration)
        {
            var fader = audioSource.GetComponent<AudioFader>();
            if (fader == null)
                fader = audioSource.gameObject.AddComponent<AudioFader>();
            fader.initialVolume = initialVolume;
            fader.targetVolume = targetVolume;
            fader.duration = duration;
            fader.time = Mathf.InverseLerp(initialVolume, targetVolume, audioSource.volume) * duration;
            fader.enabled = true;
        }

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
    
        void Update()
        {
            time += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(initialVolume, targetVolume, time / duration);
            if (time > duration)
                enabled = false;
        }
    }
}
