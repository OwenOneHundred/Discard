using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TallGrassClipper : MonoBehaviour
{
    [SerializeField] SpriteMask spriteMask;
    Transform spritemaskTransform;
    Vector3 initialPos;
    public bool inBush = false;
    PlayerMovement pm;
    [SerializeField] float speed;

    void Start()
    {
        pm = GetComponent<PlayerMovement>();
        spritemaskTransform = spriteMask.transform;
        initialPos = spritemaskTransform.position;
    }

    private void Update()
    {
        if (inBush)
        {
            spritemaskTransform.localPosition -= new Vector3(pm.movement.x * pm.Speed * Time.deltaTime * speed, 0, 0);

            if (spritemaskTransform.localPosition.x > 1)
            {
                spritemaskTransform.localPosition = new Vector3(-1, initialPos.y, 0);
            }
            else if (spritemaskTransform.localPosition.x < -1)
            {
                spritemaskTransform.localPosition = new Vector3(1, initialPos.y, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TallGrass"))
        {
            spriteMask.enabled = true;
            inBush = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("TallGrass"))
        {
            spriteMask.enabled = false;
            inBush = false;
        }
    }
}
