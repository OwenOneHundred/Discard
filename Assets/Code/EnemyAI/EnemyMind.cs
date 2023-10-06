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

    //Distance to player
    public float distToPlayer;

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
        enemyMovement.atObjective = true;
        enemyMovement.objectiveV3 = idlePos;

        enemyGun = this.gameObject.GetComponent<EnemyGun>();
        enemyGun.inRange = false;
        enemyGun.player = player.transform;

        enemyImager = this.gameObject.GetComponent<EnemyImager>();
    }

    // Update is called once per frame
    void Update()
    {
        distToPlayer = GetDist(player.transform.position);

        //Attacking
        if(aggroed == true)
        {
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
                    enemyImager.isAttacking = true;
                }
                else
                {
                    inRange = false;
                    enemyGun.inRange = false;
                    enemyImager.isAttacking = false;
                }

                //Says if close enough 
                if(inRange == false)
                {
                    enemyMovement.atObjective = false;
                    Debug.Log("Hit");
                    enemyImager.isMoving = true;
                }
                else
                {
                    enemyMovement.atObjective = true;
                    enemyImager.isMoving = false;
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
                    enemyImager.isMoving = false;
                }
                else
                {
                    enemyMovement.atObjective = false;
                    enemyImager.isMoving = true;
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
}
