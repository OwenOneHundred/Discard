using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public int hp;
    [System.NonSerialized] public WorldGenerator.Biome currentBiome;

    //Damages Player
    public void Damage()
    {
        hp--;

        if(hp <= 0)
        {
            Debug.Log("Player Death");
        }
    }

    private void Start()
    {
        StartCoroutine(CheckBiomePosition());
    }

    private IEnumerator CheckBiomePosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            currentBiome = GeneralUtil.GetBiomeAtPos(transform.position);
        }
    }
}
