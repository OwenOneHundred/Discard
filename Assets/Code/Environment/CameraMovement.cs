using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Transform player;
    private void Start()
    {
        player = GameObject.Find("Player").transform;
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(player.position.x, player.position.y - 2, -10);
    }
}
