using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMind : MonoBehaviour
{
    //Used for positioing
    private GameObject player;
    private Vector3 idlePos;

    //Enemy Info Scripts
    private EnemyInfo enemyInfo;
    private EnemyMovement enemyMovement;
    private EnemyGun enemyGun;
    private EnemyImager enemyImager;
    private BoxCollider2D thisBoxCollider;

    //Distance to player
    public float distToPlayer;
    public float angleToPlayer;

    //Enemy States
    public bool aggroed;
    public bool inRange;

    //Distance Goals
    public float agroeDistance;
    public float pacifyDistance;
    public float attackDistance;


    // Start is called before the first frame update
    void Awake()
    {
        //Gets Player
        player = GameObject.FindGameObjectWithTag("Player");
        idlePos = new Vector3(transform.position.x, transform.position.y, 0f);

        //Gets Info
        enemyInfo = this.gameObject.GetComponent<EnemyInfo>();

        enemyMovement = this.gameObject.GetComponent<EnemyMovement>();
        enemyMovement.enemyMind = this.gameObject.GetComponent<EnemyMind>();
        enemyMovement.atObjective = true;
        enemyMovement.objectiveV3 = idlePos;
        enemyMovement.player = player;

        enemyGun = this.gameObject.GetComponent<EnemyGun>();
        enemyGun.inRange = false;
        enemyGun.player = player.transform;

        enemyImager = this.gameObject.GetComponent<EnemyImager>();

        thisBoxCollider = this.gameObject.GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        distToPlayer = GetDist(player.transform.position);

        //Attacking
        if(aggroed == true)
        {
            DetermineAngle();
            enemyMovement.objective = player.transform;
            if(distToPlayer >= pacifyDistance)
            {
                aggroed = false;
            }
            else
            {
                //Activates or Deactives Gun
                if(distToPlayer <= attackDistance)
                {
                    inRange = true;
                    enemyGun.inRange = true;
                }
                else if(enemyGun.currentlyFiring == false)
                {
                    inRange = false;
                    enemyGun.inRange = false;
                    enemyGun.currentAttackPhase = 1;
                    enemyImager.isAttacking = false;
                }

                //Says if close enough 
                if(inRange == false)
                {
                    enemyMovement.atObjective = false;
                    if (enemyImager.isMoving == false)
                    {
                        enemyImager.isMoving = true;
                        enemyImager.ResetAnim();
                    }
                }
                else
                {
                    enemyMovement.atObjective = true;
                    if (enemyImager.isMoving == true)
                    {
                        enemyImager.isMoving = false;
                        enemyImager.ResetAnim();
                    }
                }
            }
        }
        //Not Attacking
        else
        {
            //Starts Agroe
            if(distToPlayer <= agroeDistance)
            {
                aggroed = true;
            }
            else
            {
                //Move to Idle Pos
                enemyMovement.objective = null;
                enemyGun.inRange = false;

                //Check if at Idle Pos
                if(GetDist(idlePos) < .5f)
                {
                    enemyMovement.atObjective = true;
                    if (enemyImager.isMoving == true)
                    {
                        enemyImager.isMoving = false;
                        enemyImager.ResetAnim();
                    }
                }
                else
                {
                    DetermineAngle();
                    enemyMovement.atObjective = false;
                    if (enemyImager.isMoving == false)
                    {
                        enemyImager.isMoving = true;
                        enemyImager.ResetAnim();
                    }
                }
            }
        }

    }

    //Returns the distance between the player and the enemy
    public float GetDist(Vector3 target)
    {
        Vector3 currentPos = transform.position;
        float dist = Vector3.Distance(target, currentPos);

        return dist;
    }

    //Gets the angle that is the target to the player
    public void DetermineAngle()
    {
        Vector3 difference = player.transform.position - transform.position;
        float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        //Facing South 
        if(angle <= -45f && angle >= -135)
        {
            enemyImager.direction = 0;
        }
        //Facing North
        else if (angle >= 45f && angle <= 135)
        {
            enemyImager.direction = 1;
        }
        //Facing East
        else if (angle < 45f && angle > -45f)
        {
            enemyImager.direction = 2;
        }
        //Facing West
        else
        {
            enemyImager.direction = 3;
        }
    }

    //Does the death stuff of the enemy
    public void Death()
    {
        enemyImager.isDead = true;
        enemyImager.ResetAnim();

        //Disable Collider
        thisBoxCollider.enabled = false;
        //Disable Movement
        enemyMovement.enabled = false;
        //Disable Atatcks
        enemyGun.enabled = false;
    }

    //Freezes enemy attack and enemy imager
    public void Freeze(bool isFrozen)
    {
        //Disable Atatcks
        enemyGun.enabled = isFrozen;
        //Disables Anim
        enemyImager.canAnimate = isFrozen;
    }
}
