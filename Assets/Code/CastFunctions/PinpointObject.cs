using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardFunctions/PinpointObject")]
public class PinpointObject : CastFunctionAbstract
{
    [SerializeField] float destroyTime;

    public override void Cast(GameObject card)
    {
        if(pt == null) {
            pt = GameObject.FindGameObjectWithTag("Player").transform;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject obj = Instantiate(card.GetComponent<CardInfo>().scriptableObject.attackPrefab, new Vector3(mousePos.x, mousePos.y, 0), Quaternion.identity);
        SetUpObject(obj, card);

        if (destroyTime != 0)
        {
            Destroy(obj, destroyTime);
        }
    }
}
