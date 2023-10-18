using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniSfXManager : MonoBehaviour
{
    [SerializeField] AudioSource aus;
    [SerializeField] List<Sound> clips;

    void Start()
    {
        if (aus == null) { aus = GetComponent<AudioSource>(); }
    }

    // calls aus.PlayOneShot(), so can play any number of things, but cannot pause, change volume in the middle, etc.
    public void PlaySfX(string name)
    {
        Sound sound = clips.Find(x => x.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound called as SFX (" + name + ") does not exist.");
            return;
        }

        aus.pitch = sound.pitch;
        aus.PlayOneShot(sound.clip, sound.volume);
    }

    // calls aus.Play(), so can only play one sound at a time.
    public void PlayMusic(string name)
    {
        Sound sound = clips.Find(x => x.name == name);
        if (sound == null)
        {
            Debug.LogWarning("Sound called as music (" + name + ") does not exist.");
            return;
        }

        aus.pitch = sound.pitch;
        aus.clip = sound.clip;
        aus.volume = sound.volume;
        aus.loop = sound.loop;
        aus.Play();
    }

    public void PlayMusicWithFade(string name)
    {
        Sound sound = clips.Find(i => i.name == name);
        aus.loop = true;
        aus.volume = 0;
        aus.clip = sound.clip;
        aus.Play();
        StopAllCoroutines();
        StartCoroutine(StartFade(aus, 1, sound.volume));
    }

    public void StopMusic()
    {
        aus.Stop();
        StopAllCoroutines();
    }

    public void StopMusicWithFade(float length = 1)
    {
        StopAllCoroutines();
        StartCoroutine(StartFade(aus, length, 0));
    }

    public IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }

    public IEnumerator FadeOutAndIn(AudioSource audioSource, AudioClip newClip, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, 0, currentTime / duration);
            yield return null;
        }
        audioSource.volume = 0;
        audioSource.clip = newClip;
        aus.Play();

        currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, targetVolume, currentTime / duration);
            yield return null;
        }
        audioSource.volume = targetVolume;

        yield break;
    }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        public float volume = 0.5f;
        public float pitch;
        public bool loop = false;
    }
}
