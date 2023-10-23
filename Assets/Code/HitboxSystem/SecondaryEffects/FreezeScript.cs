using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SecondaryEffects/Freeze")]
public class FreezeScript : SecondaryEffect
{
    public override void OnApply(GameObject enemy)
    {
        if (enemy.TryGetComponent(out EnemyMind em))
        {
            em.Freeze(true);
        }
        else
        {
            Debug.Log("Could not freeze " + enemy.name + " because it does not have an EnemyMind component.");
        }

    }

    public override void OnEnd(GameObject enemy)
    {
        if (enemy.TryGetComponent(out EnemyMind em))
        {
            em.Freeze(false);
        }
        else
        {
            Debug.Log("Could not freeze " + enemy.name + " because it does not have an EnemyMind component.");
        }
    }
}
