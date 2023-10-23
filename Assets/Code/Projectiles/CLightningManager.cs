using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CLightningManager : MonoBehaviour
{
    Transform previousEnemy;

    [SerializeField] GameObject lrPrefab;
    [SerializeField] List<SecondaryEffect> secondaryEffects;

    float damage = 10;
    int maxConnections;
    float connectionRange = 5;

    List<GameObject> alreadyHit = new List<GameObject>();

    public void OnStart(float damage1, int maxConnections1, float connectionRange1, GameObject initialCollision)
    {
        damage = damage1; maxConnections = maxConnections1; connectionRange = connectionRange1;
        alreadyHit.Add(initialCollision);
        previousEnemy = initialCollision.transform;
        StartCoroutine(Lightning());
    }

    IEnumerator Lightning()
    {
        yield return new WaitForSeconds(0.2f);
        int connections = 0;
        while(connections < maxConnections)
        {
            GameObject closestEnemy = GetClosestNewEnemy(connectionRange);

            if (closestEnemy == null) { yield break; }

            alreadyHit.Add(closestEnemy);

            closestEnemy.GetComponent<DamageScript>().Hit(secondaryEffects, damage, Vector3.zero, BuffManager.Style.None, null, 0, false);

            LineRenderer lightningLine = Instantiate(lrPrefab).GetComponent<LineRenderer>();
            lightningLine.positionCount = 2;
            lightningLine.SetPosition(0, previousEnemy.position);
            lightningLine.SetPosition(1, closestEnemy.transform.position);
            Destroy(lightningLine.gameObject, 0.2f);

            previousEnemy = closestEnemy.transform;

            connections += 1;

            yield return new WaitForSeconds(0.2f);
        }

        Destroy(gameObject);
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
