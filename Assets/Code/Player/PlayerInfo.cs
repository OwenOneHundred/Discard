using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerInfo : MonoBehaviour
{
    public float hp;
    public Slider hpSlider; 
    [System.NonSerialized] public WorldGenerator.Biome currentBiome;

    public GameObject deathPanel;

    //Damages Player
    public void Damage(int damage)
    {
        hp = hp - damage;
        hpSlider.value = hp;

        if(hp <= 0f)
        {
            Death();
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

    //Handles the death of the player
    public void Death()
    {
        Time.timeScale = .0f;
        deathPanel.SetActive(true);
        StartCoroutine("DeathWait");
    }

    private IEnumerator DeathWait()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        SceneManager.LoadScene("Menu");
    }
}
