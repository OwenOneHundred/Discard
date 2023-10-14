using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static WorldGenerator;
using UnityEngine.Tilemaps;

public static class GeneralUtil
{
    public static List<Biome> biomes;
    public static Tilemap ground;

    /// <summary>
    /// Returns index of random float from given list, but with proportionally greater weight given to bigger floats.
    /// </summary>
    /// <returns>Index of chosen float.</returns>
    public static int RandomWeighted(List<float> values)
    {
        if (values.Count == 0) { return 666; }
        if (values.Count == 1) { return 0; }

        float total = values.Sum(x => System.Convert.ToInt32(x));
        float randomValue = UnityEngine.Random.Range(0f, total);
        float tally = 0;
        for (int i = 0; i < values.Count; i++)
        {
            tally += values[i];
            if (tally > randomValue)
            {
                return i;
            }
        }
        return values.Count;
    }

    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void ShuffleList<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }


    public static Biome GetBiomeAtPos(Vector3 position)
    {
        return biomes.Find(x => x.groundTile == ground.GetTile(ground.WorldToCell(position)));
    }

    public static float AngleBetween(Vector2 vector1, Vector2 vector2)
    {
        float sin = vector1.x * vector2.y - vector2.x * vector1.y;
        float cos = vector1.x * vector2.x + vector1.y * vector2.y;

        return Mathf.Atan2(sin, cos) * (180 / Mathf.PI);
    }
}
