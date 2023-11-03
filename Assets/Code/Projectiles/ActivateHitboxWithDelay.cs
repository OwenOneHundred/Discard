using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateHitboxWithDelay : MonoBehaviour
{
    [SerializeField] float delayTime;
    [SerializeField] Collider2D hitbox;
    [SerializeField] float delayBeforeDestroy = 0.05f;
    float timer = 0;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > delayTime)
        {
            hitbox.enabled = true;
            Destroy(gameObject, delayBeforeDestroy);
        }
    }
}
