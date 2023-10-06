using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementTest : MonoBehaviour
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
    private Vector2 movement;

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

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
        movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        anim.SetFloat("Speed", movement.magnitude);

        if (movement != Vector2.zero)
        {
            anim.SetFloat("XMove", movement.x);
            anim.SetFloat("YMove", movement.y);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetTrigger("Attack");
        }
    }

    //60 times per second
    void FixedUpdate()
    {
        rb.velocity = movement * speed;
    }
}
