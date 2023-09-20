using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu]
public class BeachTiles : RuleTile<BeachTiles.Neighbor> {
    public bool customField;
    public AnimatedTile[] waterTiles;


    public class Neighbor : RuleTile.TilingRule.Neighbor {
        public const int Water = 3;
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.Water: return waterTiles.Contains(tile);
        }
        return base.RuleMatch(neighbor, tile);
    }


    
}