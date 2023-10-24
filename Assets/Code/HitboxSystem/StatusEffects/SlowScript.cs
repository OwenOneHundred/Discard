using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SecondaryEffects/Slow")]
public class SlowScript : StatusEffect
{
    [SerializeField] GameObject psPrefab;
    [SerializeField] float decreaseMultiplier;

    public override void OnApply(GameObject enemy)
    {
        enemy.GetComponent<EnemyMovement>().ChangeSpeedModifier(decreaseMultiplier);

        TryAddPS(enemy, psPrefab);
    }

    public override void OnEnd(GameObject enemy)
    {
        enemy.GetComponent<EnemyMovement>().ChangeSpeedModifier(-decreaseMultiplier);

        TryRemovePS(enemy, psPrefab);
    }
}
