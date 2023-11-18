using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    //Point for enemies to be spawned around
    public Transform hubTransform;

    //Number of enemies to spawn
    public GameObject[] enemies;
    public int[] enemyCount;

    //Min distance away from enemy
    public float minSpawnY;
    public float minSpawnX;

    //Current Informmations
    //public GameObject 

    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Spawns in the enemies for the spawner
    public void SpawnEnemies()
    {
        for(int i = 0; i < enemies.Length; i++)
        {
            for(int j = 0; j < enemyCount[i]; i++)
            {
                GameObject tempEnemy = Instantiate(enemies[i], DetermineSpawnPoint() + transform.position, Quaternion.identity);
                tempEnemy.GetComponent<DamageScript>().isLocal = true;
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
            yValue = minSpawnY + Random.Range(-1f, 1f);
        }
        //Eastern Spawn
        else if (locationZone == 1)
        {
            xValue = minSpawnX + Random.Range(-1f, 1f);
            yValue = Random.Range(-minSpawnY, minSpawnY);
        }
        //Southern Spawn
        else if (locationZone == 2)
        {
            xValue = Random.Range(-minSpawnX, minSpawnX);
            yValue = -minSpawnY + Random.Range(-1f, 1f);
        }
        //Western Spawn
        else
        {
            xValue = -minSpawnX + Random.Range(-1f, 1f); ;
            yValue = Random.Range(-minSpawnY, minSpawnY);
        }

        return new Vector3(xValue, yValue, 0f);
    }
}
