using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SecondaryEffects/Fire")]
public class FireScript : StatusEffect
{
    [SerializeField] GameObject psPrefab;
    public float damagePerSecond = 5;

    public override void OnApply(GameObject enemy)
    {
        TryAddPS(enemy, psPrefab);
    }

    public override void OnEnd(GameObject enemy)
    {
        TryRemovePS(enemy, psPrefab);
    }

    public override float EveryFrame(GameObject enemy)
    {
        return damagePerSecond * Time.deltaTime;
    }
}
