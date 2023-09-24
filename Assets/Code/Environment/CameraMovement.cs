using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class CameraMovement : MonoBehaviour
{
    Transform player;
    private void Start()
    {
            //player = GameObject.Find("Player").transform;
    }

    private void LateUpdate()
    {
        //if(transform.position != null) {
        //    transform.position = new Vector3(player.position.x, player.position.y - 2, -10);
        //} else {
            transform.position = new Vector3(0,0,-10);
        //}
    }
}
