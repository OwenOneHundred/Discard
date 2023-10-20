using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosionField : MonoBehaviour
{
    // this is not really what this thing is supposed to do so this is basically a placeholder

    [SerializeField] float openTime;
    float timer;
    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(WaitToClose());
    }

    IEnumerator WaitToClose()
    {
        while (timer < openTime)
        {
            timer += Time.deltaTime;

            yield return null;
        }
        anim.SetTrigger("Close");
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }

}
