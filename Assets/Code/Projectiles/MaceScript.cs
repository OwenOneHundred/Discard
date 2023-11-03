using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MeleeWeapon;

public class MaceScript : MonoBehaviour
{
    [SerializeField] GameObject endEffect;
    [SerializeField] Collider2D hitbox;
    [SerializeField] HitboxManager hitboxManager;
    [SerializeField] AudioSource aus;
    [SerializeField] Rigidbody2D rb;

    [SerializeField] float windUpTime;
    [SerializeField] float windUpDegrees;
    [SerializeField] float swingTime;
    [SerializeField] int swingDirection1orNeg1;
    [SerializeField] AudioClip swingSound;
    [SerializeField] float endPauseTime;

    [SerializeField] bool doNotDisableHitbox = false;

    // for use in code
    float startDegrees;
    float angleToMouse;
    Transform weaponTransform;

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = GameObject.FindGameObjectWithTag("Player").transform;
        angleToMouse = GeneralUtil.AngleBetween(Vector2.up, Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        weaponTransform = transform.GetChild(0);

        startDegrees = transform.rotation.eulerAngles.z + angleToMouse;
        StartCoroutine(Execute());
    }

    IEnumerator Execute()
    {
        // winding up
        float timer = 0;
        while (timer < windUpTime)
        {
            timer += Time.deltaTime;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, startDegrees - (Mathf.Lerp(0, windUpDegrees, timer / windUpTime) * swingDirection1orNeg1)));
            yield return null;
        }

        int count = GameObject.FindAnyObjectByType<HandManager>().Deck.Count / 2;
        for (int i = 0; i < count; i++)
        {
            // swinging
            timer = 0;
            startDegrees = transform.rotation.eulerAngles.z;
            hitbox.enabled = true;
            hitboxManager.hitboxNumber += 1;
            if (swingSound != null) { aus.PlayOneShot(swingSound); }
            while (timer < swingTime)
            {
                timer += Time.deltaTime;

                transform.rotation = Quaternion.Euler(new Vector3(0, 0, startDegrees + (Mathf.Lerp(0, 360 + windUpDegrees, timer / swingTime) * swingDirection1orNeg1)));
                yield return null;
            }

            // Stop
            if (!doNotDisableHitbox)
            {
                hitbox.enabled = false;
            }
        }
        yield return new WaitForSeconds(endPauseTime);

        if (endEffect != null)
        {
            Destroy(Instantiate(endEffect, weaponTransform.position, Quaternion.identity), 3f);
        }
        Destroy(gameObject);
    }
}
