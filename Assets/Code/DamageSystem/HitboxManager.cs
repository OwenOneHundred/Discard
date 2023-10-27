using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxManager : MonoBehaviour
{
    [SerializeField] Collider2D associatedHitbox;

    [SerializeField] float knockback = 5000;

    [SerializeField] bool KbOutInsteadOfBack = false;

    [Tooltip("For knockback, by default, forward is Vector3.right. Check this box to make forward Vector3.up.")]
    [SerializeField] bool forwardIsUp = false;

    [SerializeField] BuffManager.Style style;
    [SerializeField] BuffManager.DamageType damageType;
    [SerializeField] List<StatusEffect> secondaryEffects;
    [SerializeField] GameObject onHitPS;
    [SerializeField] bool canHitMultipleTimes = false;

    public int hitboxNumber = 0;

    [Header("These are for special cases. \n Damage is usually set according to the damage in CardSO unless unique damage is checked.")]
    public bool uniqueDamage = false;
    public float damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DamageScript damageScript))
        {
            if (!associatedHitbox.IsTouching(collision)) { return; }

            if (onHitPS != null)
            {
                Instantiate(onHitPS, transform.position, Quaternion.identity);
            }

            damageScript.Hit(damage, CalculateKB(collision.transform.position), secondaryEffects, style, gameObject, hitboxNumber, canHitMultipleTimes);
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

    public void RefreshHitbox()
    {
        hitboxNumber += 1;
    }
}
