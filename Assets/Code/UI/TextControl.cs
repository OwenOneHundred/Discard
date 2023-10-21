using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class TextControl : MonoBehaviour
{
    [SerializeField] List<Sprite> numberSprites;
    [SerializeField] float spacing = 0.5f;
    [SerializeField] GameObject numberPrefab;

    public List<Sprite> GetSpritesFromNumber(int number)
    {
        List<Sprite> toReturn = new List<Sprite>();
        foreach (char num in number + "")
        {
            toReturn.Add(numberSprites[num - '0']);
        }
        return toReturn;
    }

    public void CreateDamageText(Vector2 position, Color color, int number)
    {
        List<Sprite> sprites = GetSpritesFromNumber(number);
        GameObject newParent = new GameObject("Number");
        string stringVer = number.ToString();

        if (stringVer.Length % 2 == 0)
        {
            for (int i = 0; i < number.ToString().Length; i++)
            {
                int order = IndexToOrder(i, stringVer.Length);
                GameObject newNumber = Instantiate(numberPrefab, position + new Vector2(spacing * (order + (order < 0 ? 0.5f : -0.5f)), 0), Quaternion.identity, newParent.transform);
                newNumber.GetComponent<SpriteRenderer>().sprite = sprites[i];
                newNumber.GetComponent<SpriteRenderer>().color = color;
            }
        }
        else
        {
            for (int i = 0; i < number.ToString().Length; i++)
            {
                GameObject newNumber = Instantiate(numberPrefab, position + new Vector2(spacing * IndexToOrder(i, stringVer.Length), 0), Quaternion.identity, newParent.transform);
                newNumber.GetComponent<SpriteRenderer>().sprite = sprites[i];
                newNumber.GetComponent<SpriteRenderer>().color = color;
            }
        }

        Destroy(newParent, 2.5f);
    }




    int IndexToOrder(int index, int count) // returns the number's number relative to the center number. For example, with 7 numbers, the 2nd card is -2.
    {
        if (count % 2 == 0)
        {
            index++;
            return index + (index >= Mathf.CeilToInt(count / 2) + 1 ? 1 : 0) - ((count / 2) + 1);
        }
        else
        {
            return index - (Mathf.CeilToInt(count / 2));
        }
    }
}
