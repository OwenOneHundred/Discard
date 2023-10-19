using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingProjectile : MonoBehaviour
{
    [Header("Assign a circlecollider2d to serve as the area to search for a target.")]
    [SerializeField] CircleCollider2D detectionRange;

    [SerializeField] int bounces;
    [SerializeField] float turnSpeed = 1;
    [SerializeField] float speed;
    [SerializeField] GameObject onBouncePS;

    Rigidbody2D rb;

    Collider2D currentTarget;
    float despawnTimer = 0;
    List<GameObject> alreadyHit = new List<GameObject>();

    ContactFilter2D filter = new ContactFilter2D().NoFilter();

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Execute());
    }

    IEnumerator Execute()
    {
        while (despawnTimer < 10 && bounces >= 0)
        {
            rb.velocity = transform.up * speed;

            if (currentTarget == null)
            {
                currentTarget = SearchForTarget();
                despawnTimer += Time.deltaTime;
            }
            else
            {
                float angle = Mathf.Atan2(currentTarget.bounds.center.y - transform.position.y, currentTarget.bounds.center.x - transform.position.x) * Mathf.Rad2Deg;
                angle -= 90;
                Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            }

            yield return null;
        }

        yield return null;
        Destroy(gameObject);
    }

    IEnumerator Bounce()
    {
        yield return null;
        currentTarget = SearchForTarget();
        Destroy(Instantiate(onBouncePS, transform.position, Quaternion.identity), 5);
        if (currentTarget == null)
        {
            Destroy(gameObject, 0.05f);
        }
        else
        {
            float angle;
            angle = Mathf.Atan2(currentTarget.bounds.center.y - transform.position.y, currentTarget.bounds.center.x - transform.position.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        }
        bounces -= 1;
    }

    Collider2D SearchForTarget()
    {
        List<Collider2D> results = new List<Collider2D>();
        Physics2D.OverlapCollider(detectionRange, filter, results);
        float lowestDistance = 666;
        Collider2D toReturn = null;
        foreach (Collider2D coll in results)
        {
            if (coll.CompareTag("Enemy") && !alreadyHit.Contains(coll.gameObject))
            {
                float distanceAway = Vector2.Distance(transform.position, coll.transform.position);
                if (distanceAway < lowestDistance)
                {
                    lowestDistance = distanceAway;
                    toReturn = coll;
                    despawnTimer = 0;
                }    
            }
        }

        return toReturn;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == currentTarget && Vector2.Distance(transform.position, collision.transform.position) < detectionRange.bounds.extents.x / 3) // technically this is REALLY shit code, because extreme lag would cause this projectile to despawn immediately when moving toward its target, but that probably wont happen
        {
            alreadyHit.Add(collision.gameObject);
            StartCoroutine(Bounce());
        }
    }

}
