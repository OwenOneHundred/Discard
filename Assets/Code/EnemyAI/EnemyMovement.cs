using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public EnemyMind enemyMind;
    public GameObject player;

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
    //-1: No Direction, 0-Left/Up, 1-Right/Down
    private int movingDirection = -1;
    //0-Right of Enemy, 1-Left of Enemy, 2-Top of Enemy, 3-Below of Enemy
    public Transform[] vectorCheckPoints;
    private RaycastHit2D hit;
    public LayerMask barrierMask;
    public float distanceToBarrierCheck;

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    PauseMove(true);
        //}
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    PauseMove(false);
        //}

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
                    NodeCheck();

                    if(NodeCheck() == false)
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
    public bool NodeCheck()
    {
        //South
        RaycastHit2D hitNode = Physics2D.Raycast(transform.position, Vector2.down, distanceToBarrierCheck, barrierMask);
        if (hitNode.collider != null)
        {
            movingAroundDir = 0;

            //Determines Direction of Movement
            Vector3 difference = player.transform.position - transform.position;
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            if (angle >= 270f || angle <= 90f)
                movingDirection = 1;
            else
                movingDirection = 0;

            return true;
        }
        //North
        hitNode = Physics2D.Raycast(transform.position, Vector2.up, distanceToBarrierCheck, barrierMask);
        if (hitNode.collider != null)
        {
            movingAroundDir = 1;

            //Determines Direction of Movement
            Vector3 difference = player.transform.position - transform.position;
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            if (angle >= 270f || angle <= 90f)
                movingDirection = 1;
            else
                movingDirection = 0;

            return true;
        }
        //East
        hitNode = Physics2D.Raycast(transform.position, Vector2.left, distanceToBarrierCheck, barrierMask);
        if (hitNode.collider != null)
        {
            movingAroundDir = 2;

            //Determines Direction of Movement
            Vector3 difference = player.transform.position - transform.position;
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            if (angle >= 0f && angle <= 180f)
                movingDirection = 0;
            else
                movingDirection = 1;

            return true;
        }
        //West
        hitNode = Physics2D.Raycast(transform.position, Vector2.right, distanceToBarrierCheck, barrierMask);
        if (hitNode.collider != null)
        {
            movingAroundDir = 3;

            //Determines Direction of Movement
            Vector3 difference = player.transform.position - transform.position;
            float angle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
            if (angle >= 0f && angle <= 180f)
                movingDirection = 0;
            else
                movingDirection = 1;

            return true;
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
            //Moving Left
            if (movingDirection == 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.left, (modifedSpeed * Time.deltaTime));
                hit = Physics2D.Raycast(vectorCheckPoints[0].position, Vector2.down, distanceToBarrierCheck + 1f, barrierMask);
                Debug.DrawRay(vectorCheckPoints[0].position, Vector2.down);
            }
            //Moving Right
            else if (movingDirection == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.right, (modifedSpeed * Time.deltaTime));
                hit = Physics2D.Raycast(vectorCheckPoints[1].position, Vector2.down, distanceToBarrierCheck + 1f, barrierMask);
                Debug.DrawRay(vectorCheckPoints[1].position, Vector2.down);
            }

            if (hit.collider == null)
            {
                movingAroundDir = -1;
                movingDirection = -1;
            }
        }
        //North Move
        else if (movingAroundDir == 1)
        {
            //Moving Left
            if (movingDirection == 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.left, (modifedSpeed * Time.deltaTime));
                hit = Physics2D.Raycast(vectorCheckPoints[0].position, Vector2.up, distanceToBarrierCheck + 1f, barrierMask);
                Debug.DrawRay(vectorCheckPoints[0].position, Vector2.up);
            }
            //Moving Right
            else if (movingDirection == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.right, (modifedSpeed * Time.deltaTime));
                hit = Physics2D.Raycast(vectorCheckPoints[1].position, Vector2.up, distanceToBarrierCheck + 1f, barrierMask);
                Debug.DrawRay(vectorCheckPoints[1].position, Vector2.up);
            }

            if (hit.collider == null)
            {
                movingAroundDir = -1;
                movingDirection = -1;
            }
        }
        //East Move
        else if(movingAroundDir == 2)
        {
            //Moving up
            if (movingDirection == 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.up, (modifedSpeed * Time.deltaTime));
                hit = Physics2D.Raycast(vectorCheckPoints[3].position, Vector2.left, distanceToBarrierCheck + 1f, barrierMask);
                Debug.DrawRay(vectorCheckPoints[3].position, Vector2.left);
            }
            //Moving down
            else if (movingDirection == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.down, (modifedSpeed * Time.deltaTime));
                hit = Physics2D.Raycast(vectorCheckPoints[2].position, Vector2.left, distanceToBarrierCheck + 1f, barrierMask);
                Debug.DrawRay(vectorCheckPoints[2].position, Vector2.left);
            }

            if (hit.collider == null)
            {
                movingAroundDir = -1;
                movingDirection = -1;
            }
        }
        //West Move
        else
        {
            //Moving up
            if (movingDirection == 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.up, (modifedSpeed * Time.deltaTime));
                hit = Physics2D.Raycast(vectorCheckPoints[3].position, Vector2.right, distanceToBarrierCheck + 1f, barrierMask);
                Debug.DrawRay(vectorCheckPoints[3].position, Vector2.right);
            }
            //Moving down
            else if (movingDirection == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.down, (modifedSpeed * Time.deltaTime));
                hit = Physics2D.Raycast(vectorCheckPoints[2].position, Vector2.right, distanceToBarrierCheck + 1f, barrierMask);
                Debug.DrawRay(vectorCheckPoints[2].position, Vector2.right);
            }

            if (hit.collider == null)
            {
                movingAroundDir = -1;
                movingDirection = -1;
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
