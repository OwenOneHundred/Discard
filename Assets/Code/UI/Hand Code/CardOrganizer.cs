using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// File that moves cards in hand smoothly. UpdateOrganizedCards should be called when the list of organized cards should change. Fully autonomous otherwise.
/// </summary>

public class CardOrganizer : MonoBehaviour
{
    List<GameObject> organizedCards = new List<GameObject>();
    List<RectTransform> organizedCardRects = new List<RectTransform>();
    [SerializeField] RectTransform cardGlow;
    private int hoveredCardIndex = 666;
    public GameObject selectedCard = null;
    public RectTransform selectedCardRectTransform = null;
    [SerializeField] float additionalTilt = 15;
    [SerializeField] float edgeDecline = 0.03f;
    [SerializeField] float cardWidth = 15;
    [SerializeField] float cardspeed = 10;
    [SerializeField] float rotationSpeed = 1;
    [SerializeField] Vector2 hoveredAdjustment;
    [SerializeField] Vector2 center;

    // This function is a combination of 3 operations that could be separate functions. This is for simplicity. It updates the card list, the hovered card, and the selected card.
    // It should be called to initialize hand. Should also be called:
    // Any time cards are moved into or out of the hand
    // Any time a card is selected or deselected
    // Any time a card is hovered or unhovered
    public void UpdateOrganizedCards(List<GameObject> allCardsInHand, int newHoveredCardIndex)
    {
        organizedCards = new List<GameObject>(allCardsInHand);
        organizedCardRects = organizedCards.Select(o => o.GetComponent<RectTransform>()).ToList();
        hoveredCardIndex = newHoveredCardIndex;

        int budgetEnum = 0;
        foreach (GameObject card in allCardsInHand)
        {
            card.transform.SetSiblingIndex(budgetEnum);

            budgetEnum += 1;
        }

        if (newHoveredCardIndex != 666 && newHoveredCardIndex != -1)
        {
            allCardsInHand[newHoveredCardIndex].transform.SetAsLastSibling();
        }
        if (selectedCard != null)
        {
            cardGlow.gameObject.SetActive(true);
            cardGlow.transform.SetAsLastSibling();

            selectedCard.transform.SetAsLastSibling();
        }
        else if (cardGlow.gameObject.activeInHierarchy)
        {
            cardGlow.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < organizedCards.Count;)
        {
            MoveCard(i, CalculateGoalPos(i));
            i++;
        }

        if (selectedCard != null)
        {
            selectedCard.transform.rotation = Quaternion.Euler(0, 0, Mathf.MoveTowardsAngle(selectedCard.transform.rotation.eulerAngles.z, 0, rotationSpeed * 2));
        }
    }

    void MoveCard(int index, Vector2 goalPosition) // Moves and rotates the card. 
    {
        if (organizedCards[index] == null) return;

        GameObject card = organizedCards[index];

        int indextoorder = IndexToOrder(index, organizedCards.Count);

        // rotate if not selected
        if (selectedCard != organizedCards[index])
        {
            if (card.transform.rotation.eulerAngles.z != -additionalTilt * indextoorder)
            {
                card.transform.rotation = Quaternion.Euler(0, 0, Mathf.MoveTowardsAngle(card.transform.rotation.eulerAngles.z, -additionalTilt * indextoorder, rotationSpeed));
            }
        }
        else
        { // if card is selected, move card glow
            cardGlow.anchoredPosition = selectedCardRectTransform.anchoredPosition;
        }

        // move
        Vector3 cardAnchorPos = organizedCardRects[index].anchoredPosition;
        if ((Vector2)cardAnchorPos != goalPosition)
        {
            organizedCardRects[index].anchoredPosition = Vector2.MoveTowards(cardAnchorPos, goalPosition,
                cardspeed * Vector2.Distance(cardAnchorPos, goalPosition));
        }

    }

    Vector2 CalculateGoalPos(int index) // returns card goal position
    {
        int indextoorder = IndexToOrder(index, organizedCards.Count);

        // bool cardIsSelectedCard = organizedCards.IndexOf(selectedCard) == index;

        if (organizedCards.Count % 2 == 0)
        {
            return
                // first part is the base position. This is center if card is not selected; center + hoveredAdjustment if card is selected
                center +

                (hoveredCardIndex == index ? hoveredAdjustment : Vector2.zero) +

                new Vector2(cardWidth * indextoorder + ((cardWidth / 2) * (indextoorder > 0 ? -1 : 1)),

                -edgeDecline * Mathf.Abs(indextoorder));
        }
        else
        {
            return
                center +

                (hoveredCardIndex == index ? hoveredAdjustment : Vector2.zero) +

                new Vector2(cardWidth * indextoorder,

                -edgeDecline * Mathf.Abs(indextoorder));
        }
    }

    int IndexToOrder(int index, int count) // returns the card's number relative to the center card. For example, with 7 cards, the 2nd card is -2.
    {
        if (count % 2 == 0)
        {
            index++;
            return index + (index >= Mathf.CeilToInt(count / 2) + 1 ? 1 : 0) - ((count / 2) + 1);
        }
        else
        {
            return index - (Mathf.CeilToInt(count / 2));
        }
    }
}