using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardFunctions/Boomerang Multiplier")]
public class BoomerangMultiplier : CastFunctionAbstract
{
    float multiplier = 1.5f;

    public override float OnDamageCalculated(GameObject card)
    {
        return 1 + (multiplier * GameObject.FindGameObjectWithTag("GameManager").GetComponent<BuffManager>().consecutiveBoomerangs);
    }
}
