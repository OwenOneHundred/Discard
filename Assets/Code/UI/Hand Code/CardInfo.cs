using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    public Card scriptableObject;
    BuffManager bm;
    TMPro.TextMeshProUGUI tmpro;

    public float actualDamage = 0;

    private void Start()
    {
        bm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BuffManager>();
        tmpro = transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>();

        transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = scriptableObject.name;
        transform.GetChild(1).GetComponent<Image>().sprite = scriptableObject.art;

        tmpro.text = scriptableObject.description;

        UpdateDamage();
    }

    // Very important function. Sets all the damages for the hitboxes on this card. Called on card drawn in HandManager and when buff effects added or removed.
    public void UpdateDamage()
    {
        if (bm == null) { bm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BuffManager>(); }
        if (tmpro == null) { tmpro = transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>(); ; }

        actualDamage = scriptableObject.baseDamage * bm.GetStyleMultiplier(scriptableObject.style) * bm.GetDamageTypeMultiplier(scriptableObject.damageType);

        string text = scriptableObject.description;
        int firstIndex = text.IndexOf('#');
        if (firstIndex == -1) { return; }

        int index = firstIndex;
        int lastIndex = index + 1;

        text = text.Replace("#", (int)actualDamage + "");
        text = text.Insert(firstIndex - 1, "<color=#" + ColorUtility.ToHtmlStringRGB(bm.GetStyleColor(scriptableObject.style)) + ">");
        text = text.Insert(lastIndex + 14 + (actualDamage + "").Length, "</color>");

        tmpro.text = text;
    }

}