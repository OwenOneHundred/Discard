using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeBomb : MonoBehaviour
{
    // this doesn't handle hitboxes at all, that's expected to be handled by explosions
    // at the moment time (Sunday, Oct 15, 1am) there are no hitboxes on time bombs.

    [SerializeField] List<Sprite> timerSpritesHitboxSizes;

    [SerializeField] float time;

    [SerializeField] GameObject explosion;
    [SerializeField] float explosionLifetime = 1;

    [SerializeField] float speed;

    float timer = 0;
    SpriteRenderer sr;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        StartCoroutine(Countdown());
        GetComponent<Rigidbody2D>().AddForce((Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized * speed);
    }

    IEnumerator Countdown()
    {
        while (timer < time)
        {
            int value = Mathf.FloorToInt(timerSpritesHitboxSizes.Count * (timer / time));

            sr.sprite = timerSpritesHitboxSizes[value];

            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(Instantiate(explosion, transform.position, Quaternion.identity), explosionLifetime);
        Destroy(gameObject);
    }
}
