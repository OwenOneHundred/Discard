using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    [SerializeField] Sprite defaultSprite;
    GameObject player;
    Card.Style style;
    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        switch (style)
        {
            case Card.Style.launch:
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                break;
            case Card.Style.homing:
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                break;
            case Card.Style.melee:
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                break;
            case Card.Style.pinpoint:
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                break;
            default:
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                break;
        }
    }


    // do not use this yet, it doesn't actually work because it doesn't affect the animator
    public void ChangeSprite(Card.Style newStyle, Sprite sprite)
    {
        style = newStyle;

        if (sprite == null)
        {
            sr.sprite = defaultSprite;
        }
        else
        {
            sr.sprite = sprite;
        }
    }
}
