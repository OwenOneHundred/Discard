using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardFunctions/SpawnObject")]
public class SpawnObject : CastFunctionAbstract
{
    [SerializeField] float destroyTime;
    [SerializeField] bool pointTowardMouse = false;
    [SerializeField] float speed;

    public override void Cast(GameObject card)
    {
        if(pt == null) {
            pt = GameObject.FindGameObjectWithTag("Player").transform;
        }

        GameObject obj = Instantiate(card.GetComponent<CardInfo>().scriptableObject.attackPrefab, pt.position, Quaternion.identity);
        SetUpObject(obj, card);
        if (pointTowardMouse)
        {
            Vector3 normalizedDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - pt.position).normalized;
            obj.transform.rotation = Quaternion.Euler(new Vector3 (0, 0, GeneralUtil.AngleBetween(Vector2.up, normalizedDirection)));
            if (speed > 0)
            {
                obj.GetComponent<Rigidbody2D>().velocity = (normalizedDirection * speed);
            }
        }


        if (destroyTime != 0)
        {
            Destroy(obj, destroyTime);
        }
    }
}
