using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// parent for scripts attached to enemies that handle being damaged
public class DamageScript : MonoBehaviour
{
    List<SecondaryEffect> secondaryEffects = new List<SecondaryEffect>();
    List<GameObject> alreadyHit = new List<GameObject>();
    public Rigidbody2D rb;
    public float kbMultiplier = 1;
    public float health = 100; // test

    // available to be overwritten.
    public virtual void ReduceHealth(float damage)
    {
        health -= damage;
    }

    // called by HitboxManager when this object touches a hitbox
    public void OnHit(List<SecondaryEffect> newEffects, float damage, Vector3 knockback, Card.Style cardStyle, GameObject attacker, bool canHitMultTimes = false)
    {
        if (!canHitMultTimes)
        {
            if (alreadyHit.Contains(attacker)) { return; }
            List<GameObject> temp = alreadyHit.Where(x => x == null).ToList();
            foreach (GameObject toRemove in temp) { alreadyHit.Remove(toRemove); }
            alreadyHit.Add(attacker);
        }

        foreach (SecondaryEffect newEffect in newEffects)
        {
            TryAddSecondaryEffect(newEffect);
        }

        ReduceHealth(damage);
        rb.AddForce(knockback * kbMultiplier);
    }

    // called by OnHit when a secondary effect is added. Doesn't add the effect if there's an existing effect with more 
    protected bool TryAddSecondaryEffect(SecondaryEffect newEffect)
    {
        newEffect.OnApply();

        SecondaryEffect existingEffect = secondaryEffects.Find(x => x == newEffect);
        if (existingEffect == null)
        {
            secondaryEffects.Add(newEffect);
            return true;
        }
        if (existingEffect.time < newEffect.time)
        {
            secondaryEffects.Add(newEffect);
            return true;
        }

        return false;
    }

    protected void UpdateEffects()
    {
        if (secondaryEffects.Count == 0) { return; }
        List<SecondaryEffect> seCopy = new List<SecondaryEffect>(secondaryEffects);
        foreach (SecondaryEffect effect in seCopy)
        {
            effect.EveryFrame();
            if (effect.time <= 0)
            {
                effect.OnEnd();
                secondaryEffects.Remove(effect);
            }
        }
    }

    // reduces timers on secondary effects and calls their EveryFrame functions
    // not passed to children; must be added manually
    private void Update()
    {
        UpdateEffects();
    }

}
