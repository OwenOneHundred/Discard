using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardFunctions/Launch")]
public class Launch : CastFunctionAbstract
{
    public override void Cast(GameObject card)
        {
            if(pt == null) {
                pt = GameObject.FindGameObjectWithTag("Player").transform;
            }
            SetUpObject(Instantiate(card.GetComponent<CardInfo>().scriptableObject.attackPrefab, pt.position, Quaternion.identity), card);
        }
}
