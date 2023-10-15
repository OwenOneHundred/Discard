using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    // inputs
    [SerializeField] bool swingDirection;
    int swingDirectionInt;

    [SerializeField] Collider2D hitbox;

    [SerializeField] float windUpTime;
    [SerializeField] float windUpDegrees;
    [SerializeField] float swingTime;
    [SerializeField] float swingDegrees;
    [SerializeField] float endPauseTime;
    [SerializeField] GameObject endEffect;

    // for use in code
    float startDegrees;
    float angleToMouse;

    // Start is called before the first frame update
    void Start()
    {
        angleToMouse = GeneralUtil.AngleBetween(Vector2.up, Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);

        startDegrees = transform.rotation.eulerAngles.z + angleToMouse;
        swingDirectionInt = swingDirection ? 1 : -1;
        StartCoroutine(Swing());
    }

    IEnumerator Swing()
    {
        float timer = 0;
        while (timer < windUpTime)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, startDegrees - (Mathf.Lerp(0, windUpDegrees, timer/windUpTime) * swingDirectionInt)));
            yield return null;
        }
        timer = 0;
        startDegrees = transform.rotation.eulerAngles.z;
        hitbox.enabled = true;
        while (timer < swingTime)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, startDegrees + (Mathf.Lerp(0, swingDegrees + windUpDegrees, timer / swingTime) * swingDirectionInt)));
            yield return null;
        }
        hitbox.enabled = false;
        yield return new WaitForSeconds(endPauseTime);
        if (endEffect != null)
        {
            Destroy(Instantiate(endEffect, transform.GetChild(0).position, Quaternion.identity), 3f);
        }
        Destroy(gameObject);
    }
}
