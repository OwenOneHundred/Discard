using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour
{
    public Card scriptableObject;

    private void Start()
    {
        transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = scriptableObject.name;
        transform.GetChild(1).GetComponent<Image>().sprite = scriptableObject.art;
        transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = scriptableObject.description;
    }
}
