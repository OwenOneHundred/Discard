using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] PlayerInfo pi;
    WorldGenerator.Biome recognizedBiome;
    [SerializeField] AudioSource aus;
    MiniSfXManager msfx;

    private void Start()
    {
        msfx = GetComponent<MiniSfXManager>();
    }

    private void Update()
    {
        if (pi.currentBiome != recognizedBiome)
        {
            if (recognizedBiome != null)
            {
                if (pi.currentBiome.music.name != recognizedBiome.music.name)
                {
                    StartCoroutine(msfx.FadeOutAndIn(aus, pi.currentBiome.music.clip, 2, pi.currentBiome.music.volume));
                }
            }
            else // runs on the first frame only I think
            {
                aus.clip = pi.currentBiome.music.clip;
                aus.volume = pi.currentBiome.music.volume;
                aus.Play();
            }

            recognizedBiome = pi.currentBiome;

        }
    }
}
