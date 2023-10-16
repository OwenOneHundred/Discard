using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// File that contains all deck and hand management functions.
///
/// Also manages the important variables selectedCard, hoveredCard, deck, hand, discardPile, and drawPile.
/// </summary>

public class HandManager : MonoBehaviour
{
    [SerializeField] UIManager uim;

    [SerializeField] CardOrganizer co;

    [SerializeField] float cardPlayHeight = 4;
    [SerializeField] int maxHandSize = 7;
    [SerializeField] int initialHandSize = 3;

    protected GameObject selectedCard;
    public GameObject SelectedCard
    {
        get { return selectedCard; }
        set
        {
            if (selectedCard != value)
            {
                selectedCard = value;
                co.selectedCard = value;
                if (selectedCard != null) { co.selectedCardRectTransform = value.GetComponent<RectTransform>(); }
            }
        }
    }

    private GameObject hoveredCard;

    private int originalSiblingIndex;

    [SerializeField] private List<GameObject> hand = new List<GameObject>();
    public List<GameObject> Hand
    {
        get { return hand; }
        set
        {
            if (hand != value)
            {
                hand = value;
            }
        }
    }

    [SerializeField] private List<GameObject> deck = new List<GameObject>();
    public List<GameObject> Deck
    {
        get { return deck; }
        set
        {
            if (deck != value)
            {
                deck = value;
            }
        }
    }

    [SerializeField] private List<GameObject> drawPile = new List<GameObject>();
    public List<GameObject> DrawPile
    {

        get { return drawPile; }
        set
        {
            if (drawPile != value)
            {
                uim.DrawPileCount = value.Count() + "";
                drawPile = value;
            }
        }
    }

    [SerializeField] private List<GameObject> discardPile = new List<GameObject>();
    public List<GameObject> DiscardPile
    {
        get { return discardPile; }
        set
        {
            if (discardPile != value)
            {
                uim.DiscardPileCount = value.Count() + "";
                discardPile = value;
            }
        }
    }

    private static System.Random rng = new System.Random();

    private void Start()
    {
        ResetDrawPile();
        while (Hand.Count < initialHandSize && DrawPile.Count > 0)
        {
            DrawCard();
        }

        co.UpdateOrganizedCards(Hand, Hand.IndexOf(hoveredCard));

        uim.CDBar.onFill += DrawBarFull;
    }

    void ResetDrawPile()
    {
        drawPile = new List<GameObject>(Deck);
    }

    public bool DrawCard()
    {
        if (Hand.Count >= maxHandSize) { return false; }

        if (DrawPile.Count == 0)
        {
            ShuffleDiscardPileIntoDrawPile();
        }

        if (DrawPile.Count > 0)
        {
            GameObject drawnCard = DrawPile[0];

            PrepareCardToBeDrawn(drawnCard);

            Hand.Add(drawnCard);
            drawnCard.SetActive(true);
            DrawPile = RemoveAndReturn(DrawPile, drawnCard);
            co.UpdateOrganizedCards(Hand, hand.IndexOf(hoveredCard));

            drawnCard.transform.SetAsLastSibling();
            return true;
        }
        return false;

        static void PrepareCardToBeDrawn(GameObject card)
        {
            card.GetComponent<RectTransform>().anchoredPosition = new Vector3(-130, 0, 0);
            card.transform.localScale = new Vector3(0.25f, 0.25f, 1);
        }
    }

    public void DiscardCard(GameObject card)
    {
        Debug.Log("discard called on: " + card.name);
        DiscardPile = AddAndReturn(DiscardPile, card);

        card.SetActive(false);

        Hand.Remove(card);
        uim.PlayDiscardAnim(card);

        co.UpdateOrganizedCards(Hand, hand.IndexOf(hoveredCard));
    }

    void DrawBarFull(SlowBarFiller sbf) // called by action in update
    {
        if (DrawCard())
        {
            sbf.slider.value = 0;
        }
    }

    void ShuffleDiscardPileIntoDrawPile()
    {
        if (DiscardPile.Count > 0)
        {
            DrawPile = (DrawPile.Concat(DiscardPile)).OrderBy(a => rng.Next()).ToList();
            DiscardPile = new List<GameObject>();
            uim.PlayDiscardShuffleAnim();
        }
    }

    void PlayCard(GameObject card)
    {
        int cardCost = Int32.Parse(card.GetComponent<CardInfo>().scriptableObject.cost);

        if (uim.energyBar.slider.value > cardCost)
        {
            card.GetComponent<CardInfo>().scriptableObject.OnPlayed(card);

            // Instantiate(card.GetComponent<CardInfo>().scriptableObject.attackPrefab, GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);

            uim.energyBar.slider.value -= cardCost;

            DiscardCard(card);
        }
        else
        {
            // not enough energy
        }
    }

    public void HoverCard(GameObject card)
    {
        if (selectedCard != null) return;

        originalSiblingIndex = card.transform.GetSiblingIndex();
        card.transform.SetAsLastSibling();
        hoveredCard = card;
        co.UpdateOrganizedCards(Hand, hand.IndexOf(card));
    }

    public void UnhoverCard(GameObject card)
    {
        if (hoveredCard != null)
        {
            if (selectedCard != hoveredCard)
            {
                hoveredCard.transform.SetSiblingIndex(originalSiblingIndex);
            }
        }

        hoveredCard = null;
        co.UpdateOrganizedCards(Hand, 666);
    }

    // called when a card is clicked
    public void SelectCard(GameObject card)
    {
        SelectedCard = card;
        co.UpdateOrganizedCards(Hand, 666);
    }

    // called when a card is released
    public void DeselectCard(GameObject card)
    {
        SelectedCard = null;

        if (Input.mousePosition.y > cardPlayHeight)
        {
            PlayCard(card);
        }
        else
        {
            co.UpdateOrganizedCards(Hand, 666);
        }
    }

    List<T> RemoveAndReturn<T>(List<T> list, T toRemove)
    {
        List<T> temp = new List<T>(list);
        temp.Remove(toRemove);
        return temp;
    }

    List<T> AddAndReturn<T>(List<T> list, T toAdd)
    {
        return new List<T>(list) { toAdd };
    }
}
