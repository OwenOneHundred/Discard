using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public EnemyMind enemyMind;

    public bool atObjective;

    public Transform objective;
    public Vector3 objectiveV3;
    public float speed;

    //Used for Modifing speed
    //0 means no change, 1 means no speed
    private float speedModifier = 0;
    public int modifierCount = 0;
    //1 means not frozen, 0 means frozen
    private float isFrozen = 1;
    public SpriteRenderer enemyImageRenderer;

    //-1-Idle, 0-South, 1-North, 2-East, 3-West
    private int movingAroundDir = -1;
    public Transform[] vectorCheckPoints;
    public LayerMask barrierMask;
    public float distanceToBarrierCheck;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            PauseMove(true);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            PauseMove(false);
        }

        //Moves Enemy Object towards objective if no at it already
        if (atObjective != true && (objective != null || objectiveV3 != null))
        {
            float modifedSpeed = speed * (1 - speedModifier) * isFrozen;

            if (modifedSpeed > 0f)
            {
                //Checks if enemy is moving around
                if (movingAroundDir != -1)
                {
                    MoveAround(modifedSpeed);
                }
                else
                {
                    //Checks East direction
                    if (NodeCheck(2))
                    {
                        movingAroundDir = 2;
                    }
                    //Checks West direction
                    else if (NodeCheck(3))
                    {
                        movingAroundDir = 3;
                    }
                    //Checks South direction
                    else if (NodeCheck(0))
                    {
                        movingAroundDir = 0;
                    }
                    //Checks North direction
                    else if (NodeCheck(1))
                    {
                        movingAroundDir = 1;
                    }
                    else
                    {
                        if (objective != null)
                        {
                            transform.position = Vector2.MoveTowards(transform.position, objective.position, modifedSpeed * Time.deltaTime);
                        }
                        else if (objectiveV3 != null)
                        {
                            transform.position = Vector2.MoveTowards(transform.position, objectiveV3, modifedSpeed * Time.deltaTime);
                        }
                    }
                }
            }
        }
    }

    //Checks if nodes are blocked
    public bool NodeCheck(int nodeToCheck)
    {
        //South
        if (nodeToCheck == 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distanceToBarrierCheck, barrierMask);
            if (hit.collider != null)
            {
                Debug.Log("Start South");
                return true;
            }
        }
        //North
        else if (nodeToCheck == 1)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, distanceToBarrierCheck, barrierMask);
            if (hit.collider != null)
            {
                Debug.Log("Start North");
                return true;
            }
        }
        //East
        else if (nodeToCheck == 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, distanceToBarrierCheck, barrierMask);
            if (hit.collider != null)
            {
                Debug.Log("Start East");
                return true;
            }
        }
        //West
        else if (nodeToCheck == 3)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, distanceToBarrierCheck, barrierMask);
            if (hit.collider != null)
            {
                Debug.Log("Start West");
                return true;
            }
        }
        return false;
    }

    //Moves enemy around barrier til there is no more barrier 
    //Directions: -1-Idle, 0-South, 1-North, 2-East, 3-West
    public void MoveAround(float modifedSpeed)
    {
        //South Move
        if (movingAroundDir == 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.left, (modifedSpeed * Time.deltaTime));
            RaycastHit2D hit = Physics2D.Raycast(vectorCheckPoints[0].position, Vector2.down, distanceToBarrierCheck+1f, barrierMask);
            Debug.DrawRay(vectorCheckPoints[0].position, Vector2.down);
            if (hit.collider == null)
            {
                movingAroundDir = -1;
            }
        }
        //North Move
        else if (movingAroundDir == 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.left, (modifedSpeed * Time.deltaTime));
            RaycastHit2D hit = Physics2D.Raycast(vectorCheckPoints[1].position, Vector2.up, distanceToBarrierCheck + .5f, barrierMask);
            Debug.DrawRay(vectorCheckPoints[1].position, Vector2.up);
            if (hit.collider == null)
            {
                movingAroundDir = -1;
            }
        }
        //East Move
        else if(movingAroundDir == 2)
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.down, (modifedSpeed * Time.deltaTime));
            RaycastHit2D hit = Physics2D.Raycast(vectorCheckPoints[2].position, Vector2.left, distanceToBarrierCheck + .5f, barrierMask);
            Debug.DrawRay(vectorCheckPoints[2].position, Vector2.left);
            if (hit.collider == null)
            {
                movingAroundDir = -1;
            }
        }
        //West Move
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.down, (modifedSpeed * Time.deltaTime));
            RaycastHit2D hit = Physics2D.Raycast(vectorCheckPoints[3].position, Vector2.right, distanceToBarrierCheck + .5f, barrierMask);
            Debug.DrawRay(vectorCheckPoints[3].position, Vector2.right);
            if (hit.collider == null)
            {
                movingAroundDir = -1;
            }
        }
    }

    //Increases or deceases
    //A postive value makes it faster, a negativ eone makes it slower
    public void ChangeSpeedModifier(float speedModChange)
    {
        speedModifier = speedModifier + speedModChange;
        speedModifier = Mathf.Clamp(speedModifier, 0f, 1f);

        if (speedModChange > 0)
        {
            modifierCount++;
        }
        else
        {
            modifierCount--;
        }
    }

    //Pause enemy movment, enemy attack and enemy image
    public void PauseMove(bool toFreeze)
    {
        //Needs to Freeze
        if (toFreeze)
        {
            isFrozen = 0.0f;
            enemyMind.Freeze(false);
            enemyImageRenderer.color = new Color(.5f, 1f, 1f, 1f);
        }
        //Needs to Unfreeze
        else
        {
            isFrozen = 1.0f;
            enemyMind.Freeze(true);
            enemyImageRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
    }
}
