using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.WSA;

public static class WorldGenUtil
{
    public static bool CheckForTileInMultipleTilemaps(Vector3Int location, List<Tilemap> tilemaps)
    {
        foreach (Tilemap tilemap in tilemaps)
        {
            TileBase tile = tilemap.GetTile(location);

            if (tile != null)
            {
                return true;
            }
        }

        return false;
    }

    public static TileBase GetTileBaseInMultipleTilemaps(Vector3Int location, List<Tilemap> tilemaps)
    {
        TileBase tile = null;
        foreach (Tilemap tilemap in tilemaps)
        {
            tile = tilemap.GetTile(location);

            if (tile != null)
            {
                return tile;
            }
        }

        return tile;
    }

    public static List<Vector3Int> GetSurroundingTilePositions(Vector3Int center, bool includeCenter = true, bool includeDiagonals = true)
    {
        List<Vector3Int> toReturn = new List<Vector3Int>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x != 0 && y != 0 && !includeDiagonals) { continue; }
                if (x == 0 && y == 0 && !includeCenter) { continue; }

                toReturn.Add(new Vector3Int(center.x + x, center.y + y, 0));
            }
        }

        return toReturn;
    }

    public static List<Vector3Int> FindCenterPointsOfSquareSections(Vector3Int bottomLeft, Vector3Int topRight, int count)
    {
        if (bottomLeft.x > topRight.x || bottomLeft.y > topRight.y)
        {
            Debug.LogWarning("Called FindCenterPointsOfSquareSections with corner points.");
        }
        double result = Mathf.Sqrt(count);
        if (result % 1 != 0)
        {
            Debug.LogWarning("Called FindCenterPointsOfSquareSections with count that is not a perfect square.");
        }

        int cuts = Mathf.FloorToInt(Mathf.Sqrt(count));

        List<Vector3Int> toReturn = new List<Vector3Int>();
        int squareHeight = (topRight.x - bottomLeft.x) / cuts;

        for (int x = 1; x <= cuts; x++)
        {
            for (int y = 1; y <= cuts; y++)
            {
                Vector3Int topRightOfSquare = bottomLeft + new Vector3Int(squareHeight * x, squareHeight * y, 0);
                Vector3Int bottomLeftOfSquare = topRightOfSquare - new Vector3Int(squareHeight, squareHeight, 0);
                toReturn.Add((bottomLeftOfSquare + topRightOfSquare) / 2);
            }
        }

        return toReturn;
    }

    /// <summary>
    /// Spawns clump of tiles at position of random size and random but generally circular shape. ONLY works if tilemap is EMPTY otherwise.
    /// </summary>
    public static void CreateClump(Vector3Int center, int tileCount, TileBase tile, Tilemap tilemap)
    {
        for (int tileNumber = 0; tileNumber < tileCount; tileNumber++)
        {
            int x = 0;
            int y = 0;
            int xDirection = Random.Range(0, 2) == 0 ? -1 : 1;
            int yDirection = Random.Range(0, 2) == 0 ? -1 : 1;
            for (int tryPlaceTile = 0; tryPlaceTile < 100; tryPlaceTile++)
            {
                if (tilemap.GetTile(center + new Vector3Int(x, y, 0)) == null)
                {
                    Vector3Int chosenCell = center + new Vector3Int(x, y, 0);
                    tilemap.SetTile(chosenCell, tile);
                    foreach (Vector3Int i in GetSurroundingTilePositions(chosenCell, false))
                    {
                        int surroundingTileCount = 0;
                        foreach (Vector3Int j in GetSurroundingTilePositions(i, false, false))
                        {
                            if (tilemap.GetTile(j) != null)
                            {
                                surroundingTileCount += 1;
                            }
                        }

                        if (surroundingTileCount > 1)
                        {
                            tilemap.SetTile(i, tile);
                        }
                    }

                    break;
                }

                if (Random.Range(0, 2) == 0)
                {
                    x += xDirection;
                }
                else
                {
                    y += yDirection;
                }
            }
        }
    }

    public static List<Vector3Int> GetClump(Vector3Int center, int tileCount)
    {
        List<Vector3Int> toReturn = new List<Vector3Int>();

        for (int tileNumber = 0; tileNumber < tileCount; tileNumber++)
        {
            int x = 0;
            int y = 0;
            int xDirection = Random.Range(0, 2) == 0 ? -1 : 1;
            int yDirection = Random.Range(0, 2) == 0 ? -1 : 1;

            for (int tryPlaceTile = 0; tryPlaceTile < 100; tryPlaceTile++)
            {
                if (!toReturn.Contains(center + new Vector3Int(x, y, 0)))
                {
                    Vector3Int chosenCell = center + new Vector3Int(x, y, 0);
                    toReturn.Add(chosenCell);

                    foreach (Vector3Int i in GetSurroundingTilePositions(chosenCell, false))
                    {
                        int surroundingTileCount = 0;
                        foreach (Vector3Int j in GetSurroundingTilePositions(i, false, false))
                        {
                            if (toReturn.Contains(j))
                            {
                                surroundingTileCount += 1;
                            }
                        }

                        if (surroundingTileCount > 1)
                        {
                            if (!toReturn.Contains(i))
                            {
                                toReturn.Add(i);
                            }
                        }
                    }

                    break;
                }

                if (Random.Range(0, 2) == 0)
                {
                    x += xDirection;
                }
                else
                {
                    y += yDirection;
                }
            }
        }

        return toReturn;
    }



}