using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SecondaryEffect : ScriptableObject
{
    public enum EffectType
    {
        Fire, Slow, Freeze
    }

    public EffectType effectType;
    public float time = 5;
    public bool canStack = false;
    public int stackNumber = 1;

    public virtual void OnApply(GameObject enemy)
    {

    }

    public virtual void EveryFrame(GameObject enemy)
    {

    }

    public virtual void OnEnd(GameObject enemy)
    {

    }
}
