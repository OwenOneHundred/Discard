using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GeneralUtil
{
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


}
