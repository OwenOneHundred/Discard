using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public bool atObjective;

    public Transform objective;
    public Vector3 objectiveV3;
    public float speed;
    //0 means no change, 1 means no speed
    private float speedModifer = 0;
    public int modiferCount = 0;

    //-1-Idle, 0-South, 1-North, 2-East, 3-West
    public Transform vectorCheckPoint;
    public LayerMask barrierMask;
    public float distanceToBarrierCheck;

    // Update is called once per frame
    void Update()
    {
        //Moves Enemy Object towards objective if no at it already
        if (atObjective != true && (objective != null || objectiveV3 != null))
        {
            //Checks East direction
            if (NodeCheck(2))
            {
                transform.position += new Vector3(0f, .001f, 0f);
            }
            //Checks West direction
            else if (NodeCheck(3))
            {
                transform.position += new Vector3(0f, .001f, 0f);
            }
            //Checks South direction
            else if (NodeCheck(0))
            {
                transform.position += new Vector3(.001f, 0f, 0f);
            }
            //Checks North direction
            else if (NodeCheck(1))
            {
                transform.position += new Vector3(.001f, 0f, 0f);
            }
            else
            {
                if (objective != null)
                {
                    transform.position = Vector2.MoveTowards(transform.position, objective.position, (speed * (1 - speedModifer)) * Time.deltaTime);
                }
                else if (objectiveV3 != null)
                {
                    transform.position = Vector2.MoveTowards(transform.position, objectiveV3, (speed * (1 - speedModifer)) * Time.deltaTime);
                }
            }
        }
    }

    //Checks if nodes are blocked
    public bool NodeCheck(int nodeToCheck)
    {
        //South
        if(nodeToCheck == 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(vectorCheckPoint.position, Vector2.down, distanceToBarrierCheck, barrierMask);
            if (hit.collider != null)
            {
                Debug.Log("Hit South");
                return true;
            }
        }
        //North
        else if (nodeToCheck == 1)
        {
            RaycastHit2D hit = Physics2D.Raycast(vectorCheckPoint.position, Vector2.up, distanceToBarrierCheck, barrierMask);
            if (hit.collider != null)
            {
                Debug.Log("Hit North");
                return true;
            }
        }
        //East
        else if(nodeToCheck == 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(vectorCheckPoint.position, Vector2.left, distanceToBarrierCheck, barrierMask);
            if (hit.collider != null)
            {
                Debug.Log("Hit East");
                return true;
            }
        }
        //West
        else if (nodeToCheck == 3)
        {
            RaycastHit2D hit = Physics2D.Raycast(vectorCheckPoint.position, Vector2.right, distanceToBarrierCheck, barrierMask);
            if (hit.collider != null)
            {
                Debug.Log("Hit West");
                return true;
            }
        }

        return false;
    }

    //Increases or deceases
    //A postive value makes it faster, a negativ eone makes it slower
    public void ChangeSpeedModifer(float speedModChange)
    {
        speedModifer = speedModifer + speedModChange;
        speedModifer = Mathf.Clamp(speedModifer, 0f, 1f);

        if(speedModChange > 0)
        {
            modiferCount++;
        }
        else
        {
            modiferCount--;
        }
    }
}
