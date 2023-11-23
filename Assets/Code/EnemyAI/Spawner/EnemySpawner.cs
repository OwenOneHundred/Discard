using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //Player object
    public GameObject player;

    //Number of enemies to spawn
    public GameObject[] enemies;
    public int[] enemyCount;

    //Min distance away from enemy
    public float minSpawnY;
    public float minSpawnX;

    //Current Informmations
    public GameObject[] allEnemies;
    public int maxEnemies;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        for(int i = 0; i < enemyCount.Length; i++)
        {
            maxEnemies = maxEnemies + enemyCount[i];
        }
        allEnemies = new GameObject[maxEnemies];
    }

    // Update is called once per frame
    void Update()
    {
        if(GetDist(player.transform.position) <= 25f)
        {
            SpawnEnemies();
        }
    }

    //Spawns in the enemies for the spawner
    public void SpawnEnemies()
    {
        int countThrough = 0; 
        for(int i = 0; i < enemies.Length; i++)
        {
            for(int j = 0; j < enemyCount[i]; j++)
            {
                if(allEnemies[countThrough] == null)
                {
                    GameObject tempEnemy = Instantiate(enemies[i], DetermineSpawnPoint() + transform.position, Quaternion.identity);
                    tempEnemy.GetComponent<DamageScript>().isLocal = true;
                    allEnemies[countThrough] = tempEnemy;
                }
                countThrough++;
            }
        }
    }

    //Returns a point in the game world where the enemy can spawn 
    public Vector3 DetermineSpawnPoint()
    {
        //Determines the zone of the enemy that it is spawned it in
        //0-North, 1-East, 2-South, 3-West
        int locationZone = Random.Range(0, 4);
        float xValue = 0f;
        float yValue = 0f;

        //Northern Spawn
        if (locationZone == 0)
        {
            xValue = Random.Range(-minSpawnX, minSpawnX);
            yValue = minSpawnY + Random.Range(-2f, 2f);
        }
        //Eastern Spawn
        else if (locationZone == 1)
        {
            xValue = minSpawnX + Random.Range(-2f, 2f);
            yValue = Random.Range(-minSpawnY, minSpawnY);
        }
        //Southern Spawn
        else if (locationZone == 2)
        {
            xValue = Random.Range(-minSpawnX, minSpawnX);
            yValue = -minSpawnY + Random.Range(-2f, 2f);
        }
        //Western Spawn
        else
        {
            xValue = -minSpawnX + Random.Range(-2f, 2f); ;
            yValue = Random.Range(-minSpawnY, minSpawnY);
        }

        return new Vector3(xValue, yValue, 0f);
    }

    //Returns the distance between the player and the enemy
    public float GetDist(Vector3 target)
    {
        Vector3 currentPos = transform.position;
        float dist = Vector3.Distance(target, currentPos);

        return dist;
    }
}
