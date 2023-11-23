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
        if (enemy.TryGetComponent(out EnemyMovement em))
        {
            em.ChangeSpeedModifier(decreaseMultiplier);
        }
        else
        {
            Debug.Log("Could not slow " + enemy.name + " because it does not have an EnemyMovement component.");
        }

        TryAddPS(enemy, psPrefab);
    }

    public override void OnEnd(GameObject enemy)
    {
        if (enemy.TryGetComponent(out EnemyMovement em))
        {
            em.ChangeSpeedModifier(-decreaseMultiplier);
        }
        else
        {
            Debug.Log("Could not slow " + enemy.name + " because it does not have an EnemyMovement component.");
        }

        TryRemovePS(enemy, psPrefab);
    }
}
