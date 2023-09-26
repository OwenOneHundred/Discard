using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class WorldGenerator : MonoBehaviour
{
    int seed;
    Vector2 perlinCenter;

    [SerializeField] Tilemap ground;
    [SerializeField] Tilemap raisedGround;

    [SerializeField] List<Tilemap> structureTilemapsDNR;

    [SerializeField] RuleTile rgShadowTile;

    [SerializeField] int worldSize;
    [SerializeField] int genPerFrame = 20;

    [SerializeField] float biomeSize;
    [SerializeField] int hillSpawnAttempts = 30;
    [SerializeField] int averageHillSize = 20;

    public List<Biome> biomes;

    public event Action<Vector3Int, Biome> CallCustomScripts;

    [System.NonSerialized] public List<Vector3Int> doNotSpawnTiles = new();
    [System.NonSerialized] public List<Vector3Int> decorObjectTiles = new();

    [SerializeField] bool Debug_DoNotSpawnObjs = false;
    [SerializeField] bool Debug_DoNotSpawnHills = false;
    [SerializeField] bool Debug_DoNotSpawnStructures = false;

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

    // operation order:
    // Generate biome tiles
    // Generate hills
    // Generate structures
    // Generate decor objects
    IEnumerator GenerateWorld()
    {
        yield return StartCoroutine(GenerateBiomeTiles(worldSize, Vector2.zero));

        if (!Debug_DoNotSpawnHills) { yield return StartCoroutine(GenerateHills()); }

        if (!Debug_DoNotSpawnStructures) { yield return StartCoroutine(StructureSpawner()); }

        if (!Debug_DoNotSpawnObjs) { yield return StartCoroutine(ObjectSpawner()); }
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
                biomes[biomeIndex].numberOfTiles += 1;

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
            Vector3Int pos = WorldGenUtil.PickRandomTilePos(worldSize);
            Biome startBiome = GetBiomeAtPos(pos);

            if (startBiome.raisedGroundTop == null || startBiome.hillOdds0to1 <= 0) { continue; }

            int height = UnityEngine.Random.Range(2, 5);

            if (UnityEngine.Random.value < startBiome.hillOdds0to1)
            {
                List<Vector3Int> hillPositions = WorldGenUtil.GetClump(pos, UnityEngine.Random.Range(averageHillSize/2, (int) (averageHillSize * 1.5) ));
                List<Vector3Int> bottomTiles = new();

                // remove invalid positions
                List<Vector3Int> dummyList = new List<Vector3Int>(hillPositions);
                foreach (Vector3Int hillPos in dummyList)
                {
                    if (Mathf.Abs(hillPos.x) > worldSize || Mathf.Abs(hillPos.y) > worldSize)
                    {
                        hillPositions.Remove(hillPos);
                        continue;
                    }

                    if (GetBiomeAtPos(hillPos).raisedGroundTop == null)
                    {
                        hillPositions.Remove(hillPos);
                        continue;
                    }
                }

                // add top tiles
                foreach (Vector3Int currentPos in hillPositions)
                {
                    Biome currentBiome = GetBiomeAtPos(currentPos);
                    raisedGround.SetTile(currentPos, currentBiome.raisedGroundTop);

                    // add edge tiles and immediate borders to doNotSpawn list
                    List<Vector3Int> topTileSurrounding = WorldGenUtil.GetSurroundingTilePositions(currentPos);
                    foreach (Vector3Int surroundingTile in topTileSurrounding)
                    {
                        if (raisedGround.GetTile(surroundingTile) == null)
                        {
                            doNotSpawnTiles.AddRange(topTileSurrounding);
                        }
                    }
                }

                // add front tiles
                foreach (Vector3Int currentPos in hillPositions)
                {
                    Biome currentBiome = GetBiomeAtPos(currentPos);

                    int j = 1;
                    for (j = 1; j < height; j++)
                    {
                        Vector3Int bottomPos = currentPos + (Vector3Int.down * j);
                        TileBase tileInTheWay = raisedGround.GetTile(bottomPos);
                        if (tileInTheWay == null || tileInTheWay == rgShadowTile)
                        {
                            bottomTiles.Add(bottomPos);
                            if (!doNotSpawnTiles.Contains(bottomPos)) { doNotSpawnTiles.Add(bottomPos); }
                            raisedGround.SetTile(bottomPos, currentBiome.raisedGroundFront);
                        }
                    }
                }

                // add shadows
                foreach (Vector3Int bottomPos in bottomTiles)
                {
                    if (raisedGround.GetTile(bottomPos + Vector3Int.down) == null)
                    {
                        raisedGround.SetTile(bottomPos + Vector3Int.down, rgShadowTile);
                    }
                }
            }
        }

        yield return null;
    }

    IEnumerator StructureSpawner()
    {
        foreach (Biome biome in biomes)
        {
            if (biome.averageNumTilesForStructure == 0) { continue; }

            int numStructures = biome.numberOfTiles / biome.averageNumTilesForStructure;
            numStructures += UnityEngine.Random.Range(-numStructures / 2, (numStructures / 2) + 1);

            for (int i = 0; i < numStructures; i++)
            {
                List<float> structureWeights = new();
                foreach (Biome.Structure structure in biome.structures) { structureWeights.Add(structure.weightInBiome); }

                Biome.Structure selectedStructure = biome.structures[GeneralUtil.RandomWeighted(structureWeights)];
                Debug.Log(selectedStructure.GetBiggestBounds().size);
                SpawnStructure(selectedStructure, GetRandomFreeAreaInBiomeViaRandom(biome, selectedStructure.GetBiggestBounds().size));

                yield return null;
            }
        }
    }

    void SpawnStructure(Biome.Structure structure, Vector3Int selectedPosition)
    {
        GameObject newTilemaps = Instantiate(structure.prefab);
        Debug.Log("Spawn structure at " + selectedPosition);

        int budgetEnum = 0;
        foreach (Transform i in newTilemaps.transform)
        {
            Tilemap toTilemap = structureTilemapsDNR[budgetEnum];
            Tilemap fromTilemap = i.GetComponent<Tilemap>();

            fromTilemap.CompressBounds();

            BoundsInt bounds = fromTilemap.cellBounds;
            Vector2Int xVector = new Vector2Int(bounds.xMin, bounds.xMax);
            Vector2Int yVector = new Vector2Int(bounds.yMin, bounds.yMax);

            for (int x = xVector.x; x <= xVector.y; x++)
            {
                for (int y = yVector.x; y <= yVector.y; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    toTilemap.SetTile(pos - structure.center + selectedPosition, fromTilemap.GetTile(pos));
                    doNotSpawnTiles.Add(pos);
                }
            }

            budgetEnum += 1;
        }

        Destroy(newTilemaps);
    }

    IEnumerator ObjectSpawner()
    {
        for (int x = -worldSize; x < worldSize; x++)
        {
            for (int y = -worldSize; y < worldSize; y++)
            {
                Vector3Int currentCell = new Vector3Int(x, y, 0);

                if (!doNotSpawnTiles.Contains(currentCell) && !decorObjectTiles.Contains(currentCell))
                {
                    RunSpawnObjects(currentCell, GetBiomeAtPos(currentCell));
                }
            }

            yield return null;
        }
    }

    void RunSpawnObjects(Vector3Int location, Biome biome)
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

    Vector3Int GetRandomTileOfBiomeViaRandom(Biome biome, int maxAttempts = 300)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3Int pos = WorldGenUtil.PickRandomTilePos(worldSize);
            if (GetBiomeFromTile(ground.GetTile(pos)) == biome)
            {
                return pos;
            }
        }
        return new Vector3Int(666, 666, 666);
    }

    Vector3Int GetRandomFreeAreaInBiomeViaRandom(Biome biome, Vector3Int size, int maxAttempts = 300)
    {
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3Int pos = WorldGenUtil.PickRandomTilePos(worldSize - (size.x / 2));
            if (GetBiomeFromTile(ground.GetTile(pos)) == biome)
            {
                if (CheckAreaForSpawn(pos))
                {
                    return pos;
                }
                else
                {
                    continue;
                }
            }
        }
        return new Vector3Int(666, 666, 666);

        bool CheckAreaForSpawn(Vector3Int pos)
        {
            int x = pos.x - (size.x / 2);
            int y = pos.y - (size.y / 2);
            int yMin = y;
            int xMax = pos.x + (size.x / 2);
            int yMax = pos.y + (size.y / 2);

            for (; x < xMax; x++)
            {
                for (; y < yMax; y++)
                {
                    if (doNotSpawnTiles.Contains(new Vector3Int (x, y, 0))) { return false; }
                }
                y = yMin;
            }

            return true;
        }
    }

    Biome GetBiomeAtPos(Vector3Int position)
    {
        return biomes.Find(x => x.groundTile == ground.GetTile(position));
    }

    public Biome GetBiomeFromTile(TileBase tile)
    {
        return biomes.Find(x => x.groundTile == tile);
    }

    [System.Serializable]
    public class Biome
    {
        public string name;

        public int temperature;

        public int weight = 1;

        public int averageNumTilesForStructure = 1000;

        [System.NonSerialized] public float relativeWeight = 0;
        [System.NonSerialized] public float thresholdWeight = 0;

        public RuleTile groundTile;

        [Tooltip("Leave this empty if this biome does not have hills, and cuts off hills from other biomes that reach into this biome.")]
        public RuleTile raisedGroundTop;
        [Tooltip("Leave this empty if this biome does not have hills, and cuts off hills from other biomes that reach into this biome.")]
        public RuleTile raisedGroundFront;

        public float hillOdds0to1 = 1f;

        [System.NonSerialized] public int numberOfTiles = 0;

        [System.NonSerialized] public Transform decorObjectRoot;

        [Tooltip("Input the GameObject for the object, and the chance of it spawning on any particular tile (0 - 1).")]
        public List<DecorObject> decorObjects;

        [Tooltip("Input the tilemaps for the structure, the center of the structure on the tilemap (if it's not (0, 0)), and the frequency.")]
        public List<Structure> structures;

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
            public GameObject prefab;
            public float weightInBiome = 1;
            public Vector3Int center;

            public BoundsInt GetBiggestBounds()
            {
                BoundsInt biggestBounds = new BoundsInt(0, 0, 0, 0, 0, 0);
                if (prefab == null) { return biggestBounds; }
                foreach (Transform trans in prefab.transform)
                {
                    Tilemap tilemap = trans.GetComponent<Tilemap>();
                    tilemap.CompressBounds();
                    if (biggestBounds == null) { biggestBounds = tilemap.cellBounds; }
                    if (tilemap.cellBounds.size.magnitude > biggestBounds.size.magnitude)
                    {
                        biggestBounds = tilemap.cellBounds;
                    }
                }
                return biggestBounds;
            }
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
