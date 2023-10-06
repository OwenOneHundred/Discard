using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

[CreateAssetMenu]
public class CustomConnections : RuleTile<CustomConnections.Neighbor>
{
    public RuleTile[] connectableRTs;
    public Tile[] connectableTiles;

    public class Neighbor : RuleTile.TilingRule.Neighbor
    {
        public const int Connectable = 3;
        public const int NotConnectable = 4;
    }

    public override bool RuleMatch(int neighbor, TileBase tile)
    {
        switch (neighbor)
        {
            case Neighbor.Connectable: return tile == this || connectableRTs.Contains(tile) || connectableTiles.Contains(tile);
            case Neighbor.NotConnectable: return !(tile == this || connectableRTs.Contains(tile) || connectableTiles.Contains(tile));
            default:
                break;
        }
        return base.RuleMatch(neighbor, tile);
    }
}