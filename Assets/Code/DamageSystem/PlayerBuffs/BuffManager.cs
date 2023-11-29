using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuffManager : MonoBehaviour
{
    public int consecutiveBoomerangs = 0;
    HandManager hm;

    [SerializeField] List<SpellBuff> spellBuffs = new List<SpellBuff>();

    public List<Pair<Style, Color32>> stylesAndColors;

    private void Start()
    {
        hm = GameObject.Find("Hand").GetComponent<HandManager>();
    }

    public void AddSpellBuff(SpellBuff spellBuff)
    {
        spellBuffs.Add(spellBuff);
        hm.UpdateCardDamages();
    }

    public void RemoveSpellBuff(SpellBuff spellBuff)
    {
        spellBuffs.Remove(spellBuff);
        hm.UpdateCardDamages();
    }

    public float GetStyleMultiplier(Style style)
    {
        float multiplier = 1;
        foreach (SpellBuff buff in spellBuffs)
        {
            Pair<Style, float> info = buff.styleDamageMultipliers.Find(x => x.left == style);
            if (info != null)
            {
                multiplier *= info.right;
            }
        }
        return multiplier;
    }

    public float GetDamageTypeMultiplier(StatusEffect damageType)
    {
        float multiplier = 1;
        foreach (SpellBuff buff in spellBuffs)
        {
            Pair<StatusEffect, float> info = buff.damageTypeMultipliers.Find(x => x.left == damageType);
            if (info != null)
            {
                multiplier *= info.right;
            }
        }
        return multiplier;
    }

    public void OnProjectileSpawned(GameObject obj, GameObject card)
    {
        foreach (SpellBuff sb in spellBuffs)
        {
            sb.OnObjectSpawned(obj, card);
        }
    }

    public Color32 GetStyleColor(Style style)
    {
        return stylesAndColors.Find(x => x.left == style).right;
    }

    public enum StatusEffect
    {
        Fire, Lightning, None
    }

    public enum Style
    {
        launch, pinpoint, homing, melee, None
    }
}
