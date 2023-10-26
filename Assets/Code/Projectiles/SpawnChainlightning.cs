using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChainlightning : MonoBehaviour
{
    [SerializeField] float damage = 20;

    [SerializeField] int maxConnections;
    [SerializeField] float connectionRange = 5;

    [SerializeField] int maxTriggers = 1;
    int triggerCount = 0;

    [SerializeField] GameObject CLManagerPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") || triggerCount >= maxTriggers)
        {
            return;
        }
        if (!collision.GetComponent<DamageScript>().Hit(damage))
        {
            return;
        }

        maxTriggers += 1;

        Instantiate(CLManagerPrefab).GetComponent<CLightningManager>().OnStart(damage, maxConnections, connectionRange, collision.gameObject);
    }
}
