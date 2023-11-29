using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaBuff : MonoBehaviour
{
    [SerializeField] SpellBuff spellBuffPrefab;
    SpellBuff assignedSpellBuff;
    BuffManager bm;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AddBuff();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            RemoveBuff();
        }
    }

    void AddBuff()
    {
        if (bm == null) { bm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BuffManager>(); }
        assignedSpellBuff = Instantiate(spellBuffPrefab);
        bm.AddSpellBuff(assignedSpellBuff);
    }

    void RemoveBuff()
    {
        if (assignedSpellBuff != null)
        {
            bm.RemoveSpellBuff(assignedSpellBuff);
        }
    }

    void OnDestroy()
    {
        RemoveBuff();
    }
}
