using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardFunctions/DiscardLast")]
public class DiscardLast : CastFunctionABS
{
    [SerializeField] int count = 1;

    public override void Cast()
    {
        Debug.Log("cast called");
        if (hm == null)
        {
            hm = GameObject.Find("Hand").GetComponent<HandManager>();
        }

        for (int i = 0; i < count; i++)
        {
            if (hm.Hand.Count <= 1) { return; }
            List<GameObject> handCopy = new List<GameObject>(hm.Hand);
            handCopy.Remove(hm.SelectedCard);
            hm.DiscardCard(handCopy[^1]);
        }
    }
}
