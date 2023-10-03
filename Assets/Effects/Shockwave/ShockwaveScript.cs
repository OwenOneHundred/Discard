using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveScript : MonoBehaviour
{
    [SerializeField] float lifetime = 1;
    float timeValue = 0;
    float timer;
    public Material mat;

    private void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        timeValue = timer / lifetime;

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
        else
        {
            mat.SetFloat("_WaveDistFromCenter", Mathf.Lerp(-0.1f, 0.5f, timeValue));
            Debug.Log(mat.GetFloat("_WaveDistFromCenter"));
        }
    }
}
