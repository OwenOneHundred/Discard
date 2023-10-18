using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyImager : MonoBehaviour
{
    [System.Serializable]
    public class SpriteAnimation
    {
        public Sprite[] sprites;
        public int[] timeBetweenAnim;
        public Vector3 targetVector;
    }

    //Info to determine anim
    //-1-Idle, 0-South, 1-North, 2-East, 3-West
    public int direction = -1;
    public bool isDead;
    public bool isMoving;
    public bool isAttacking;
    // Int value that indicates what attakc number is being used
    public int attackCount;
    public SpriteRenderer enemySpriteRenderer;
    public GameObject targetObj;
    public BoxCollider2D boxCollider2D;

    //Stuff for playing through animation
    public int timeThroughAnim;
    public int currentSpriteShowing;
    public bool isAnimDone = true;
    //0 = idle, 1 = movement, 2 = attack, 3 = death
    public int currentAnim;

    public SpriteAnimation idleAnimation;
    public SpriteAnimation[] walkAnimations;
    public SpriteAnimation[] attackAnimations;
    public SpriteAnimation[] deathAnimations;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(isDead == true)
        {
            currentAnim = 3;
            AnimateDeath();
        }
        else if (isAttacking == true && (isAnimDone == true || currentAnim == 2))
        {
            currentAnim = 2;
            AnimateAttack();
        }
        /*else if ((isMoving == true && isAnimDone == true) || currentAnim == 1)
        {
            currentAnim = 1;
            AnimateWalk();
        }*/
        else if(isMoving == true)
        {
            AnimateWalk();
        }
        //If not the other options then must by idle
        else
        {
            currentAnim = 0;
            AnimateIdle();
            isAnimDone = true;
        }
    }

    //Animates enemy based on sprite sanimation 
    public void Animate(SpriteAnimation spriteAnimation)
    {
        //Sets Sprites
        int timeToWait = spriteAnimation.timeBetweenAnim[0];
        if(spriteAnimation.timeBetweenAnim.Length > 1)
        {
            timeToWait = spriteAnimation.timeBetweenAnim[currentSpriteShowing];
        }

        if (currentSpriteShowing == 0 && timeThroughAnim == 0)
        {
            isAnimDone = false;
        }

        if(timeThroughAnim > timeToWait)
        {
            currentSpriteShowing++;
            if (currentSpriteShowing >= spriteAnimation.sprites.Length)
            {
                currentSpriteShowing = 0;
                isAnimDone = true;
                currentAnim = 0;
            }

            enemySpriteRenderer.sprite = spriteAnimation.sprites[currentSpriteShowing];
            timeThroughAnim = 0;
        }
        else
        {
            timeThroughAnim++;
        }

        //Sets Target Point
        if(spriteAnimation.targetVector.x != 69f)
        {
            targetObj.transform.localPosition = spriteAnimation.targetVector;
        }
        else
        {
            //Sets Idle Pos
            targetObj.transform.localPosition = idleAnimation.targetVector;
        }
    }

    //Animates Idle
    public void AnimateIdle()
    {
        enemySpriteRenderer.sprite = idleAnimation.sprites[direction];
    }

    //Animates Walk
    public void AnimateWalk()
    {
        if(direction == 0)
        {
            Animate(walkAnimations[0]);
        }
        else if (direction == 1)
        {
            Animate(walkAnimations[1]);
        }
        else if (direction == 2)
        {
            Animate(walkAnimations[2]);
        }
        else if (direction == 3)
        {
            Animate(walkAnimations[3]);
        }
        else
        {
            Debug.Log("Idle");
        }
    }

    //Animates Attack 
    public void AnimateAttack()
    {
        if(attackAnimations.Length <= 4)
        {
            Animate(attackAnimations[direction]);
        }
        else
        {
            Animate(attackAnimations[direction + (4 * attackCount)]);
        }
    }

    //Animates Death 
    public void AnimateDeath()
    {
        if (direction == 0)
        {
            Animate(deathAnimations[0]);
        }
        else if (direction == 1)
        {
            Animate(deathAnimations[1]);
        }
        else if (direction == 2)
        {
            Animate(deathAnimations[2]);
        }
        else if (direction == 3)
        {
            Animate(deathAnimations[3]);
        }
        else
        {
            Debug.Log("Idle");
        }
    }

    //Ensure next anim starts at begining frame
    public void ResetAnim()
    {
        timeThroughAnim = 0;
        currentSpriteShowing = 0;
    }

}
