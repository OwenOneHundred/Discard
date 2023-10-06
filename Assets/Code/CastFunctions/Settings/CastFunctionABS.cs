using UnityEngine;

public abstract class CastFunctionABS : ScriptableObject
{
    public static HandManager hm;
    private void Awake()
    {
        if (hm == null)
        {
            hm = GameObject.Find("Hand").GetComponent<HandManager>();
        }
    }

    public virtual void Cast()
    {

    }
}
