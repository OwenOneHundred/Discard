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
    public void PlayPrimary(string name)
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
