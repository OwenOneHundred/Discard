using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnimationEvents : MonoBehaviour
{
    public void PlayParticleSystem()
    {
        GetComponent<ParticleSystem>().Play();
    }

    public void PlayParticleSystemOnChild(int index)
    {
        transform.GetChild(index).GetComponent<ParticleSystem>().Play();
    }
}
