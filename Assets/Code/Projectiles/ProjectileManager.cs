using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// Script that controls various aspects of a certain projectile such as speed, direction, movement, etc.
/// </summary>
public class ProjectileManager : MonoBehaviour
{
    [SerializeField] int speed = 10;

    [SerializeField] int dmg = 1;

    [SerializeField] Vector2 startingPos;

    Vector2 direction;

    [SerializeField] GameObject explosionEffect;

    [SerializeField] String effectName = "ExplosionEffect(Clone)";

    [SerializeField] float effectLengthInSeconds = 0.667f;

    Rigidbody2D rb;


    void Start()
    {
        setUpTransform();
    }

    void FixedUpdate()
    {
        projectileMotion(); 
    }

    void Update()
    {
        destroyOutOfBounds();
    }

    public void setUpTransform() {
        startingPos = transform.position;

        //Translates the cursor position to World Space and then subracting from player position to obtain the direction of the projectile when spawned.

        Vector2 worldMousePos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 heading = worldMousePos - startingPos;

        //Normalizes the vector so magnitude doesn't influence projectile speed.

        direction = heading.normalized;

        //Calculates amount to rotate projectile in degrees in order to aim towards mouse.

        float theta = (float)Math.Acos(direction.y) * (float)Mathf.Rad2Deg;   //Math.Acos(direction.y)

        if(worldMousePos.x > startingPos.x) theta = -theta; 

        transform.Rotate(0,0,theta,Space.World);

        rb = GetComponent<Rigidbody2D>();
    }

    public void projectileMotion(){
        //transform.Translate(Vector2.up * Time.deltaTime * speed);

        rb.MovePosition(rb.position + direction * Time.fixedDeltaTime * speed);
    }

    //Destroy Projectile outside of screen. Buffer is 0.2 by default but can be modified in method call.
    public void destroyOutOfBounds(float buffer = 0.2f){
        Vector2 screenPos = Camera.main.WorldToViewportPoint(transform.position);

        float xPos = screenPos.x;

        float yPos = screenPos.y;

        if((xPos < 0 - buffer || yPos < 0 - buffer) || (xPos > 1 + buffer ||  yPos > 1 + buffer)) Destroy(gameObject);
    }

    //Replaces fireball with explosion on collision
    public void OnTriggerEnter2D(Collider2D c) {
        
        Debug.Log("Triggered");

        GameObject obj = c.gameObject;

        string cTag = obj.tag;

        if(!cTag.Equals("Player")){
            Instantiate(explosionEffect,transform.GetChild(0).position,transform.rotation);

            Destroy(GameObject.Find(effectName),effectLengthInSeconds);

            Destroy(gameObject);
        }
        
        if(cTag.Equals("Enemy")){
            obj.GetComponent<EnemyInfo>().hp -= dmg;
        }
    }
}
