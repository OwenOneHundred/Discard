using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public float hp;
    //Indicates if enemy was spawned in at a Spawner
    public bool isLocal;

    private EnemyMind enemyMind;
    private DamageScript damageScript;

    //Time til enemy is destoryed
    public float lengthOfDeathAnim;

    // Start is called before the first frame update
    void Start()
    {
        enemyMind = this.gameObject.GetComponent<EnemyMind>();
        damageScript = this.gameObject.GetComponent<DamageScript>();
    }

    //Does damage to enemy
    public void Damage(float damageAmount)
    {
        hp -= damageAmount;
        if(hp <= 0)
        {
            Death();
        }
    }

    //Kills Enemy
    public void Death()
    {
        enemyMind.Death();

        StartCoroutine("DeathWait");
    }

    IEnumerator DeathWait()
    {
        yield return new WaitForSeconds(lengthOfDeathAnim);

        if(isLocal == false)
        {
            EnemyWorldSpawner.enemyCount--;
        }
        Destroy(this.gameObject);
    }
}
