using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public float hp;

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

    void Update()
    {
        if(Input.GetKey(KeyCode.Z))
        {
            Death();
        }
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

        Destroy(this.gameObject);
    }
}
