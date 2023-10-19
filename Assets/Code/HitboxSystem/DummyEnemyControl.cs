using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DummyEnemyControl : DamageScript
{
    public override void ReduceHealth(float damage)
    {
        health -= damage;
    }

}
