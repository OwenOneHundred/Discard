using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    LootManager lm;
    [SerializeField] int optionCount;
    List<Card> options = new List<Card>();

    private void Start()
    {
        lm = GetComponent<LootManager>();
    }

    public void Open()
    {
        for (int i = 0; i < optionCount; i++)
        {
            options.Add(lm.GetLootCard());
        }

        Debug.Log("If this system was set up, you would have gotten these choices:");
        foreach (Card card in options)
        {
            Debug.Log(card.name);
        }
    }
}
