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
    private int frozenCount;
    public SpriteRenderer enemyImageRenderer;

    //-1-Idle, 0-South, 1-North, 2-East, 3-West
    private int movingAroundDir;
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
                        movingAroundDir = 3;
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
                Debug.Log("Hit South");
                return true;
            }
        }
        //North
        else if (nodeToCheck == 1)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, distanceToBarrierCheck, barrierMask);
            if (hit.collider != null)
            {
                Debug.Log("Hit North");
                return true;
            }
        }
        //East
        else if (nodeToCheck == 2)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, distanceToBarrierCheck, barrierMask);
            if (hit.collider != null)
            {
                Debug.Log("Hit East");
                return true;
            }
        }
        //West
        else if (nodeToCheck == 3)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, distanceToBarrierCheck, barrierMask);
            if (hit.collider != null)
            {
                Debug.Log("Hit West");
                return true;
            }
        }

        return false;
    }

    //Moves enemy in the right dir and checks for movement done
    //directions 
    public void MoveAround(int direction)
    {
        //South Move
        if (direction == 0)
        {

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
            frozenCount++;
            enemyMind.Freeze(false);
            enemyImageRenderer.color = new Color(.5f, 1f, 1f, 1f);
        }
        //If enemy has been frozen multiple times
        else if (frozenCount > 1)
        {
            frozenCount--;
        }
        //Needs to Unfreeze
        else
        {
            frozenCount--;
            isFrozen = 1.0f;
            enemyMind.Freeze(true);
            enemyImageRenderer.color = new Color(1f, 1f, 1f, 1f);
        }
    }
}
