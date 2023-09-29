using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    //Basic Attack of Enemy
    [System.Serializable]
    public class Attack
    {
        public GameObject bullet;
        public int fireRate;
    }

    //In attack range of player
    public bool inRange;

    //Attacks
    public Attack[] attacks;
    //if equal to -1 then now attack selected
    private int currentAttack;

    //Fire Rate
    private int currentFireCount;

    //Objs Needed--------
    //Gotten from enemy mind
    public Transform player;
    public GameObject enemyTargeter;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //If there are only one attack then does the one attack
        if(attacks.Length == 1)
        {
            //If enemy is in range and has had enough time since last fire
            if (inRange && currentFireCount > attacks[0].fireRate)
            {
                currentFireCount = 0;

                //Positioning Firing Object
                Vector3 difference = player.transform.position - transform.position;
                float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                enemyTargeter.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

                //Fire
                Instantiate(attacks[0].bullet, enemyTargeter.transform.position, enemyTargeter.transform.rotation);
            }
            else
            {
                currentFireCount++;
            }
        }
        //If multiple attacks then random choices
        else
        {
            //Give new attack
            if(currentAttack == -1)
            {
                currentAttack = Random.Range(0, attacks.Length + 1);
                currentFireCount++;
            }
            else
            {
                //If enemy is in range and has had enough time since last fire
                if (inRange && currentFireCount > attacks[currentAttack].fireRate)
                {
                    currentFireCount = 0;
                    currentAttack = -1;

                    //Positioning Firing Object
                    Vector3 difference = player.transform.position - transform.position;
                    float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
                    enemyTargeter.transform.rotation = Quaternion.Euler(0f, 0f, rotZ);

                    //Fire
                    Instantiate(attacks[currentAttack].bullet, enemyTargeter.transform.position, enemyTargeter.transform.rotation);
                }
                else
                {
                    currentFireCount++;
                }
            }
        }
    }
}
