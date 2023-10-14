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
    [SerializeField] Tilemap slopes;
    [SerializeField] Tilemap tallGrassTilemap;

    [SerializeField] List<Tilemap> structureTilemapsDNR;

    [SerializeField] RuleTile rgShadowTile;
    [SerializeField] Tile connectNotCollideTile;

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
    [SerializeField] bool Debug_DoNotSpawnSlopes = false;
    [SerializeField] bool Debug_DoNotSpawnBorder = false;

    [SerializeField] bool Debug_RunGenTimer = false;
    private float worldGenTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (Debug_RunGenTimer) { worldGenTimer = Time.time; }

        seed = UnityEngine.Random.Range(-9999999, 10000000);
        UnityEngine.Random.InitState(seed);
        perlinCenter = new Vector2(UnityEngine.Random.Range(-999999f, 999999f), UnityEngine.Random.Range(-999999f, 999999f));

        SetUpGeneralUtil();

        SetUpBiomeHierarchy();

        SetUpBiomeWeights();

        StartCoroutine(GenerateWorld());
    }

    void SetUpGeneralUtil()
    {
        GeneralUtil.biomes = biomes;
        GeneralUtil.ground = ground;
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

        // also spawns splotches
        if (!Debug_DoNotSpawnObjs) { yield return StartCoroutine(ObjectSpawner()); }

        if (!Debug_DoNotSpawnBorder) { yield return StartCoroutine(SpawnBorder()); }

        if (Debug_RunGenTimer) { Debug.Log("Generation completed in " + (Time.time - worldGenTimer) + " seconds."); }
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

    // this has to be the first thing with collision generated or it will break. Could be changed tho
    IEnumerator GenerateHills()
    {
        List<Hill> hills = new List<Hill>();
        for (int i = 0; i < hillSpawnAttempts; i++)
        {
            hills.Add(new Hill());
            Hill currentHill = hills[i];

            Vector3Int pos = WorldGenUtil.PickRandomTilePos(worldSize);
            Biome startBiome = GetBiomeAtPos(pos);

            if (startBiome.raisedGroundTop == null || startBiome.hillOdds0to1 <= 0) { continue; }

            if (!(UnityEngine.Random.value < startBiome.hillOdds0to1))
            {
                continue;
            }

            currentHill.height = UnityEngine.Random.Range(2, 5);

            List<Vector3Int> hillPositions = WorldGenUtil.GetClump(pos, UnityEngine.Random.Range(averageHillSize / 2, (int)(averageHillSize * 1.5)));

            // remove invalid positions
            List<Vector3Int> dummyList = new List<Vector3Int>(hillPositions);
            foreach (Vector3Int hillPos in dummyList)
            {
                if (Mathf.Abs(hillPos.x) > worldSize || Mathf.Abs(hillPos.y) > worldSize)
                {
                    hillPositions.Remove(hillPos);
                    continue;
                }

                if (GetBiomeAtPos(hillPos) != startBiome)
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
                for (j = 1; j < currentHill.height; j++)
                {
                    Vector3Int bottomPos = currentPos + (Vector3Int.down * j);
                    TileBase tileInTheWay = raisedGround.GetTile(bottomPos);
                    if (tileInTheWay == null || tileInTheWay == rgShadowTile)
                    {
                        currentHill.bottomCenters.Add(bottomPos);
                        if (!doNotSpawnTiles.Contains(bottomPos)) { doNotSpawnTiles.Add(bottomPos); }
                        raisedGround.SetTile(bottomPos, currentBiome.raisedGroundFront);
                    }
                }
            }

            // add shadows
            // uses list populated by AddFrontTiles
            List<Vector3Int> bottomCentersCopy = new List<Vector3Int>(currentHill.bottomCenters);
            foreach (Vector3Int bottomPos in bottomCentersCopy)
            {
                if (!(raisedGround.GetTile(bottomPos + Vector3Int.down) == null)) { continue; }

                raisedGround.SetTile(bottomPos + Vector3Int.down, rgShadowTile);

                // populate bottom corner tile lists
                if (raisedGround.GetTile(bottomPos + Vector3Int.right) == null || raisedGround.GetTile(bottomPos + Vector3Int.right) == rgShadowTile)
                {
                    currentHill.bottomRCorners.Add(bottomPos);
                }
                else if (raisedGround.GetTile(bottomPos + Vector3Int.left) == null || raisedGround.GetTile(bottomPos + Vector3Int.left) == rgShadowTile)
                {
                    currentHill.bottomLCorners.Add(bottomPos);
                }
                else
                {
                    currentHill.bottomCenters.Add(bottomPos);
                }    
            }

            yield return null;
        }
        // end of once per hill loop

        if (!Debug_DoNotSpawnSlopes)
        {
            // add slopes
            int budgetEnum = 0;
            foreach (Hill hill in hills)
            {
                int slopeNum = UnityEngine.Random.Range(1, 3);

                GeneralUtil.ShuffleList(hill.bottomCenters);
                if (slopeNum > hill.bottomCenters.Count) { slopeNum = hill.bottomCenters.Count; }
                for (int count = 0; count < slopeNum; count++)
                {
                    Vector3Int current = hill.bottomCenters[count];
                    Debug.Log("current: " + current);

                    Biome biome = GetBiomeAtPos(current);

                    if (biome == null) { continue; }
                    if (biome.bottomSlope == null) { continue; }

                    for (int slopeHeight = -1; slopeHeight < hill.height; slopeHeight++)
                    {
                        slopes.SetTile(current, biome.bottomSlope);
                        raisedGround.SetTile(current, connectNotCollideTile);
                        current += Vector3Int.up;
                    }
                }

                budgetEnum += 1;
            }
        }

        foreach (Hill hill in hills)
        {
            AddHillSides(hill.bottomLCorners, hill.bottomRCorners);
        }
    }

    void AddHillSides(List<Vector3Int> bottomLCorners, List<Vector3Int> bottomRCorners)
    {
        List<List<Vector3Int>> combined = new List<List<Vector3Int>>
        {
            bottomLCorners,
            bottomRCorners
        };

        int budgetEnum = 0;
        // add wall center edge tiles using corner tile lists (because there's no other way to do that for some reason)
        foreach (List<Vector3Int> list in combined)
        {
            foreach (Vector3Int bottom in list)
            {
                if (raisedGround.GetTile(bottom) == null || raisedGround.GetTile(bottom) == rgShadowTile) { continue; }
                if (budgetEnum == 0)
                {
                    if (!(raisedGround.GetTile(bottom + Vector3Int.left) == null || raisedGround.GetTile(bottom + Vector3Int.left) == rgShadowTile)) { continue; }
                }
                else
                {
                    if (!(raisedGround.GetTile(bottom + Vector3Int.right) == null || raisedGround.GetTile(bottom + Vector3Int.right) == rgShadowTile)) { continue; }
                }

                Vector3Int current = bottom;
                Biome above = GetBiomeFromHillFront(raisedGround.GetTile(current + Vector3Int.up));
                while (above != null)
                {
                    if (budgetEnum == 0) { raisedGround.SetTile(current + Vector3Int.up, above.rgFrontL); }
                    else { raisedGround.SetTile(current + Vector3Int.up, above.rgFrontR); }

                    current += Vector3Int.up;
                    above = GetBiomeFromHillFront(raisedGround.GetTile(current + Vector3Int.up));
                }
            }

            budgetEnum += 1;
        }
    }

    IEnumerator StructureSpawner()
    {
        foreach (Biome biome in biomes)
        {
            if (biome.averageNumTilesForStructure == 0) { continue; }
            if (biome.structures.Count == 0) { continue; }

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
            List<Vector3Int> xValueList = new List<Vector3Int> (doNotSpawnTiles.Where(f => f.x == x));

            for (int y = -worldSize; y < worldSize; y++)
            {
                Vector3Int currentCell = new Vector3Int(x, y, 0);

                if (!xValueList.Contains(currentCell))
                {
                    Biome biomeAtPos = GetBiomeAtPos(currentCell);
                    RunSpawnSplotches(currentCell, biomeAtPos);
                    RunSpawnObjects(currentCell, biomeAtPos);
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
                Instantiate(decorObject.prefab, location + new Vector3(0.5f, 0.5f), Quaternion.identity, decorObject.typeParent).tag = "DecorObj";
                return;
            }
        }
    }

    void RunSpawnSplotches(Vector3Int location, Biome biome)
    {
        if (biome.tallGrass.tile == null) { return; }
        if (biome.tallGrass.odds0to1 > UnityEngine.Random.value)
        {
            List<Vector3Int> positions = WorldGenUtil.GetClump(location, UnityEngine.Random.Range(15, 22));
            foreach (Vector3Int pos in positions)
            {
                tallGrassTilemap.SetTile(pos, biome.tallGrass.tile);
                doNotSpawnTiles.Add(pos);
            }
        }
    }

    IEnumerator SpawnBorder()
    {
        // bottom and top walls
        for (int x = -worldSize; x < worldSize; x++)
        {
            raisedGround.SetTile(new Vector3Int(x, -worldSize, 0), biomes[0].raisedGroundTop);
            raisedGround.SetTile(new Vector3Int(x, worldSize + 2, 0), biomes[0].raisedGroundTop);
            for (int yAdjust = -1; yAdjust > -4; yAdjust--)
            {
                raisedGround.SetTile(new Vector3Int(x, -worldSize + yAdjust, 0), biomes[0].raisedGroundTop);
                raisedGround.SetTile(new Vector3Int(x, worldSize + 2 + yAdjust, 0), biomes[0].raisedGroundFront);
            }
        }
        for (int y = -worldSize; y < worldSize; y++)
        {
            raisedGround.SetTile(new Vector3Int(-worldSize, y, 0), biomes[0].raisedGroundTop);
            raisedGround.SetTile(new Vector3Int(worldSize, y, 0), biomes[0].raisedGroundTop);
            for (int xAdjust = -1; xAdjust > -4; xAdjust--)
            {
                raisedGround.SetTile(new Vector3Int(-worldSize + xAdjust, y, 0), biomes[0].raisedGroundTop);
                raisedGround.SetTile(new Vector3Int(worldSize - xAdjust, y, 0), biomes[0].raisedGroundTop);
            }
        }
        yield return null;
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

    Biome GetBiomeFromHillFront(TileBase front)
    {
        return biomes.Find(x => x.raisedGroundFront == front);
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

        public float weight = 1;

        public int averageNumTilesForStructure = 1000;

        [System.NonSerialized] public float relativeWeight = 0;
        [System.NonSerialized] public float thresholdWeight = 0;

        public RuleTile groundTile;

        [Tooltip("Leave this empty if this biome does not have hills, and cuts off hills from other biomes that reach into this biome.")]
        public RuleTile raisedGroundTop;
        [Tooltip("Leave this empty if this biome does not have hills, and cuts off hills from other biomes that reach into this biome.")]
        public RuleTile raisedGroundFront;
        public Tile rgFrontL;
        public Tile rgFrontR;
        public RuleTile bottomSlope;
        public RuleTile topSlope;

        public float hillOdds0to1 = 1f;

        [System.NonSerialized] public int numberOfTiles = 0;

        [System.NonSerialized] public Transform decorObjectRoot;

        [Tooltip("Input the GameObject for the object, and the chance of it spawning on any particular tile (0 - 1).")]
        public List<DecorObject> decorObjects;

        [Tooltip("Input the tilemaps for the structure, the center of the structure on the tilemap (if it's not (0, 0)), and the frequency.")]
        public List<Structure> structures;

        public Splotch tallGrass;

        public List<MiniSfXManager.Sound> footstepSounds;
        public MiniSfXManager.Sound ambientNoise;

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
            public float odds0to1 = 0.0001f;
        }
    }

    public class Hill
    {
        public List<Vector3Int> bottomCenters = new List<Vector3Int>();
        public List<Vector3Int> bottomLCorners = new List<Vector3Int>();
        public List<Vector3Int> bottomRCorners = new List<Vector3Int>();
        public int height = 2;
    }

}
