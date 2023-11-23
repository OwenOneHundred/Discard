using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectionManager : MonoBehaviour
{
    [SerializeField] GameObject cardOption;
    List<GameObject> cardOptions = new List<GameObject>();
    HandManager hm;

    bool menuOpen = false;

    private void Start()
    {
        hm = transform.root.GetComponentInChildren<HandManager>();
    }

    public void CreateCardSelectionMenu(List<Card> cards)
    {
        if (menuOpen )

        if (cards == null) { return; }
        if (cards.Count == 0) { return; }

        gameObject.SetActive(true);

        int count = cards.Count;
        int budgetEnum = 0;
        foreach (Card i in cards)
        {
            GameObject newCardOption = Instantiate(cardOption, transform.position, Quaternion.identity, transform);
            newCardOption.GetComponent<CardSelectionUI>().SetCardOption(i);
            cardOptions.Add(newCardOption);

            int indexToOrder = IndexToOrder(budgetEnum, count);
            float spacePerCard = 250 / count;

            if (count % 2 == 0)
            {
                newCardOption.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(0, 1.5f) + new Vector2((spacePerCard * indexToOrder) + ((spacePerCard / 2) * (indexToOrder > 0 ? -1 : 1)), 0);
            }
            else
            {
                newCardOption.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(0, 1.5f) + new Vector2(spacePerCard * indexToOrder, 0);
            }

            budgetEnum += 1;
        }
    }

    public void OnCardSelected(Card card)
    {
        hm.AddCardToDeck(card);
        CloseMenu();
    }

    public void CloseMenu()
    {
        foreach (GameObject i in cardOptions)
        {
            Destroy(i);
        }
        cardOptions.Clear();

        gameObject.SetActive(false);
    }

    public void Skip()
    {
        CloseMenu();
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
