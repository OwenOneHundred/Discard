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

    [SerializeField] List<Sprite> cardBackgrounds;

    private void Start()
    {
        int cost = int.Parse(scriptableObject.cost);
        bm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BuffManager>();
        tmpro = transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>();
        if (cost < 4 && cost >= 0)
        {
            transform.GetChild(1).GetComponent<Image>().sprite = cardBackgrounds[cost];
        }
        transform.GetChild(3).GetComponent<TMPro.TextMeshProUGUI>().text = scriptableObject.name;
        transform.GetChild(0).GetComponent<Image>().sprite = scriptableObject.art;

        tmpro.text = scriptableObject.description;
        UpdateDamage();
    }

    // Very important function. Sets all the damages for the hitboxes on this card. Called on card drawn in HandManager and when buff effects added or removed.
    public void UpdateDamage()
    {
        if (bm == null) { bm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BuffManager>(); }
        if (tmpro == null) { tmpro = transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>();}

        float multplierFromCardFunctions = 1;
        foreach (CastFunctionAbstract i in scriptableObject.castFunctions)
        {
            multplierFromCardFunctions *= i.OnDamageCalculated(gameObject);
        }

        actualDamage = scriptableObject.baseDamage * bm.GetStyleMultiplier(scriptableObject.style) * bm.GetDamageTypeMultiplier(scriptableObject.damageType) * multplierFromCardFunctions;

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