using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerInfo : MonoBehaviour
{
    public float hp;
    public Slider hpSlider; 
    [System.NonSerialized] public WorldGenerator.Biome currentBiome;

    //Damages Player
    public void Damage(int damage)
    {
        hp = hp - damage;
        hpSlider.value = hp;

        if(hp <= 0f)
        {
            Debug.Log("Player Death");
        }
    }

    private void Start()
    {
        hpSlider.maxValue = hp;
        hpSlider.value = hp;
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
