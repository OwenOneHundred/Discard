using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardFunctions/SpawnObject")]
public class SpawnObject : CastFunctionABS
{
    [SerializeField] float delay = 0;

    public override void Cast(GameObject card)
    {
        if(pt == null) {
            pt = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (delay != 0)
        {
            pt.GetComponent<PlayerMovement>().StartCoroutine(Timer(card)); // this line is extremely shit LOL this breaks super hard if we ever destroy the player
        }
        else
        {
            Instantiate(card.GetComponent<CardInfo>().scriptableObject.attackPrefab, pt.position, Quaternion.identity);
        }

    }

    IEnumerator Timer(GameObject card)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(card.GetComponent<CardInfo>().scriptableObject.attackPrefab, pt.position, Quaternion.identity);
    }
}
