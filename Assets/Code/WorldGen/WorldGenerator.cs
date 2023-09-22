using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    int seed;
    Vector2 perlinCenter;

    [SerializeField] Tilemap ground;

    [SerializeField] int worldSize;
    [SerializeField] int genPerFrame = 20;

    [SerializeField] float biomeSize;
    float changeAmount;

    [SerializeField] List<Biome> biomes;
    float biomeThreshold;

    // Start is called before the first frame update
    void Start()
    {
        seed = Random.Range(-9999999, 10000000);
        UnityEngine.Random.InitState(seed);
        perlinCenter = new Vector2(UnityEngine.Random.Range(-999999f, 999999f), UnityEngine.Random.Range(-999999f, 999999f));
        biomeThreshold = 1f / biomes.Count;
        changeAmount = 1 / biomeSize;

        GenerateWorld();
    }

    void GenerateWorld()
    {
        StartCoroutine(GenerateBiomeTiles(worldSize, Vector2.zero));
    }

    IEnumerator GenerateBiomeTiles(int generationRange, Vector2 positionToGenerate)
    {
        int genCount = 0;
        Vector3Int centerCell = ground.WorldToCell(new Vector3(positionToGenerate.x, positionToGenerate.y, 0));
        for (int currentCellX = (int)centerCell.x - generationRange; currentCellX < centerCell.x + generationRange; currentCellX++)
        {
            for (int currentCellY = (int)centerCell.y - generationRange; currentCellY < centerCell.y + generationRange; currentCellY++)
            {
                Vector3Int currentCell = new Vector3Int(currentCellX, currentCellY, 0);

                float positionPerlin =
                    Unity.Mathematics.noise.snoise(new Unity.Mathematics.float2((perlinCenter.x + currentCell.x) * changeAmount, (perlinCenter.y + currentCell.y) * changeAmount));

                int biomeIndex = Mathf.FloorToInt(Mathf.Clamp(positionPerlin, 0, 1) / biomeThreshold);

                ground.SetTile(currentCell, biomes[biomeIndex].groundTile);
                genCount += 1;
                if (genCount >= genPerFrame)
                {
                    genCount = 0;
                    yield return null;
                }
            }
        }
    }


    [System.Serializable]
    public class Biome
    {
        public string name;

        public int temperature;

        public RuleTile groundTile;
        public RuleTile raisedGroundFront;
        public RuleTile raisedGroundTop;

        [Tooltip("Input the GameObject for the object, and the chance of it spawning on any particular tile (0 - 1).")]
        public List<DecorObject> decorObjects;

        [Tooltip("Input the tilemaps for the structure, the center of the structure on the tilemap (if it's not (0, 0)), and the frequency.")]
        public List<DecorObject> structures;

        [System.Serializable]
        public class DecorObject
        {
            public GameObject prefab;
            public float spawnrate0to1;
        }

        [System.Serializable]
        public class Structure
        {
            public GameObject structureTilemaps;
            public int size;
            public Vector2 center;
        }
    }
}
