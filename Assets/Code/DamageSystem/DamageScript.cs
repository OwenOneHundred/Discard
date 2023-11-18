using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

// parent for scripts attached to enemies that handle being damaged
public class DamageScript : MonoBehaviour
{
    public List<StatusEffect> activeStatusEffects = new List<StatusEffect>();
    List<Pair<GameObject, int>> alreadyHit = new List<Pair<GameObject, int>>();
    public Rigidbody2D rb;
    public float kbMultiplier = 1;
    public float health = 100;
    public bool isLocal;
    GameObject gameManager;
    EnemyHealthBarManager ehbm;

    //Stuff used for Death
    //Time til enemy is destoryed
    public float lengthOfDeathAnim;
    private EnemyMind enemyMind;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
        ehbm = GetComponent<EnemyHealthBarManager>();
        enemyMind = GetComponent<EnemyMind>();
    }

    // available to be overwritten.
    public virtual void OnReduceHealth(float damage)
    {
        health -= damage;
        if (ehbm != null) { ehbm.OnHit(health); }
        if (health <= 0){
            enemyMind.Death();
            StartCoroutine("DeathWait");
        }
    }

    void DealDamage(float damage, bool doNotSpawnNumber = false)
    {
        if (!doNotSpawnNumber)
        {
            gameManager.GetComponent<TextControl>().CreateDamageText(transform.position, Color.red, Mathf.FloorToInt(damage));
        }
        OnReduceHealth(damage);
    }

    // called by HitboxManager when this object touches a hitbox
    // returns if the hit was registered or ignored
    public bool Hit(float damage = 0, Vector3 knockback = default, List<StatusEffect> newEffects = null, BuffManager.Style cardStyle = BuffManager.Style.None, GameObject attacker = null, int hitboxNum = 0, bool ignoreAlreadyHit = false, bool doNotSpawnNumber = false)
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
                TryAddStatusEffect(newEffect);
            }
        }

        DealDamage(damage, doNotSpawnNumber);
        rb.AddForce(knockback * kbMultiplier);
        return true;
    }

    // called by OnHit when a secondary effect is added. Doesn't add the effect if there's an existing effect with more 
    protected bool TryAddStatusEffect(StatusEffect newEffect)
    {
        StatusEffect existingEffect = activeStatusEffects.Find(x => x.effectType == newEffect.effectType);
        if (existingEffect == null)
        {
            newEffect.OnApply(gameObject);
            activeStatusEffects.Add(Instantiate(newEffect));
            return true;
        }

        // if tried applying effect but effect already existed, if timer is longer on new version, apply new version
        if (existingEffect.timer < newEffect.timer)
        {
            existingEffect.timer = newEffect.timer;
            return true;
        }

        return false;
    }

    protected void UpdateEffects()
    {
        if (activeStatusEffects.Count == 0) { return; }
        List<StatusEffect> activeEffectsCopy = new List<StatusEffect>(activeStatusEffects);
        foreach (StatusEffect se in activeEffectsCopy)
        {
            // reduce health by value returned by EveryFrame
            float damageRecorded = se.EveryFrame(gameObject);
            if (damageRecorded > 0)
            {
                Hit(damage: damageRecorded, doNotSpawnNumber: true);
            }

            if (se.timer <= 0)
            {
                se.OnEnd(gameObject);
                activeStatusEffects.Remove(se);
            }
            se.timer -= Time.deltaTime;
        }
    }

    // reduces timers on secondary effects and calls their EveryFrame functions
    // not passed to children; must be added manually
    public virtual void Update()
    {
        UpdateEffects();
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnReduceHealth(25f);
        }
    }

    //The time need to wait for death animation
    IEnumerator DeathWait()
    {
        yield return new WaitForSeconds(lengthOfDeathAnim);
        if (isLocal == false)
        {
            EnemyWorldSpawner.enemyCount--;
        }
        ehbm.Death();
        Destroy(this.gameObject);
    }
}
