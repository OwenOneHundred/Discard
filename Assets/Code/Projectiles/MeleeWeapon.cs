using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LootManager;
using static UnityEngine.Rendering.DebugUI;

public class MeleeWeapon : MonoBehaviour
{
    [SerializeField] List<Swing> swings;

    [SerializeField] GameObject endEffect;
    [SerializeField] Collider2D hitbox;

    // for use in code
    float startDegrees;
    float angleToMouse;
    Transform weaponTransform;
    Vector2 startWeaponOffset;
    Vector2 actualOutwardMovement;

    // Start is called before the first frame update
    void Start()
    {
        transform.parent = GameObject.FindGameObjectWithTag("Player").transform;
        angleToMouse = GeneralUtil.AngleBetween(Vector2.up, Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position);
        weaponTransform = transform.GetChild(0);
        startWeaponOffset = weaponTransform.localPosition;

        startDegrees = transform.rotation.eulerAngles.z + angleToMouse;
        StartCoroutine(Execute());
    }

    IEnumerator Execute()
    {
        foreach (Swing swing in swings)
        {
            // winding up
            float timer = 0;
            while (timer < swing.windUpTime)
            {
                timer += Time.deltaTime;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, startDegrees - (Mathf.Lerp(0, swing.windUpDegrees, timer / swing.windUpTime) * swing.swingDirection1orNeg1)));
                yield return null;
            }

            // swinging
            timer = 0;
            startDegrees = transform.rotation.eulerAngles.z;
            hitbox.enabled = true;
            while (timer < swing.swingTime)
            {
                Debug.Log("swingtime: " + swing.swingTime);
                timer += Time.deltaTime;

                if (swing.swingOutwardMovement != Vector2.zero)
                {
                    if (timer < swing.swingTime / 2)
                    {
                        Vector3 adjustedPosition = new Vector3(
                            Mathf.Lerp(startWeaponOffset.x, swing.swingOutwardMovement.x * 2, timer / swing.swingTime),
                            Mathf.Lerp(startWeaponOffset.y, swing.swingOutwardMovement.y * 2, timer / swing.swingTime), 0);
                        weaponTransform.localPosition = adjustedPosition;
                    }
                    else
                    {
                        Vector3 adjustedPosition = new Vector3(
                            Mathf.Lerp(swing.swingOutwardMovement.x * 2, startWeaponOffset.x, timer / swing.swingTime),
                            Mathf.Lerp(swing.swingOutwardMovement.y * 2, startWeaponOffset.y, timer / swing.swingTime), 0);
                        weaponTransform.localPosition = adjustedPosition;
                    }
                }

                transform.rotation = Quaternion.Euler(new Vector3(0, 0, startDegrees + (Mathf.Lerp(0, swing.swingDegrees + swing.windUpDegrees, timer / swing.swingTime) * swing.swingDirection1orNeg1)));
                yield return null;
            }

            // Stop
            hitbox.enabled = false;
            yield return new WaitForSeconds(swing.endPauseTime);
        }

        if (endEffect != null)
        {
            Destroy(Instantiate(endEffect, weaponTransform.position, Quaternion.identity), 3f);
        }
        Destroy(gameObject);
    }

    [System.Serializable]
    public class Swing
    {
        // inputs
        public int swingDirection1orNeg1;

        public float windUpTime;
        public float windUpDegrees;
        public float swingTime;
        public float swingDegrees;
        public float endPauseTime;
        public Vector2 swingOutwardMovement;
    }
}
