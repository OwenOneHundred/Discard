using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMind : MonoBehaviour
{
    private GameObject player;

    //Enemy Info Scripts
    private EnemyInfo enemyInfo;
    private EnemyMovement enemyMovement;
    private EnemyGun enemyGun;

    //Distance to player
    public float distToPlayer;

    //Enemy States
    public bool aggroed;
    public bool inRange;

    //Distance Goals
    public float agroeDistance;
    public float pacifyDistance;
    public float rangeDistance;


    // Start is called before the first frame update
    void Awake()
    {
        //Gets Player
        player = GameObject.FindGameObjectWithTag("Player");

        //Gets Info
        enemyInfo = this.gameObject.GetComponent<EnemyInfo>();

        enemyMovement = this.gameObject.GetComponent<EnemyMovement>();
        enemyMovement.atObjective = true;
        enemyMovement.objective = player.transform;

        enemyGun = this.gameObject.GetComponent<EnemyGun>();
        enemyGun.inRange = false;
        enemyGun.player = player.transform;
    }

    // Update is called once per frame
    void Update()
    {
        distToPlayer = GetDist();

        //Attacking
        if(aggroed == true)
        {
            if(distToPlayer >= pacifyDistance)
            {
                aggroed = false;
            }
            else
            {
                //Activates or Deactives Gun
                if(distToPlayer < rangeDistance)
                {
                    inRange = true;
                    enemyGun.inRange = true;
                }
                else
                {
                    inRange = false;
                    enemyGun.inRange = false;
                }

                //Says if close enough 
                if(inRange == false)
                {
                    enemyMovement.atObjective = false;
                }
                else
                {
                    enemyMovement.atObjective = true;
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
                enemyMovement.atObjective = true;
                enemyGun.inRange = false;
            }
        }

    }

    //Returns the distance between the player and the enemy
    public float GetDist()
    {
        Vector3 currentPos = transform.position;
        float dist = Vector3.Distance(player.transform.position, currentPos);

        return dist;
    }
}
