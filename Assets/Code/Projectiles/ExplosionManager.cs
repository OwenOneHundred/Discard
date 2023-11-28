using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    [SerializeField] GameObject toSpawnOnDeath;
    [SerializeField] float deathObjMultiplier = 1;
    float actualDamage;
    
    public void OnDeath()
    {
        GameObject newObj = Instantiate(toSpawnOnDeath, transform.position, transform.rotation);
        Debug.Log("ActualDamage = " + actualDamage);
        if (newObj.TryGetComponent(out HitboxManager hbm))
        {
            hbm.damage = actualDamage * deathObjMultiplier;
        }
    }

    public void OnSpawn(float actualDamage1)
    {
        actualDamage = actualDamage1;
    }
}
