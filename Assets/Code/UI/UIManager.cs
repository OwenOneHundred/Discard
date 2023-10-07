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

    [SerializeField] GameObject discardEffect;
    [SerializeField] Vector2 discardEffectOffset;
    [SerializeField] GameObject pileBurstEffect;
    [SerializeField] Vector2 drawShuffleEffectOffset;

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
        GameObject newPileBurstEffect = Instantiate(pileBurstEffect, drawPileText.transform.parent.parent);
        Destroy(newPileBurstEffect, 0.2f);
    }

    public void PlayDiscardAnim(GameObject card)
    {
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
