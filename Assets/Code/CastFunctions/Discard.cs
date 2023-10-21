using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardFunctions/Discard")]
public class Discard : CastFunctionABS
{
    [SerializeField] int count = 1;

    [Tooltip("Select true to discard the first card in hand, false to discard the last card.")]
    [SerializeField] bool firstOrLast = false;

    public override void Cast(GameObject card)
    {
        if (hm == null)
        {
            hm = GameObject.Find("Hand").GetComponent<HandManager>();
        }

        if (firstOrLast)
        {
            for (int i = 0; i < count; i++)
            {
                if (hm.Hand.Count <= 1) { return; }

                List<GameObject> handCopy = new List<GameObject>(hm.Hand);
                handCopy.Remove(card);

                hm.DiscardCard(handCopy[0]);
            }
        }
        else
        {
            for (int i = 0; i < count; i++)
            {
                if (hm.Hand.Count <= 1) { return; }

                List<GameObject> handCopy = new List<GameObject>(hm.Hand);
                handCopy.Remove(card);

                hm.DiscardCard(handCopy[^1]);
            }
        }
    }
}
