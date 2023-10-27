using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BuffManager : MonoBehaviour
{
    [SerializeField] List<SpellBuff> spellBuffs = new List<SpellBuff>();

    public List<Pair<Style, Color32>> stylesAndColors;

    public void AddSpellBuff(SpellBuff spellBuff)
    {
        spellBuffs.Add(spellBuff);
    }

    public void RemoveSpellBuff(SpellBuff spellBuff)
    {
        spellBuffs.Remove(spellBuff);
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

    public float GetDamageTypeMultiplier(DamageType damageType)
    {
        float multiplier = 1;
        foreach (SpellBuff buff in spellBuffs)
        {
            Pair<DamageType, float> info = buff.damageTypeMultipliers.Find(x => x.left == damageType);
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

    public enum DamageType
    {
        Explosion, Lightning, None
    }

    public enum Style
    {
        launch, pinpoint, homing, melee, None
    }
}
