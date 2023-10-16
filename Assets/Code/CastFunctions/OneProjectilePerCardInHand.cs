using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardFunctions/OneProjectileForEachCardInHand")]
public class OneProjectilePerCardInHand : CastFunctionABS
{
    [SerializeField] float timeBetween;
    [SerializeField] GameObject shuriken;
    [SerializeField] float speed;
    [SerializeField] float spinSpeed;
    [SerializeField] float randomAngleChange;

    public override void Cast(GameObject card)
    {
        if (pt == null)
        {
            pt = GameObject.FindGameObjectWithTag("Player").transform;
        }
        if (hm == null)
        {
            hm = FindAnyObjectByType<HandManager>();
        }
        pt.GetComponent<PlayerMovement>().StartCoroutine(Spawner()); // stupid line
    }

    IEnumerator Spawner()
    {
        int cardCount = hm.Hand.Count;
        for (int i = 0; i < cardCount; i++)
        {
            GameObject newShuriken = Instantiate(shuriken, pt.position, pt.rotation); // GetChild(0) ???
            Rigidbody2D projRB = newShuriken.GetComponent<Rigidbody2D>();
            Vector3 normalizedDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - pt.position).normalized;

            projRB.velocity = Quaternion.AngleAxis(Random.Range(-randomAngleChange, randomAngleChange), Vector3.forward) * normalizedDirection * speed;
            projRB.angularVelocity = spinSpeed;
            Destroy(newShuriken, 6);
            yield return new WaitForSeconds(timeBetween);
        }
    }
}
