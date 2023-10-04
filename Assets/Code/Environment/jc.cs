using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.position += new Vector3 (Camera.main.transform.position.x - 15,0 ,0);
    }

    // Update is called once per frame
    float resetTimer;
    float timer;
    
    void Update()
    {
        if (transform.position.x < Camera.main.transform.position.x + 15)
        {
            transform.position += new Vector3 (10,0,0) * Time.deltaTime;
        } 
        else
        {
            timer+= Time.deltaTime;
            if (timer > resetTimer){
                timer = 0;
                resetTimer = Random.Range(4, 15);
                transform.position = new Vector3 (Camera.main.transform.position.x - 15,Camera.main.transform.position.y + Random.Range(-8,8) ,0);
            }
            // transform.position = new Vector3 (Camera.main.transform.position.x - 15,0 ,0);
            
        }

    }
}
