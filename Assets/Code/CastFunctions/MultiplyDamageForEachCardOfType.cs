using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardFunctions/DamageXByEffectInHand")]
public class MultiplyDamageForEachCardOfType : CastFunctionAbstract
{
    [SerializeField] float multiplier = 2;
    [SerializeField] BuffManager.StatusEffect effect;

    public override float OnDamageCalculated(GameObject card)
    {
        if (hm == null)
        {
            hm = GameObject.Find("Hand").GetComponent<HandManager>();
        }

        int count = 0;
        foreach (GameObject go in hm.Hand)
        {
            if (go == card) { continue; }
            if (go.GetComponent<CardInfo>().scriptableObject.damageType == effect)
            {
                count += 1;
            }
        }
        return count == 0 ? 1 : count * multiplier;
    }
}
