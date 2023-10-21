using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ChainLightning : MonoBehaviour
{
    [SerializeField] int maxTriggers = 1;
    int triggerCount = 0;

    [SerializeField] int maxConnections;
    [SerializeField] float connectionRange = 5;
    List<GameObject> alreadyHit;
    [SerializeField] float damage = 10;

    Transform previousEnemy;

   [SerializeField] GameObject lrPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || triggerCount >= maxTriggers || alreadyHit.Contains(collision.gameObject))
        {
            return;
        }

        maxTriggers += 1;

        previousEnemy = collision.transform;
        StartCoroutine(Lightning());
    }

    IEnumerator Lightning()
    {
        int connections = 0;
        while(connections < connectionRange)
        {
            GameObject closestEnemy = GetClosestNewEnemy(connectionRange);

            if (closestEnemy == null) { yield break; }

            alreadyHit.Add(closestEnemy);

            closestEnemy.GetComponent<DamageScript>().Hit(null, damage, Vector3.zero, Card.Style.None, null, 0, false);

            LineRenderer lightningLine = Instantiate(lrPrefab).GetComponent<LineRenderer>();
            lightningLine.positionCount = 2;
            lightningLine.SetPosition(0, previousEnemy.position);
            lightningLine.SetPosition(0, closestEnemy.transform.position);
            Destroy(lightningLine.gameObject, 2);

            yield return new WaitForSeconds(1);
        }
    }

    GameObject GetClosestNewEnemy(float range = 99999)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float min = range;
        GameObject minObj = null;
        foreach (GameObject enemy in enemies)
        {
            if (alreadyHit.Contains(enemy))
            {
                continue;
            }

            float distance = Vector3.Distance(enemy.transform.position, transform.position);
            if (distance < min)
            {
                min = distance;
                minObj = enemy;
            }
        }

        return minObj;
    }
}
