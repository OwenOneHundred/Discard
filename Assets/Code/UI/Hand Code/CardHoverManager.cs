using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardHoverManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, ICanvasRaycastFilter
{
    [SerializeField] HandManager hm;
    public bool interactable = false;

    public bool IsRaycastLocationValid(Vector2 screenPos, Camera eventCamera)
    {
        return interactable;
    }

    private void Awake()
    {
        if (hm == null)
        {
            hm = transform.GetComponentInParent<HandManager>();
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
        hm.UnhoverCard(gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!hm.DeselectCard(gameObject))
        {
            hm.HoverCard(gameObject);
        }
    }
}
