using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    [SerializeField] float outTime;
    [SerializeField] float speed;
    [SerializeField] float turnSpeed;
    [SerializeField] float spinSpeed;
    [SerializeField] float despawnDistance = 0.75f;

    [SerializeField] Rigidbody2D rb;
    GameObject player;

    Vector2 outDirection;
    float timer;

    private void Start()
    {
        outDirection = transform.up;
        player = GameObject.Find("Player");
        rb.angularVelocity = spinSpeed;

        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        rb.velocity = outDirection * speed;
        while (timer < outTime)
        {
            timer += Time.deltaTime;

            yield return null;
        }

        while (Vector2.Distance(player.transform.position, transform.position) > despawnDistance)
        {
            rb.velocity = Vector2.MoveTowards(rb.velocity, (player.transform.position - transform.position).normalized * speed, turnSpeed * Time.deltaTime);
            yield return null;
        }

        Destroy(gameObject);

    }
}
