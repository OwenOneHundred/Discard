using System;
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

    public List<Biome> biomes;

    public event Action<Vector3Int, Biome> CallCustomScripts;

    // Start is called before the first frame update
    void Start()
    {
        seed = UnityEngine.Random.Range(-9999999, 10000000);
        UnityEngine.Random.InitState(seed);
        perlinCenter = new Vector2(UnityEngine.Random.Range(-999999f, 999999f), UnityEngine.Random.Range(-999999f, 999999f));

        SetUpBiomeWeights();

        GenerateWorld();
    }

    void SetUpBiomeWeights()
    {
        float totalBiomeWeight = 0;

        biomeSize = 1 / biomeSize;

        foreach (Biome biome in biomes)
        {
            totalBiomeWeight += biome.weight;
        }

        foreach (Biome biome1 in biomes)
        {
            biome1.relativeWeight = biome1.weight / totalBiomeWeight;
        }

        float stack = 0;
        foreach (Biome biome1 in biomes)
        {
            biome1.thresholdWeight = stack + biome1.relativeWeight;
            stack += biome1.relativeWeight;
        }
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
                    Unity.Mathematics.noise.snoise(new Unity.Mathematics.float2((perlinCenter.x + currentCell.x) * biomeSize, (perlinCenter.y + currentCell.y) * biomeSize));

                float biomeValue = (Mathf.Clamp(positionPerlin, -1, 1) + 1) / 2f;

                int biomeIndex = 0;
                foreach (Biome biome in biomes)
                {
                    if (biome.thresholdWeight > biomeValue)
                    {
                        break;
                    }
                    else
                    {
                        biomeIndex += 1;
                    }
                }    

                ground.SetTile(currentCell, biomes[biomeIndex].groundTile);

                biomes[biomeIndex].biomeTiles.Add(currentCell);

                // try to spawn objects
                CheckSpawnObject(currentCell, biomes[biomeIndex]);

                CallCustomScripts?.Invoke(currentCell, biomes[biomeIndex]);

                genCount += 1;
                if (genCount >= genPerFrame)
                {
                    genCount = 0;
                    yield return null;
                }
            }
        }
    }

    void CheckSpawnObject(Vector3Int location, Biome biome)
    {
        foreach (Biome.DecorObject decorObject in biome.decorObjects)
        {
            if (UnityEngine.Random.value < decorObject.spawnrate0to1)
            {
                Instantiate(decorObject.prefab, location + new Vector3(0.5f, 0.5f), Quaternion.identity);
                return;
            }
        }
    }

    [System.Serializable]
    public class Biome
    {
        public string name;

        public int temperature;

        public int weight = 1;

        [System.NonSerialized] public float relativeWeight = 0;
        [System.NonSerialized] public float thresholdWeight = 0;

        [System.NonSerialized] public List<Vector3Int> biomeTiles = new List<Vector3Int>();
        public RuleTile groundTile;
        public RuleTile raisedGroundFront;
        public RuleTile raisedGroundTop;

        public MonoBehaviour customScript;
        public System.Action<Vector3Int> customMethod;

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

        [System.Serializable]
        public class Splotch
        {
            public TileBase tile;
            public int size;
            public Tilemap tilemap;
        }
    }
}
