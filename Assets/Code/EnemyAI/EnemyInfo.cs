using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public int hp;
    public float armor;

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
        Debug.Log("Enemy Death");
    }
}
