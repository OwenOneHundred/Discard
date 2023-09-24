using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    int seed;
    Vector2 perlinCenter;

    [SerializeField] Tilemap ground;
    [SerializeField] Tilemap raisedGround;

    [SerializeField] RuleTile rgShadowTile;

    [SerializeField] int worldSize;
    [SerializeField] int genPerFrame = 20;

    [SerializeField] float biomeSize;
    [SerializeField] int hillSpawnAttempts = 30;
    [SerializeField] int averageHillSize = 20;

    public List<Biome> biomes;

    public event Action<Vector3Int, Biome> CallCustomScripts;

    [System.NonSerialized] public List<Vector3Int> raisedTiles;
    [System.NonSerialized] public List<Vector3Int> decorObjectTiles;

    // Start is called before the first frame update
    void Start()
    {
        seed = UnityEngine.Random.Range(-9999999, 10000000);
        UnityEngine.Random.InitState(seed);
        perlinCenter = new Vector2(UnityEngine.Random.Range(-999999f, 999999f), UnityEngine.Random.Range(-999999f, 999999f));

        SetUpBiomeHierarchy();

        SetUpBiomeWeights();

        StartCoroutine(GenerateWorld());
    }

    void SetUpBiomeHierarchy()
    {
        foreach (Biome biome in biomes)
        {
            biome.decorObjectRoot = new GameObject(biome.name + " Decor Root").transform;

            foreach (Biome.DecorObject decorObject in biome.decorObjects)
            {
                decorObject.typeParent = new GameObject(decorObject.name + " Parent").transform;
                decorObject.typeParent.parent = biome.decorObjectRoot;
            }
        }
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

    IEnumerator GenerateWorld()
    {
        yield return StartCoroutine(GenerateBiomeTiles(worldSize, Vector2.zero));
        StartCoroutine(GenerateHills());
    }

    IEnumerator GenerateBiomeTiles(int generationRange, Vector2 positionToGenerate)
    {
        int genCount = 0;
        Vector3Int centerCell = ground.WorldToCell(new Vector3(positionToGenerate.x, positionToGenerate.y, 0));
        for (int currentCellX = (int)centerCell.x - generationRange; currentCellX <= centerCell.x + generationRange; currentCellX++)
        {
            for (int currentCellY = (int)centerCell.y - generationRange; currentCellY <= centerCell.y + generationRange; currentCellY++)
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

    IEnumerator GenerateHills()
    {
        for (int i = 0; i < hillSpawnAttempts; i++)
        {
            Vector3Int pos = PickRandomTilePos();
            Biome startBiome = GetBiomeAtPos(pos);

            if (startBiome.raisedGroundTop == null || startBiome.hillOdds0to1 <= 0) { continue; }

            int height = UnityEngine.Random.Range(2, 5);

            if (UnityEngine.Random.value < startBiome.hillOdds0to1)
            {
                List<Vector3Int> hillPositions = WorldGenUtil.GetClump(pos, UnityEngine.Random.Range(averageHillSize/2, (int) (averageHillSize * 1.5) ));
                List<Vector3Int> bottomTiles = new();

                // add top tiles
                foreach (Vector3Int currentPos in hillPositions)
                {
                    if (Mathf.Abs(currentPos.x) > worldSize || Mathf.Abs(currentPos.y) > worldSize) { continue; }
                    Biome currentBiome = GetBiomeAtPos(currentPos);
                    if (currentBiome.raisedGroundTop == null) { continue; }

                    raisedGround.SetTile(currentPos, currentBiome.raisedGroundTop);
                }
                foreach (Vector3Int currentPos in hillPositions)
                {
                    if (Mathf.Abs(currentPos.x) > worldSize || Mathf.Abs(currentPos.y) > worldSize) { continue; }
                    Biome currentBiome = GetBiomeAtPos(currentPos);
                    if (currentBiome.raisedGroundTop == null) { continue; }

                    // add front tiles
                    int j = 1;
                    for (j = 1; j < height; j++)
                    {
                        if (raisedGround.GetTile(currentPos + (Vector3Int.down * j)) == null)
                        {
                            bottomTiles.Add(currentPos + (Vector3Int.down * j));
                            raisedGround.SetTile(currentPos + (Vector3Int.down * j), currentBiome.raisedGroundFront);
                        }
                        yield return null;
                    }
                }
                foreach (Vector3Int bottomPos in bottomTiles)
                {
                    // add shadows
                    if (raisedGround.GetTile(bottomPos + Vector3Int.down) == null)
                    {
                        raisedGround.SetTile(bottomPos + Vector3Int.down, rgShadowTile);
                    }
                }
            }
        }

        yield return null;
    }

    // this is here, and not in Utils, because it includes random, and this file has the seed set.
    Vector3Int PickRandomTilePos()
    {
        int safeWorldSize = worldSize - 2;
        return new Vector3Int(UnityEngine.Random.Range(-safeWorldSize, safeWorldSize), UnityEngine.Random.Range(-safeWorldSize, safeWorldSize));
    }

    Biome GetBiomeAtPos(Vector3Int position)
    {
        return biomes.Find(x => x.groundTile == ground.GetTile(position));
    }    

    void CheckSpawnObject(Vector3Int location, Biome biome)
    {
        foreach (Biome.DecorObject decorObject in biome.decorObjects)
        {
            if (UnityEngine.Random.value < decorObject.spawnrate0to1)
            {
                Instantiate(decorObject.prefab, location + new Vector3(0.5f, 0.5f), Quaternion.identity, decorObject.typeParent);
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

        public RuleTile groundTile;

        [Tooltip("Leave this empty if this biome does not have hills, and cuts off hills from other biomes that reach into this biome.")]
        public RuleTile raisedGroundTop;
        [Tooltip("Leave this empty if this biome does not have hills, and cuts off hills from other biomes that reach into this biome.")]
        public RuleTile raisedGroundFront;

        public float hillOdds0to1 = 1f;

        [System.NonSerialized] public Transform decorObjectRoot;

        [Tooltip("Input the GameObject for the object, and the chance of it spawning on any particular tile (0 - 1).")]
        public List<DecorObject> decorObjects;

        [Tooltip("Input the tilemaps for the structure, the center of the structure on the tilemap (if it's not (0, 0)), and the frequency.")]
        public List<DecorObject> structures;

        [System.Serializable]
        public class DecorObject
        {
            public string name;
            public GameObject prefab;
            public float spawnrate0to1;
            public bool occupyTiles = true;
            [System.NonSerialized] public Transform typeParent;
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
