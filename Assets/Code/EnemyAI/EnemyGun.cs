using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    //Basic Attack of Enemy
    [System.Serializable]
    public class Attack
    {
        //What the Enemy is Firing
        public GameObject bullet;
        //How fast they are firing
        //Time til start of anim
        public int preAnimfireRate;
        //Time til fire from start of anim
        public int animfireRate;
        //Time til animOver after firerate
        public int postAnimfireRate;
        //Kills enemy after attack
        public bool diesOnFire;
        //Stops the bullet from angle towards the player
        //If true does not rotate bullet
        public bool dontAngle;
        //Number of Bullets Fired per attack
        public int bulletCount;
        //Amount of time between bullets being fired
        public int timeBetweenBullets;
    }

    //In attack range of player
    public bool inRange;

    //Attacks
    public Attack[] attacks;
    //if equal to -1 then now attack selected
    private int currentAttack = -1;

    //Fire Rate
    public int currentAttackPhase;
    private int currentFireCount;
    private int currentNumBulletsFired;
    private int currentInBetweenFireCount = int.MaxValue;

    //Objs Needed--------
    //Gotten from enemy mind
    public Transform player;
    public GameObject enemyTargeter;

    private EnemyInfo enemyInfo;
    private EnemyImager enemyImager;

    // Start is called before the first frame update
    void Start()
    {
        //Gets Info
        enemyInfo = this.gameObject.GetComponent<EnemyInfo>();
        enemyImager = this.gameObject.GetComponent<EnemyImager>();
    }

    // Update is called 60 times a second
    void FixedUpdate()
    {
        Fight();
    }

    //Casues Enemy to Attack
    public void Fight()
    {
        //If there are only one attack then does the one attack
        if (attacks.Length == 1)
        {
            if(currentAttackPhase == 1)
            {
                OneOptionAttackPhase1();
            }
            else if(currentAttackPhase == 2)
            {
                OneOptionAttackPhase2();
            }
            else
            {
                OneOptionAttackPhase3();
            }
        }
        //If multiple attacks then random choices
        else if (attacks.Length > 1)
        {
            if (currentAttackPhase == 1)
            {
                MultiOptionAttackPhase1();
            }
            else if (currentAttackPhase == 2)
            {
                MultiOptionAttackPhase2();
            }
            else
            {
                MultiOptionAttackPhase3();
            }
        }
    }

    //Handles attack if only one option is available
    //Counts up time with enemy in idle pos
    public void OneOptionAttackPhase1()
    {
        if(inRange && currentFireCount > attacks[0].preAnimfireRate)
        {
            currentFireCount = 0;
            currentAttackPhase++;
            enemyImager.isAttacking = true;
            enemyImager.attackCount = 0;
            enemyImager.ResetAnim();
        }
        else
        {
            currentFireCount++;
        }
    }

    //Handles attack if only one option is available
    //Does the Firing
    public void OneOptionAttackPhase2()
    {
        //If enemy is in range and has had enough time since last fire
        if (inRange && currentFireCount > attacks[0].animfireRate)
        {
            //When the bullet only fires one shot
            if (attacks[0].bulletCount == 1)
            {
                currentFireCount = 0;
                currentAttackPhase++;

                //Positioning Firing Object
                Vector3 difference = player.transform.position - transform.position;
                float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                enemyTargeter.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

                Fire(0);
            }
            //Fires Multiple Bullets
            else
            {
                //Checks if they have fired all bullets
                if (currentNumBulletsFired < attacks[0].bulletCount)
                {
                    //Checks if been long enough
                    if (currentInBetweenFireCount > attacks[0].timeBetweenBullets)
                    {
                        currentInBetweenFireCount = 0;
                        currentNumBulletsFired++;

                        //Positioning Firing Object
                        Vector3 difference = player.transform.position - transform.position;
                        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                        enemyTargeter.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

                        Fire(0);
                    }

                    currentInBetweenFireCount++;
                }
                //Fired all bullets, resets info
                else
                {
                    currentAttackPhase++;
                    currentFireCount = 0;
                    currentNumBulletsFired = 0;
                    currentInBetweenFireCount = int.MaxValue;
                }
            }
        }

        currentFireCount++;
    }

    //Handles attack if only one option is available
    //Loops over time for bost shot anim
    public void OneOptionAttackPhase3()
    {
        if (inRange && currentFireCount > attacks[0].preAnimfireRate)
        {
            currentFireCount = 0;
            currentAttackPhase = 1;
            enemyImager.isAttacking = false;
        }
        else
        {
            currentFireCount++;
        }
    }

    //Handles attack if several options are available
    //Counts up time with enemy in idle pos
    public void MultiOptionAttackPhase1()
    {
        //Gives Attack Type
        //Give new attack
        if (currentAttack == -1)
        {
            currentAttack = Random.Range(0, attacks.Length);
        }


        if (inRange && currentFireCount > attacks[currentAttack].preAnimfireRate)
        {
            currentFireCount = 0;
            currentAttackPhase++;
            enemyImager.isAttacking = true;
            enemyImager.attackCount = currentAttack;
            enemyImager.ResetAnim();
        }
        else
        {
            currentFireCount++;
        }
    }

    //Handles attack if several options are available
    //Does the Firing
    public void MultiOptionAttackPhase2()
    {
        //If enemy is in range and has had enough time since last fire
        if (inRange && currentFireCount > attacks[currentAttack].animfireRate)
        {
            //When the bullet only fires one shot
            if (attacks[currentAttack].bulletCount == 1)
            {
                currentFireCount = 0;
                currentAttackPhase++;

                //Positioning Firing Object
                Vector3 difference = player.transform.position - transform.position;
                float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                enemyTargeter.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

                Fire(currentAttack);
            }
            //Fires Multiple Bullets
            else
            {
                //Checks if they have fired all bullets
                if (currentNumBulletsFired < attacks[currentAttack].bulletCount)
                {
                    //Checks if been long enough
                    if (currentInBetweenFireCount > attacks[currentAttack].timeBetweenBullets)
                    {
                        currentInBetweenFireCount = 0;
                        currentNumBulletsFired++;

                        //Positioning Firing Object
                        Vector3 difference = player.transform.position - transform.position;
                        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                        enemyTargeter.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

                        Fire(currentAttack);
                    }

                    currentInBetweenFireCount++;
                }
                //Fired all bullets, resets info
                else
                {
                    currentAttackPhase++;
                    currentFireCount = 0;
                    currentNumBulletsFired = 0;
                    currentInBetweenFireCount = int.MaxValue;
                }
            }
        }

        currentFireCount++;
    }

    //Handles attack if several options are available
    //Counts up time with enemy in idle pos
    public void MultiOptionAttackPhase3()
    {
        if (inRange && currentFireCount > attacks[0].preAnimfireRate)
        {
            currentFireCount = 0;
            currentAttackPhase = 1;
            enemyImager.isAttacking = false;
            enemyImager.attackCount = 0;
            currentAttack = -1;
        }
        else
        {
            currentFireCount++;
        }
    }


    //Fires the Bullet at the player
    public void Fire(int attackNum)
    {
        if (attacks[attackNum].dontAngle == true)
        {
            //Fire
            Instantiate(attacks[attackNum].bullet, enemyTargeter.transform.position, Quaternion.identity);

            if (attacks[attackNum].diesOnFire == true)
            {
                enemyInfo.Death();
            }
        }
        else
        {
            //Fire
            Instantiate(attacks[attackNum].bullet, enemyTargeter.transform.position, enemyTargeter.transform.rotation);

            if (attacks[attackNum].diesOnFire == true)
            {
                enemyInfo.Death();
            }
        }
    }
}
