using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "SpellBuff/Fire Damage Multiplier")]
public class FireDamageChange : SpellBuff
{
    [SerializeField] float multiplier = 2;
    public override void OnObjectSpawned(GameObject obj, GameObject card)
    {
        HitboxManager[] hms = obj.transform.root.GetComponentsInChildren<HitboxManager>();
        foreach (HitboxManager hm in hms)
        {
            foreach (StatusEffect se in hm.statusEffects)
            {
                if (se.GetType() == typeof(FireScript))
                {
                    var transSE = (FireScript)se;
                    transSE.damagePerSecond = multiplier;
                }
            }
        }
    }
}
