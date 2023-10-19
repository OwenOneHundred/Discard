using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SecondaryEffects/Fire")]
public class FireScript : SecondaryEffect
{
    [SerializeField] GameObject psPrefab;

    public override void OnApply(GameObject enemy)
    {
        if (TryGetPSOnEnemyByTag(enemy.transform) == null)
        {
            GameObject newPS = Instantiate(psPrefab, enemy.transform);
            var shape = newPS.GetComponent<ParticleSystem>().shape;
            shape.sprite = enemy.GetComponent<SpriteRenderer>().sprite;
            shape.scale = new Vector3(enemy.transform.localScale.x, enemy.transform.localScale.y, 1);
        }
    }

    public override void OnEnd(GameObject enemy)
    {
        GameObject ps = TryGetPSOnEnemyByTag(enemy.transform);
        if (ps != null)
        {
            Debug.Log("here");
            ps.GetComponent<ParticleSystem>().Stop();
            Destroy(ps, 2);
            ps.tag = "InactivePS";
        }
    }

    GameObject TryGetPSOnEnemyByTag(Transform enemy)
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
}
