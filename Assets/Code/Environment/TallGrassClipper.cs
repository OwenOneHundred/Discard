using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TallGrassClipper : MonoBehaviour
{
    [SerializeField] GameObject spriteMask;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("TallGrass"))
        {
            spriteMask.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("TallGrass"))
        {
            spriteMask.SetActive(false);
        }
    }
}
