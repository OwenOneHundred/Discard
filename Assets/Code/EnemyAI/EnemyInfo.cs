using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public int hp;
    public float armor;

    private EnemyMind enemyMind;

    //Time til enemy is destoryed
    public float lengthOfDeathAnim;

    // Start is called before the first frame update
    void Start()
    {
        enemyMind = this.gameObject.GetComponent<EnemyMind>();
    }

    void Update()
    {
        if(Input.GetKey(KeyCode.Z))
        {
            Death();
        }
    }

    //Does damage to enemy
    public void Damage(int damageAmount, bool ignoresDamage)
    {
        if(ignoresDamage == false)
        {
            damageAmount = (int) ((float)damageAmount * armor);
        }

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
