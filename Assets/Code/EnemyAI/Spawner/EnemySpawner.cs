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
                tempEnemy.GetComponent<EnemyInfo>().isLocal = true;
            }
        }
    }

    //Returns a point in the game world where the enemy can spawn 
    public Vector3 DetermineSpawnPoint()
    {
        return new Vector3(0f, 0f, 0f);
    }
}
