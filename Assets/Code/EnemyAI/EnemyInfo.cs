using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public int hp;

    //Does damage to enemy
    public void Damage(int damageAmount)
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
        Debug.Log("Enemy Death");
    }
}
