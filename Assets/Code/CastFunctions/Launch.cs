using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardFunctions/Launch")]
public class Launch : CastFunctionABS
{
    public override void Cast(GameObject card)
        {
            Instantiate(card.GetComponent<CardInfo>().scriptableObject.attackPrefab,GameObject.FindGameObjectWithTag("Player").transform);
        }
}
