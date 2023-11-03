using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorItem : MonoBehaviour
{
    Armor armor;

    public void SetUp(Armor armor)
    {
        GetComponent<SpriteRenderer>().sprite = armor.image;
        this.armor = armor;
    }

    public void Pickup()
    {
        ArmorManager am = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ArmorManager>();
        am.ForceArmorOnSlot(armor);
        SelfDestruct();
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
