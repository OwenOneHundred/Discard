using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHitbox : MonoBehaviour
{
    [SerializeField] CircleCollider2D cc;
    [SerializeField] float maxSize;
    [SerializeField] float expansionTime;
    [SerializeField] bool startOnAwake = true;
    [SerializeField] bool disableAtMaxSize = true;
    [SerializeField] float lifetime = 0f;

    private void Start()
    {
        if (startOnAwake)
        {
            StartExplosion();
        }

        if (lifetime > 0)
        {
            Destroy(gameObject, lifetime);
        }
    }

    public void StartExplosion()
    {
        EnableHitbox();
        StartCoroutine(StartSizeChange());
    }

    public void EnableHitbox()
    {
        cc.enabled = true;
    }

    public void DisableHitbox()
    {
        cc.enabled = false;
    }

    private IEnumerator StartSizeChange()
    {
        float timer = 0;
        float startSize = cc.radius;
        while (cc.radius < maxSize)
        {
            cc.radius = Mathf.Lerp(startSize, maxSize, timer / expansionTime);
            timer += Time.deltaTime;
            yield return null;
        }
        if (disableAtMaxSize)
        {
            cc.enabled = false;
        }
    }
}
