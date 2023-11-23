using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionAndDestroyOnHit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Barrier"))
        {
            if (TryGetComponent<ExplosionManager>(out ExplosionManager em))
            {
                em.OnDeath();
            }

            Destroy(gameObject);
        }
    }
}
