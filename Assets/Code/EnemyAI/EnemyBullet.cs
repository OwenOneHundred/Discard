using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public int damage;
    public float speed;

    public float timeRemove;

    public bool dontDestroy;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("RemoveBullet");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    IEnumerator RemoveBullet()
    {
        yield return new WaitForSeconds(timeRemove);

        Destroy(this.gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Barrier")
        {
            if (dontDestroy == false)
            {
                Destroy(this.gameObject);
            }
        }

        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerInfo>().Damage(damage);
            if (dontDestroy == false)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
