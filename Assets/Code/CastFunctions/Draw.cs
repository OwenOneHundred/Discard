using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardFunctions/Draw")]
public class Draw : CastFunctionAbstract
{
    [SerializeField] int count = 1;

    public override void Cast(GameObject card)
    {
        if (hm == null)
        {
            hm = GameObject.Find("Hand").GetComponent<HandManager>();
        }

        for (int i = 0; i < count; i++)
        {
            hm.DrawCard();
        }
    }
}
