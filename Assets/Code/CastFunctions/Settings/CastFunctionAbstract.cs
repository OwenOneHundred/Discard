using UnityEngine;

public abstract class CastFunctionAbstract : ScriptableObject
{
    public static HandManager hm;
    public static Transform pt;
    public virtual void Cast(GameObject card)
    {

    }

    // must be called whenever you spawn an object
    protected void SetUpObject(GameObject obj, GameObject card)
    {
        HitboxManager[] hms = obj.transform.root.GetComponentsInChildren<HitboxManager>();
        CardInfo cardInfo = card.GetComponent<CardInfo>();
        BuffManager bm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BuffManager>();

        foreach (HitboxManager hm in hms)
        {
            hm.damage = cardInfo.actualDamage;
        }

        bm.OnProjectileSpawned(obj, card);
    }
}
