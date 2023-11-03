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
    [SerializeField] BuffManager.StatusEffect damageType;
    [System.NonSerialized] public List<StatusEffect> statusEffects = new List<StatusEffect>();
    [SerializeField] List<StatusEffect> statusEffectData;

    [SerializeField] GameObject onHitEffect;
    [SerializeField] float effectLifetime = 1;
    [SerializeField] bool canHitMultipleTimes = false;

    [System.NonSerialized] public int hitboxNumber = 0;

    public float damage;

    private void Awake()
    {
        foreach (StatusEffect i in statusEffectData)
        {
            statusEffects.Add(Instantiate(i));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DamageScript damageScript))
        {
            if (!associatedHitbox.IsTouching(collision)) { return; }

            if (onHitEffect != null)
            {
                Destroy(Instantiate(onHitEffect, collision.transform.position, Quaternion.identity), effectLifetime);
            }

            damageScript.Hit(damage, CalculateKB(collision.transform.position), statusEffects, style, gameObject, hitboxNumber, canHitMultipleTimes);
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
