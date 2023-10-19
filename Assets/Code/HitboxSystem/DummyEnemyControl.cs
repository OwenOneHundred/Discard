using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DummyEnemyControl : DamageScript
{
    //  Reduces timers on secondary effects and calls their EveryFrame functions
    private void Update()
    {
        UpdateEffects();
    }

    public override void ReduceHealth(float damage)
    {
        health -= damage;
    }

}
