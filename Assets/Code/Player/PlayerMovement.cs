using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [System.Serializable]
    public class SpriteAnimation
    {
        public Sprite[] sprites;
    }

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

    //Used for Player Anim
    public SpriteRenderer playerRenderer;
    public SpriteAnimation[] walkAnim;
    public int timePerFrame;
    private int currentTime;
    private int currentFrame;
    private int dir;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
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

        //Runs Time Stuff For Anim
        currentTime++;
        if(currentTime >= timePerFrame)
        {
            currentTime = 0;
            currentFrame++;
            if(currentFrame >= walkAnim[0].sprites.Length)
            {
                currentFrame = 0;
            }

            bool idle = false;
            //Determines if Idle
            if (movement.x == 0f && movement.y == 0f)
            {
                idle = true;
            }
            else
            {
                dir = GetDirection();
            }

            Animate(idle, dir);
        }
    }

    //Aniamtes the Player Moving
    //Dir: 0-South, 1-North, 2-East, 3-West
    public void Animate(bool isIdle, int dir)
    {
        //Checks if Idle
        if(isIdle == true)
        {
            playerRenderer.sprite = walkAnim[dir].sprites[0];
        }
        else
        {
            playerRenderer.sprite = walkAnim[dir].sprites[currentFrame];
        }
    }

    //Gets the direction the player should angle towards
    public int GetDirection()
    {
        if (movement.y == 1f)
        {
            return 1;
        }
        else if (movement.y == -1f)
        {
            return 0;
        }
        else if (movement.x == 1f)
        {
            return 2;
        }
        else 
        {
            return 3;
        }
    }

}
