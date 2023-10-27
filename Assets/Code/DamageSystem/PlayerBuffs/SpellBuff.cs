using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellBuff : ScriptableObject
{
    public List<Pair<BuffManager.Style, float>> styleDamageMultipliers;
    public List<Pair<BuffManager.DamageType, float>> damageTypeMultipliers;

    public virtual void OnObjectSpawned(GameObject obj, GameObject card)
    {

    }
}
