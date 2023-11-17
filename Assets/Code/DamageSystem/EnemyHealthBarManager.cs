using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DamageScript))]
public class EnemyHealthBarManager : MonoBehaviour
{
    [SerializeField] GameObject healthBarPrefab;
    [SerializeField] float height = 20;
    [SerializeField] float healthPerDivider;
    [SerializeField] GameObject divider;
    [SerializeField] int maxDividers = 10;
    [SerializeField] float barWidth;
    [SerializeField] float barOffset;
    Slider slider;

    private void Start()
    {
        GameObject worldSpaceCanvas = GameObject.Find("WorldSpaceCanvas");
        slider = Instantiate(healthBarPrefab, transform.position + (Vector3.up * height), Quaternion.identity, worldSpaceCanvas.transform).GetComponent<Slider>();
        SetUp(GetComponent<DamageScript>().health, slider.gameObject);
        slider.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        slider.transform.position = transform.position + (Vector3.up * height);
    }

    /// <summary>
    /// Changes slider value. Enter a value from 0 to 1, where 1 is full health.
    /// </summary>
    public void OnHit(float value)
    {
        slider.value = value;
        if (!slider.gameObject.activeInHierarchy)
        {
            slider.gameObject.SetActive(true);
        }
    }

    public void SetUp(float maxHealth, GameObject healthBarObj)
    {
        slider.maxValue = maxHealth;
        slider.value = maxHealth;
        int dividerCount = Mathf.Clamp(Mathf.FloorToInt(maxHealth / healthPerDivider), 0, maxDividers);
        float distanceBetweenDividers = barWidth / (dividerCount + 1);
        Debug.Log("divider count: " + dividerCount);
        for (int i = 1; i < dividerCount + 1; i++)
        {
            RectTransform newDividerRT = Instantiate(divider, healthBarObj.transform).GetComponent<RectTransform>();
            newDividerRT.transform.SetSiblingIndex(2);
            newDividerRT.anchoredPosition = new Vector2((distanceBetweenDividers * i) - (barWidth / 2) + barOffset, 5);
        }
    }

    public void Death()
    {
        Destroy(slider.gameObject);
    }
}
