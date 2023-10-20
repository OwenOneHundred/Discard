using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : MonoBehaviour
{
    [SerializeField] Collider2D associatedHitbox;

    [SerializeField] float damage;

    [SerializeField] float knockback = 5000;

    [SerializeField] bool KbOutInsteadOfBack = false;

    [Tooltip("For knockback, by default, forward is Vector3.right. This makes forward Vector3.up.")]
    [SerializeField] bool forwardIsUp = false;

    [SerializeField] Card.Style style;
    [SerializeField] DamageType damageType;
    [SerializeField] List<SecondaryEffect> secondaryEffects;
    [SerializeField] GameObject onHitPS;
    [SerializeField] bool canHitMultipleTimes = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DamageScript damageScript))
        {
            if (!associatedHitbox.IsTouching(collision)) { return; }

            if (onHitPS != null)
            {
                Instantiate(onHitPS, transform.position, Quaternion.identity);
            }

            damageScript.OnHit(secondaryEffects, damage, CalculateKB(collision.transform.position), style, gameObject);
        }
    }

    private Vector2 CalculateKB(Vector2 collisionPosition)
    {
        if (KbOutInsteadOfBack)
        {
            return (collisionPosition - (Vector2) associatedHitbox.bounds.center).normalized * knockback;
        }
        else
        {
            return forwardIsUp ? transform.up * knockback : Vector3.right * knockback;
        }
    }

    public enum DamageType
    {
        Explosion, Lightning, None
    }
}
