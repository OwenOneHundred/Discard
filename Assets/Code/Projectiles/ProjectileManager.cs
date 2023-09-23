using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// Script that controls various aspects of a certain projectile such as speed, direction, movement, etc.
/// </summary>
public class ProjectileManager : MonoBehaviour
{
    [SerializeField] int speed = 10;

    [SerializeField] Vector2 playerPos;

    [SerializeField] Vector2 direction;

    // Start is called before the first frame update
    void Start()
    {
        playerPos = new Vector2(0,0);

        //Translates the cursor position to World Space and then subracting from player position to obtain the direction of the projectile when spawned.

        Vector2 worldMousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 heading = worldMousePos - playerPos;

        //Normalizes the vector so magnitude doesn't influence projectile speed.

        direction = heading.normalized;

        float theta = (float)Math.Acos(direction.y) * (float)(180/Math.PI);

        if(worldMousePos.x > 0) theta = -theta; 

        transform.Rotate(0,0,theta,Space.World);
    }

    // Update is called once per frame
    void Update()
    {
        projectileMotion();
    }

    public void projectileMotion(){
        transform.Translate(Vector2.up * Time.deltaTime * speed);
    }

    public void destroyOutOfBounds(){
        
    }
}
