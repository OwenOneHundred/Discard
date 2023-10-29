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
    [SerializeField] BuffManager bm;

    [SerializeField] CardOrganizer co;

    [SerializeField] float cardPlayHeight = 4;
    [SerializeField] int maxHandSize = 7;
    [SerializeField] int initialHandSize = 3;
    [SerializeField] GameObject targetObject;
    [SerializeField] RectTransform discardPilePos;
    [SerializeField] RectTransform drawPilePos;
    TargetScript ts;

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
                else { co.selectedCardRectTransform = null; }
            }
        }
    }

    public GameObject hoveredCard;

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

        ts = targetObject.GetComponent<TargetScript>();
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
            StartCoroutine(DrawCardAnimation(drawnCard));

            drawnCard.transform.SetAsLastSibling();

            foreach (GameObject go in Hand)
            {
                go.GetComponent<CardInfo>().UpdateDamage();
            }

            return true;
        }
        return false;

        void PrepareCardToBeDrawn(GameObject card)
        {
            card.GetComponent<RectTransform>().anchoredPosition = new Vector3(-130, 0, 0);
            card.transform.localScale = new Vector3(0.25f, 0.25f, 1);
        }
    }

    public void DiscardCard(GameObject card)
    {
        DiscardPile = AddAndReturn(DiscardPile, card);

        Hand.Remove(card);
        uim.PlayDiscardAnim(card);

        StartCoroutine(DiscardCardAnimation(card));

        co.UpdateOrganizedCards(Hand, hand.IndexOf(hoveredCard));

        foreach (GameObject go in Hand)
        {
            go.GetComponent<CardInfo>().UpdateDamage();
        }
    }

    IEnumerator DiscardCardAnimation(GameObject card)
    {
        CardInfo ci = card.GetComponent<CardInfo>();
        RectTransform crt = card.GetComponent<RectTransform>();

        card.GetComponent<CardHoverManager>().interactable = false;
        while (Vector2.Distance(crt.position, discardPilePos.position) > 0.01f)
        {
            // if you drew this card instantly, stop this animation
            if (hand.Contains(card)) { yield break; }

            // move toward discard pile
            crt.position = Vector2.MoveTowards(crt.position, discardPilePos.position, 3200f * Time.deltaTime);

            // if not totally shrunk yet, shrink
            if (crt.localScale.x > 0.25f)
            {
                crt.localScale -= 3 * Time.deltaTime * new Vector3(1, 1, 0);
                crt.localScale = new Vector3(Mathf.Clamp(crt.localScale.x, 0, 1), Mathf.Clamp(crt.localScale.y, 0, 1), 1);
            }

            yield return null;
        }

        // disable
        card.SetActive(false);
    }

    IEnumerator DrawCardAnimation(GameObject card)
    {
        CardInfo ci = card.GetComponent<CardInfo>();
        RectTransform crt = card.GetComponent<RectTransform>();
        while (crt.localScale.x < 1)
        {
            crt.localScale += 3 * Time.deltaTime * new Vector3(1, 1, 0);
            crt.localScale = new Vector3(Mathf.Clamp(crt.localScale.x, 0, 1), Mathf.Clamp(crt.localScale.y, 0, 1), 1);
            yield return null;
        }
        card.GetComponent<CardHoverManager>().interactable = true;
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
        CardInfo cardInfo = card.GetComponent<CardInfo>();
        int cardCost = Int32.Parse(cardInfo.scriptableObject.cost);

        if (uim.energyBar.slider.value >= cardCost)
        {
            if (cardInfo.scriptableObject.isBoomerang) { bm.consecutiveBoomerangs += 1; }
            else { bm.consecutiveBoomerangs = 0; }

            cardInfo.scriptableObject.OnPlayed(card);

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
        CardInfo cardScript = card.GetComponent<CardInfo>();
        co.UpdateOrganizedCards(Hand, 666);
        targetObject.SetActive(true);
        ts.ChangeAnimator(cardScript.scriptableObject.targetAnimator, cardScript.scriptableObject.style);
    }

    // called when a card is released, returns if it was played
    public bool DeselectCard(GameObject card)
    {
        bool toReturn = false;
        SelectedCard = null;
        targetObject.SetActive(false);

        if (Input.mousePosition.y > cardPlayHeight)
        {
            PlayCard(card);
            toReturn = true;
        }

        co.UpdateOrganizedCards(Hand, 666);
        return toReturn;
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
