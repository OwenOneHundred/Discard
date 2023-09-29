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
        public int fireRate;
        //Kills enemy after attack
        public bool diesOnFire;
        //Stops the bullet from angle towards the player
        //If true does not rotate bullet
        public bool dontAngle;
    }

    //In attack range of player
    public bool inRange;

    //Attacks
    public Attack[] attacks;
    //if equal to -1 then now attack selected
    private int currentAttack = -1;

    //Fire Rate
    private int currentFireCount;

    //Objs Needed--------
    //Gotten from enemy mind
    public Transform player;
    public GameObject enemyTargeter;

    private EnemyInfo enemyInfo;

    // Start is called before the first frame update
    void Start()
    {
        //Gets Info
        enemyInfo = this.gameObject.GetComponent<EnemyInfo>();
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
            //If enemy is in range and has had enough time since last fire
            if (inRange && currentFireCount > attacks[0].fireRate)
            {
                currentFireCount = 0;

                //Positioning Firing Object
                Vector3 difference = player.transform.position - transform.position;
                float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                enemyTargeter.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

                Fire(0);
            }
            else
            {
                currentFireCount++;
            }
        }
        //If multiple attacks then random choices
        else if(attacks.Length > 1)
        {
            //Give new attack
            if (currentAttack == -1)
            {
                currentAttack = Random.Range(0, attacks.Length);
                currentFireCount++;
            }
            else
            {
                //If enemy is in range and has had enough time since last fire
                if (inRange && currentFireCount > attacks[currentAttack].fireRate)
                {
                    currentFireCount = 0;

                    //Positioning Firing Object
                    Vector3 difference = player.transform.position - transform.position;
                    float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                    enemyTargeter.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

                    Fire(currentAttack);
                    currentAttack = -1;
                }
                else
                {
                    currentFireCount++;
                }
            }
        }
    }

    //Fires the Bullet at the player
    public void Fire(int attackNum)
    {
        if(attacks[attackNum].dontAngle == true)
        {
            //Fire
            Instantiate(attacks[attackNum].bullet, enemyTargeter.transform.position, Quaternion.identity);

            if(attacks[attackNum].diesOnFire == true)
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
