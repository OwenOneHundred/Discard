using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientSound : MonoBehaviour
{
    [SerializeField] PlayerInfo pi;
    WorldGenerator.Biome recognizedBiome = null;
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
                StartCoroutine(msfx.FadeOutAndIn(aus, pi.currentBiome.ambientNoise.clip, 2, pi.currentBiome.ambientNoise.volume));
            }
            else
            {
                aus.clip = pi.currentBiome.ambientNoise.clip;
                aus.volume = pi.currentBiome.ambientNoise.volume;
                aus.Play();
            }

            recognizedBiome = pi.currentBiome;

        }
    }

    

}
