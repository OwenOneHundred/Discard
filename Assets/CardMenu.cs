using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class CardMenu : MonoBehaviour
{
    [SerializeField] GameObject cardVisualPrefab;
    [SerializeField] Transform cardVisualParent;
    [SerializeField] RectTransform cvpRT;
    [SerializeField] HandManager hm;

    [SerializeField] List<float> xPositions;
    [SerializeField] float yStartHeight;
    [SerializeField] float yHeightDifference;

    [SerializeField] List<Toggle> costTogglesNotAll;
    [SerializeField] List<Toggle> collectionToggles;
    [SerializeField] Toggle allToggle;

    [SerializeField] List<Sprite> collectionSprites;
    [SerializeField] Image collectionNameImage;

    List<GameObject> cardVisuals = new List<GameObject>();
    float topScrollCap = -200;

    [SerializeField] float scrollScale = 10;

    List<int> selectedCardCosts = new List<int>() { 0, 1, 2, 3 };
    Collection activeCollection = Collection.deck;
    private Collection ActiveCollection
    {
        get { return activeCollection; }
        set
        {
            if (activeCollection == value) { return; }
            collectionNameImage.sprite = collectionSprites[(int) value];
            activeCollection = value;
        }
    }

    bool open = false;
    public bool Open
    {
        get { return open; }
        set
        {
            if (open == value) { return; }
            open = value;
            if (open) { OnOpen(); }
            else { OnClose(); }
        }
    }

    private void Update()
    {
        float scrollDelta = -Input.mouseScrollDelta.y;
        if (scrollDelta != 0)
        {
            cvpRT.anchoredPosition += new Vector2(0, scrollDelta * scrollScale);
            cvpRT.anchoredPosition = new Vector2(0, Mathf.Clamp(cvpRT.anchoredPosition.y, -10, topScrollCap));
        }
    }

    public enum Collection
    {
        deck, hand, drawPile, discardPile
    }

    void GenerateCardVisuals(List<Card> cards)
    {
        cvpRT.anchoredPosition = Vector2.zero;

        foreach (GameObject i in cardVisuals)
        {
            Destroy(i);
        }
        cardVisuals.Clear();

        int columnCount = 0;
        int rowCount = 0;

        foreach (Card card in cards)
        {
            if (columnCount >= 4)
            {
                columnCount = 0;
                rowCount += 1;
            }

            RectTransform newCardRT = Instantiate(cardVisualPrefab, cardVisualParent).GetComponent<RectTransform>();
            cardVisuals.Add(newCardRT.gameObject);

            newCardRT.anchoredPosition = new Vector2(xPositions[columnCount], yStartHeight - (yHeightDifference * rowCount));

            CardInfo ci = newCardRT.GetComponent<CardInfo>();
            ci.scriptableObject = card;
            ci.SetUp();

            columnCount += 1;
        }

        topScrollCap = Mathf.Ceil(cardVisuals.Count / 4f) * 40;
    }

    public void ChangeSelectedCardCosts(int cost)
    {
        if (selectedCardCosts.Contains(cost))
        {
            selectedCardCosts.Remove(cost);

            if (allToggle.isOn) { allToggle.isOn = false; }
        }
        else
        {
            selectedCardCosts.Add(cost);

            int onNum = 0;
            foreach (Toggle i in costTogglesNotAll)
            {
                if (i.isOn)
                {
                    onNum += 1;
                }
            }
            if (onNum == costTogglesNotAll.Count)
            {
                allToggle.isOn = true;
            }
        }

        GenerateCardVisuals(GetVisibleCards());
    }

    public void EnableAllCardCosts() // called when the toggle is clicked or enabled/disabled
    {
        if (allToggle.isOn) // you just clicked the toggle on
        {
            foreach (Toggle i in costTogglesNotAll)
            {
                i.isOn = true;
            }
            selectedCardCosts = new List<int>() { 0, 1, 2, 3 };
        }
        else // you just clicked the toggle off
        {
            foreach (Toggle i in costTogglesNotAll)
            {
                if (!i.isOn) { return; }
            }

            foreach (Toggle i in costTogglesNotAll)
            {
                i.isOn = false;
            }
            selectedCardCosts.Clear();
        }

        GenerateCardVisuals(GetVisibleCards());
    }

    public void ChangeSelectedCollection(int newCollection)
    {
        if (newCollection > 3 || newCollection < 0) { return; } 
        Toggle thisToggle = collectionToggles[newCollection];

        if (thisToggle.isOn)
        {
            ActiveCollection = (Collection) newCollection;
            foreach (Toggle i in collectionToggles)
            {
                if (i != thisToggle)
                {
                    i.isOn = false;
                }
            }
            GenerateCardVisuals(GetVisibleCards());
        }
        else
        {
            int count = 0;
            foreach (Toggle i in collectionToggles)
            {
                if (!i.isOn)
                {
                    count += 1;
                }    
            }
            if (count == collectionToggles.Count)
            {
                ActiveCollection = Collection.deck;
                GenerateCardVisuals(GetVisibleCards());
            }
        }
    }

    public void OnOpen()
    {
        gameObject.SetActive(true);
        GenerateCardVisuals(GetVisibleCards());
    }

    public void OnClose()
    {
        foreach (GameObject i in cardVisuals)
        {
            Destroy(i);
        }
        cardVisuals.Clear();
        gameObject.SetActive(false);
    }

    public void IncreaseCollectionEnum()
    {
        if ((int) ActiveCollection == 3)
        {
            collectionToggles[0].isOn = true;
            ChangeSelectedCollection(0);
        }
        else
        {
            collectionToggles[(int)activeCollection + 1].isOn = true;
            ChangeSelectedCollection((int) activeCollection + 1);
        }
    }

    public void DecreaseCollectionEnum()
    {
        if ((int)ActiveCollection == 0)
        {
            collectionToggles[3].isOn = true;
            ChangeSelectedCollection(3);
        }
        else
        {
            collectionToggles[(int)activeCollection - 1].isOn = true;
            ChangeSelectedCollection((int)activeCollection - 1);
        }
    }

    private List<Card> GetVisibleCards()
    {
        List<Card> toReturn = new List<Card>();
        List<Card> collection;
        if (ActiveCollection == Collection.deck) { collection = hm.Deck.Select(o => o.GetComponent<CardInfo>().scriptableObject).ToList(); }
        else if (ActiveCollection == Collection.hand) { collection = hm.Hand.Select(o => o.GetComponent<CardInfo>().scriptableObject).ToList(); }
        else if (ActiveCollection == Collection.drawPile) { collection = hm.DrawPile.Select(o => o.GetComponent<CardInfo>().scriptableObject).ToList(); }
        else if (ActiveCollection == Collection.discardPile) { collection = hm.DiscardPile.Select(o => o.GetComponent<CardInfo>().scriptableObject).ToList(); }
        else { collection = new List<Card>(); }

        foreach (Card card in collection)
        {
            if (selectedCardCosts.Contains(int.Parse(card.cost)))
            {
                toReturn.Add(card);
            }
        }

        return toReturn;
    }
}
