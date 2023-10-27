using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorManager : MonoBehaviour
{
    BuffManager bm;

    public Armor helmet;
    public Armor chestplate;
    public Armor leggings;

    Dictionary<Armor.ArmorType, Armor> armorTypeDict = new();
    private void Awake()
    {
        armorTypeDict.Add(Armor.ArmorType.helmet, helmet);
        armorTypeDict.Add(Armor.ArmorType.chestplate, chestplate);
        armorTypeDict.Add(Armor.ArmorType.leggings, leggings);
    }

    private void Start()
    {
        bm = GetComponent<BuffManager>();
    }

    /// <summary>
    /// Tries to put armor in slot, but fails if there's already armor in that slot. Returns true/false for if successful.
    /// </summary>
    public bool TryAddArmor(Armor newArmor)
    {
        if (armorTypeDict[newArmor.armorType] == null)
        {
            armorTypeDict[newArmor.armorType] = newArmor;
            AddArmorBuffs(newArmor);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Remove armor from slot.
    /// </summary>
    public void RemoveArmor(Armor.ArmorType armorType)
    {
        RemoveArmorBuffs(armorTypeDict[armorType]);
        armorTypeDict[armorType] = null;
    }

    /// <summary>
    /// Puts armor on regardless of whether an armor is in that slot already. Returns old armor; null if nothing.
    /// </summary>
    public Armor ForceArmorOnSlot(Armor newArmor)
    {
        Armor oldArmor = armorTypeDict[newArmor.armorType];
        RemoveArmor(newArmor.armorType);
        TryAddArmor(newArmor);
        return oldArmor;
    }

    private void AddArmorBuffs(Armor armor)
    {
        foreach (SpellBuff spellBuff in armor.spellBuffs)
        {
            bm.AddSpellBuff(spellBuff);
        }
    }

    private void RemoveArmorBuffs(Armor armor)
    {
        foreach (SpellBuff spellBuff in armor.spellBuffs)
        {
            bm.RemoveSpellBuff(spellBuff);
        }
    }

}
