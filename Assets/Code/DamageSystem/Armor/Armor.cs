using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Armor")] 
public class Armor : ScriptableObject
{
    public List<SpellBuff> spellBuffs = new List<SpellBuff>();
    public float defense;
    public Sprite image;

    public enum ArmorType
    {
        helmet, chestplate, leggings
    }

    public ArmorType armorType;
}
