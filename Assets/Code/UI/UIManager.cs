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
    }

    void ChangeEnergyBarNumbers()
    {
        energyBarImage.sprite = energyBarNumbers[Mathf.FloorToInt(energyBarSlider.value)];
    }
}
