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
    public BuffManager.Style style;
    public BuffManager.StatusEffect damageType;
    BuffManager bm;
    public bool isBoomerang;

    public float baseDamage;

    [Header("Leave blank for default.")]
    public RuntimeAnimatorController targetAnimator = null;

    public List<CastFunctionAbstract> castFunctions;

    public void OnPlayed(GameObject card)
    {
        foreach (CastFunctionAbstract func in castFunctions)
        {
            func.Cast(card);
        }
    }
}