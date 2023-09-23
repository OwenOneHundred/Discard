using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlainsCustomScript : MonoBehaviour
{
    // just change "Plains" to the biome name if you copied this script and want to use it for a different biome, and it'll work
    readonly string biomeName = "Plains";

    WorldGenerator.Biome representedBiome;
    WorldGenerator wg;

    private void Start()
    {
        wg = gameObject.GetComponent<WorldGenerator>();
        representedBiome = wg.biomes.Find(x => x.name == biomeName);

        wg.CallCustomScripts += OnTileGenerated;
    }

    public void OnTileGenerated(Vector3Int position, WorldGenerator.Biome biome)
    {
        if (!biome.Equals(representedBiome))
        {
            return;
        }


        // any code down here will be called any time a grass tile is generated

    }
}
