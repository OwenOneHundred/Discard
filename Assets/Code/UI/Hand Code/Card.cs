using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public new string name;
    public string description;
    public Sprite art;
    public string cost;
    public GameObject attackPrefab;

    public List<CastFunctionABS> castFunctions;

    public virtual void OnPlayed()
    {
        foreach (CastFunctionABS func in castFunctions)
        {
            func.Cast();
        }
    }
}