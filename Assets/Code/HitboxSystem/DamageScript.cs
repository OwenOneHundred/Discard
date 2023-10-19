using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// parent for scripts attached to enemies that handle being damaged
public class DamageScript : MonoBehaviour
{
    public List<SecondaryEffectInfo> seInfo = new List<SecondaryEffectInfo>();
    List<GameObject> alreadyHit = new List<GameObject>();
    public Rigidbody2D rb;
    public float kbMultiplier = 1;
    public float health = 100;

    // available to be overwritten.
    public virtual void ReduceHealth(float damage)
    {
        health -= damage;
    }

    // called by HitboxManager when this object touches a hitbox
    public void OnHit(List<SecondaryEffect> newEffects, float damage, Vector3 knockback, Card.Style cardStyle, GameObject attacker)
    {
        // if already hit by this hurtbox, return
        if (alreadyHit.Contains(attacker)) { return; }

        // check list and remove if null (for storage reasons)
        List<GameObject> temp = alreadyHit.Where(x => x == null).ToList();
        foreach (GameObject toRemove in temp) { alreadyHit.Remove(toRemove); }

        // add hitbox to already hit list
        alreadyHit.Add(attacker);

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
        SecondaryEffectInfo existingSEInfo = seInfo.Find(x => x.effect == newEffect);
        if (existingSEInfo == null)
        {
            newEffect.OnApply(gameObject);
            seInfo.Add(new SecondaryEffectInfo(newEffect, newEffect.time));
            return true;
        }

        // if tried applying effect but effect already existed, if timer is longer on new version, apply new version
        SecondaryEffect existingEffect = existingSEInfo.effect;
        if (existingSEInfo.timer < newEffect.time)
        {
            existingSEInfo.timer = newEffect.time;
            return true;
        }

        return false;
    }

    protected void UpdateEffects()
    {
        if (seInfo.Count == 0) { return; }
        List<SecondaryEffectInfo> seCopy = new List<SecondaryEffectInfo>(seInfo);
        foreach (SecondaryEffectInfo info in seCopy)
        {
            info.effect.EveryFrame(gameObject);
            if (info.timer <= 0)
            {
                info.effect.OnEnd(gameObject);
                seInfo.Remove(info);
            }
            info.timer -= Time.deltaTime;
        }
    }

    // reduces timers on secondary effects and calls their EveryFrame functions
    // not passed to children; must be added manually
    public virtual void Update()
    {
        UpdateEffects();
    }

    [System.Serializable]
    public class SecondaryEffectInfo
    {
        public SecondaryEffectInfo(SecondaryEffect effect1, float timer1)
        {
            effect = effect1; timer = timer1;
        }

        public SecondaryEffect effect;
        public float timer;
    }

}
