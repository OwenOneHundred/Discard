using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetScript : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController defaultController;
    GameObject player;
    BuffManager.Style style;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        switch (style)
        {
            case BuffManager.Style.launch:
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                break;
            case BuffManager.Style.homing:
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                break;
            case BuffManager.Style.melee:
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                break;
            case BuffManager.Style.pinpoint:
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                break;
            default:
                transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                break;
        }
    }


    // do not use this yet, it doesn't actually work because it doesn't affect the animator
    public void ChangeAnimator(RuntimeAnimatorController newController, BuffManager.Style newStyle)
    {   
        style = newStyle;

        if (newController == null)
        {
            anim.runtimeAnimatorController = defaultController;
        }
        else
        {
            anim.runtimeAnimatorController = newController;
        }
    }
}
