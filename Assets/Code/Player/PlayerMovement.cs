using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //The speed at which the player moves through space
    [SerializeField]
    private float speed;
    //Allows for getting and setting speed 
    public float Speed
    {
        get { return speed; }

        set { speed = value; }
    }

    //Movement Stuff
    private Rigidbody2D rb;
    public Vector2 movement;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();

        //Example of Changing speed of Player
        /*
        Debug.Log(speed); //outputs 5.0
        speed = 2f;
        Debug.Log(speed); //outputs 2.0
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //60 times per second
    void FixedUpdate()
    {
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        rb.velocity = movement * speed;
    }
}
