using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoverManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] HandManager hm;
    bool selected = false;
    Vector2 relativePointOfContact;
    [SerializeField] float growRate = 1;


    private void Awake()
    {
        if (hm == null)
        {
            hm = transform.GetComponentInParent<HandManager>();
        }
    }

    private void Update()
    {
        if (transform.localScale.x < 1)
        {
            transform.localScale += new Vector3(1, 1, 0) * Time.deltaTime * growRate;
            transform.localScale = new Vector3(Mathf.Clamp(transform.localScale.x, 0, 1), Mathf.Clamp(transform.localScale.y, 0, 1), 1);
        }
    }

    public void LateUpdate()
    {
        if (selected)
        {
            transform.position = (Vector2)Input.mousePosition + relativePointOfContact;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hm.HoverCard(gameObject);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hm.UnhoverCard(gameObject);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        hm.SelectCard(gameObject);
        selected = true;
        hm.UnhoverCard(gameObject);
        relativePointOfContact = transform.position - Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        hm.DeselectCard(gameObject);
        hm.HoverCard(gameObject);
        selected = false;
        relativePointOfContact = Vector2.zero;
    }
}
