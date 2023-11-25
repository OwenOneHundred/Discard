using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public SlowBarFiller CDBar;
    public SlowBarFiller energyBar;
    [SerializeField] Image energyBarImage;
    Slider energyBarSlider;
    [SerializeField] TMPro.TextMeshProUGUI drawPileText;
    [SerializeField] TMPro.TextMeshProUGUI discardPileText;
    [SerializeField] RectTransform drawPileRT;
    [SerializeField] RectTransform discardPileRT;
    [SerializeField] RectTransform healthBarRT;
    [SerializeField] RectTransform energyBarRT;

    [SerializeField] GameObject discardEffect;
    [SerializeField] Vector2 discardEffectOffset;
    [SerializeField] GameObject pileBurstEffect;
    [SerializeField] Vector2 drawShuffleEffectOffset;

    public GameObject cardSelectionUI;

    [SerializeField] List<Sprite> energyBarNumbers;

    [SerializeField] List<BarCap> barCaps;

    public string DrawPileCount
    {
        get { return drawPileText.text; }
        set
        {
            if (drawPileText.text != value)
            {
                drawPileText.text = value;
            }
        }
    }

    public string DiscardPileCount
    {
        get { return discardPileText.text; }
        set
        {
            if (discardPileText.text != value)
            {
                discardPileText.text = value;
            }
        }
    }

    public void PlayDiscardShuffleAnim()
    {
        if (!pileBurstEffect) { return; }
        GameObject newPileBurstEffect = Instantiate(pileBurstEffect, drawPileText.transform.parent.parent);
        Destroy(newPileBurstEffect, 0.2f);
    }

    public void PlayDiscardAnim(GameObject card)
    {
        if (!pileBurstEffect) { return; }
        GameObject newPileBurstEffect = Instantiate(pileBurstEffect, discardPileText.transform.parent.parent);
        Destroy(newPileBurstEffect, 0.2f);
    }

    private void Start()
    {
        energyBarSlider = energyBar.GetComponent<Slider>();
    }

    private void Update()
    {
        ChangeEnergyBarNumbers();
        MoveBarCaps();
    }

    void ChangeEnergyBarNumbers()
    {
        energyBarImage.sprite = energyBarNumbers[Mathf.FloorToInt(energyBarSlider.value)];
    }

    void MoveBarCaps()
    {
        foreach (BarCap i in barCaps)
        {
            float percentFilled = i.slider.value / i.slider.maxValue;
            if (i.rightToLeft)
            {
                i.cap.anchoredPosition = new Vector3(Mathf.Lerp(i.barRect.offsetMax.x, i.barRect.offsetMin.x, percentFilled) + i.offset.x, i.cap.anchoredPosition.y, 0);
            }
            else
            {
                i.cap.anchoredPosition = new Vector3(Mathf.Lerp(i.barRect.offsetMin.x, i.barRect.offsetMax.x, percentFilled) + i.offset.x, i.cap.anchoredPosition.y, 0);
            }
        }
    }

    public bool MoveUIOnscreen()
    {
        drawPileRT.anchoredPosition = Vector2.MoveTowards(drawPileRT.anchoredPosition, Vector2.zero, 100 * Time.deltaTime);
        discardPileRT.anchoredPosition = Vector2.MoveTowards(discardPileRT.anchoredPosition, Vector2.zero, 100 * Time.deltaTime);

        healthBarRT.anchoredPosition = Vector2.MoveTowards(healthBarRT.anchoredPosition, Vector2.zero, 100 * Time.deltaTime);
        energyBarRT.anchoredPosition = Vector2.MoveTowards(energyBarRT.anchoredPosition, Vector2.zero, 100 * Time.deltaTime);

        return Vector2.Distance(discardPileRT.anchoredPosition, Vector2.zero) < 0.0001f;
    }

    public bool MoveUIOffscreen()
    {
        drawPileRT.anchoredPosition = Vector2.MoveTowards(drawPileRT.anchoredPosition, new Vector2(0, -40), 100 * Time.deltaTime);
        discardPileRT.anchoredPosition = Vector2.MoveTowards(discardPileRT.anchoredPosition, new Vector2(0, -40), 100 * Time.deltaTime);

        healthBarRT.anchoredPosition = Vector2.MoveTowards(healthBarRT.anchoredPosition, new Vector2(0, 30), 100 * Time.deltaTime);
        energyBarRT.anchoredPosition = Vector2.MoveTowards(energyBarRT.anchoredPosition, new Vector2(0, 30), 100 * Time.deltaTime);

        return Vector2.Distance(discardPileRT.anchoredPosition, new Vector2(0, -40)) < 0.0001f;
    }



    [System.Serializable]
    public class BarCap
    {
        public Vector2 offset;
        public Slider slider;
        public RectTransform barRect;
        public RectTransform cap;
        public bool rightToLeft = false;
    }
}
