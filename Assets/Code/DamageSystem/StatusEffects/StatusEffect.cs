using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : ScriptableObject
{
    public enum EffectType
    {
        Fire, Slow, Freeze
    }

    public EffectType effectType;
    public float timer = 5;

    public virtual void OnApply(GameObject enemy)
    {

    }

    public virtual float EveryFrame(GameObject enemy)
    {
        return 0;
    }

    public virtual void OnEnd(GameObject enemy)
    {

    }

    public GameObject TryGetPSOnEnemyByTag(Transform enemy, GameObject psPrefab)
    {
        foreach (Transform i in enemy)
        {
            if (i.CompareTag(psPrefab.tag))
            {
                return i.gameObject;
            }
        }

        return null;
    }

    public void TryRemovePS(GameObject enemy, GameObject psPrefab)
    {
        GameObject ps = TryGetPSOnEnemyByTag(enemy.transform, psPrefab);
        if (ps != null)
        {
            ps.GetComponent<ParticleSystem>().Stop();
            Destroy(ps, 2);
            ps.tag = "InactivePS";
        }
    }

    public void TryAddPS(GameObject enemy, GameObject psPrefab)
    {
        if (TryGetPSOnEnemyByTag(enemy.transform, psPrefab) == null)
        {
            GameObject newPS = Instantiate(psPrefab, enemy.transform);
            var shape = newPS.GetComponent<ParticleSystem>().shape;
            shape.sprite = enemy.GetComponent<SpriteRenderer>().sprite;
            shape.scale = new Vector3(enemy.transform.localScale.x, enemy.transform.localScale.y, 1);
        }
    }

}
