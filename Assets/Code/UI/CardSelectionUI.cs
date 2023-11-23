using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    GameObject outline;
    TMPro.TextMeshProUGUI textObj;
    GameObject cardObj;
    Card assignedCard;

    // must be called when menu is opened
    public void SetCardOption(Card card)
    {
        textObj.text = card.name;
        assignedCard = card;
        cardObj.GetComponent<CardInfo>().scriptableObject = card;
        cardObj.GetComponent<CardInfo>().SetUp();
    }

    private void Awake()
    {
        outline = transform.GetChild(0).gameObject;
        textObj = transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        cardObj = transform.GetChild(2).gameObject;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Chosen();
    }

    void Chosen()
    {
        transform.parent.GetComponent<CardSelectionManager>().OnCardSelected(assignedCard);
        Debug.Log("chosen card: " + assignedCard);
    }
}
