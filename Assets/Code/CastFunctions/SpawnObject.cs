using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardFunctions/SpawnObject")]
public class SpawnObject : CastFunctionABS
{
    [SerializeField] float destroyTime;
    [SerializeField] bool pointTowardMouse = false;

    public override void Cast(GameObject card)
    {
        if(pt == null) {
            pt = GameObject.FindGameObjectWithTag("Player").transform;
        }

        GameObject obj = Instantiate(card.GetComponent<CardInfo>().scriptableObject.attackPrefab, pt.position, Quaternion.identity);
        if (pointTowardMouse)
        {
            Vector3 normalizedDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - pt.position).normalized;
            obj.transform.rotation = Quaternion.Euler(new Vector3 (0, 0, GeneralUtil.AngleBetween(Vector2.up, normalizedDirection)));
        }

        if (destroyTime != 0)
        {
            Destroy(obj, destroyTime);
        }
    }
}
