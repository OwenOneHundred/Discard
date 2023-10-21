using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SecondaryEffects/Slow")]
public class SlowScript : SecondaryEffect
{
    [SerializeField] GameObject psPrefab;
    [SerializeField] float decreaseMultiplier;

    public override void OnApply(GameObject enemy)
    {
        enemy.GetComponent<EnemyMovement>().ChangeSpeedModifer(decreaseMultiplier);

        TryAddPS(enemy, psPrefab);
    }

    public override void OnEnd(GameObject enemy)
    {
        enemy.GetComponent<EnemyMovement>().ChangeSpeedModifer(-decreaseMultiplier);

        TryRemovePS(enemy, psPrefab);
    }
}
