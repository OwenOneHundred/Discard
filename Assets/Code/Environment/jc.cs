using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3 (0, 10, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < 100)
        {
            transform.position += new Vector3 (10,0,0) * Time.deltaTime;
        } 
        else
        {
            transform.position = new Vector3 (0,10,0);
        }

    }
}
