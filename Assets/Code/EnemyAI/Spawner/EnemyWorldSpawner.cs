using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWorldSpawner : MonoBehaviour
{
    //Info stats
    public WorldGenerator.Biome currentBiome;
    private GameObject player;
    private int currentTime = 0;
    private RaycastHit2D pointChecker;
    public LayerMask barrierMask;

    //Spawn Stuff
    public static int enemyCount;
    public int currentRate;
    public int[] timesToSpawn;
    public int[] countOfEnemies;
    public float minSpawnY;
    public float minSpawnX;

    //List of Enemies by location
    public GameObject[] forestEnemies;
    public GameObject[] plainsEnemies;
    public GameObject[] desertEnemies;
    public GameObject[] redBiomeEnemies;

    // Start is called before the first frame update
    void Awake()
    {
        //Gets Player
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(currentTime >= timesToSpawn[currentRate])
        {
            currentTime = 0;
            int spawnValue = 0;

            //Determine Enemy
            currentBiome = GeneralUtil.GetBiomeAtPos(player.transform.position);
            //currentBiome = "Forest";
            if(currentBiome.name == "Forest")
            {
                spawnValue = Random.Range(0, forestEnemies.Length);
            }
            else if (currentBiome.name == "Plains")
            {
                spawnValue = Random.Range(0, plainsEnemies.Length);
            }
            else if (currentBiome.name == "Desert")
            {
                spawnValue = Random.Range(0, desertEnemies.Length);
            }
            else
            {
                spawnValue = Random.Range(0, redBiomeEnemies.Length);
            }

            //Spawn Enemy
            SpawnEnemy(forestEnemies[0]);
        }
        else
        {
            currentTime++;
        }
    }

    //Spawns Enemy in Game
    public void SpawnEnemy(GameObject enemyToSpawn)
    {
        Vector3 spawnPoint = DetermineSpawnPoint();

        Instantiate(enemyToSpawn, spawnPoint + transform.position, Quaternion.identity);
        enemyCount++;
        CheckRate();
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
        else if(locationZone == 1)
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

    //Checks if the enemy can spawn at that location
    public bool IsVaildPoint(Vector3 pointToCheck)
    {
        return true;

        pointChecker = Physics2D.Raycast(pointToCheck, Vector2.up, 1.0f, barrierMask);
        if (pointChecker.collider != null)
            return true;

        return false;
    }

    //Checks if the player
    public void CheckRate()
    {
        for(int i = countOfEnemies.Length-1; i >= 0; i--)
        {
            if(enemyCount >= countOfEnemies[i])
            {
                currentRate = i;
            }
        }
    }
}
