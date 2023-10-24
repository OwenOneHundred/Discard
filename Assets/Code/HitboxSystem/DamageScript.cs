using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// parent for scripts attached to enemies that handle being damaged
public class DamageScript : MonoBehaviour
{
    public List<SecondaryEffectInfo> seInfo = new List<SecondaryEffectInfo>();
    List<Pair<GameObject, int>> alreadyHit = new List<Pair<GameObject, int>>();
    public Rigidbody2D rb;
    public float kbMultiplier = 1;
    public float health = 100;
    GameObject gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
    }

    // available to be overwritten.
    public virtual void OnReduceHealth(float damage)
    {
        health -= damage;

    }

    void DealDamage(float damage)
    {
        gameManager.GetComponent<TextControl>().CreateDamageText(transform.position, Color.red, Mathf.FloorToInt(damage));
        OnReduceHealth(damage);
    }

    // called by HitboxManager when this object touches a hitbox
    // returns if the hit was registered or ignored
    public bool Hit(List<StatusEffect> newEffects, float damage, Vector3 knockback, BuffManager.Style cardStyle, GameObject attacker, int hitboxNum, bool ignoreAlreadyHit = false)
    {
        if (!ignoreAlreadyHit)
        {
            // if already hit by this hurtbox and number, return
            var existing = alreadyHit.Find(x => x.left == attacker);
            if (existing != null) // if already hit by this hitbox
            {
                if (existing.right == hitboxNum) // if same number
                {
                    return false;
                }
                existing.right = hitboxNum; // otherwise set number to new number and continue
            }
            else
            {
                // add hitbox to already hit list
                if (attacker != null)
                {
                    alreadyHit.Add(new Pair<GameObject, int>(attacker, hitboxNum));
                }
            }
        }

        // check list and remove if null (for storage reasons)
        List<Pair<GameObject, int>> temp = alreadyHit.Where(x => x.left == null).ToList();
        foreach (Pair<GameObject, int> toRemove in temp) { alreadyHit.Remove(toRemove); }

        if (newEffects != null)
        {
            foreach (StatusEffect newEffect in newEffects)
            {
                TryAddSecondaryEffect(newEffect);
            }
        }

        DealDamage(damage);
        rb.AddForce(knockback * kbMultiplier);
        return true;
    }

    // called by OnHit when a secondary effect is added. Doesn't add the effect if there's an existing effect with more 
    protected bool TryAddSecondaryEffect(StatusEffect newEffect)
    {
        SecondaryEffectInfo existingSEInfo = seInfo.Find(x => x.effect == newEffect);
        if (existingSEInfo == null)
        {
            newEffect.OnApply(gameObject);
            seInfo.Add(new SecondaryEffectInfo(newEffect, newEffect.time));
            return true;
        }

        // if tried applying effect but effect already existed, if timer is longer on new version, apply new version
        StatusEffect existingEffect = existingSEInfo.effect;
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
            // reduce health by value returned by EveryFrame
            health -= info.effect.EveryFrame(gameObject);

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
        public SecondaryEffectInfo(StatusEffect effect1, float timer1)
        {
            effect = effect1; timer = timer1;
        }

        public StatusEffect effect;
        public float timer;
    }
}
