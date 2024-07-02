using System;
using System.Collections.Generic;
using Components;
using Extensions.System;
using Extensions.Unity;
using UnityEngine;

public static class GridF
{
    private const int MatchOffset = 2;

    public static void GetSpawnableColors(this Tile[,] grid, Vector2Int coord, List<int> results)
    {
        int lastPrefabID = -1;
        int lastIDCounter = 0;

        int leftMax = coord.x - MatchOffset;
        int rightMax = coord.x + MatchOffset;

        leftMax = ClampInsideGrid(leftMax, grid.GetLength(0));
        rightMax = ClampInsideGrid(rightMax, grid.GetLength(0));

        for(int x = leftMax; x <= rightMax; x ++)
        {
            Tile currTile = grid[x, coord.y];

            if(currTile == null)
            {
                lastIDCounter = 0;
                lastPrefabID = -1;

                continue;
            }

            if(lastPrefabID == -1)
            {
                lastPrefabID = currTile.ID;
                lastIDCounter = 1; // Initialize counter
            }
            else if(lastPrefabID == currTile.ID)
            {
                lastIDCounter ++;
            }
            else
            {
                lastPrefabID = currTile.ID;
                lastIDCounter = 1; // Reset counter to 1 for new tile ID
            }

            if(lastIDCounter == MatchOffset) results.Remove(lastPrefabID);
        }
        
        lastPrefabID = -1;
        lastIDCounter = 0;

        int botMax = coord.y - MatchOffset;
        int topMax = coord.y + MatchOffset;

        botMax = ClampInsideGrid(botMax, grid.GetLength(1));
        topMax = ClampInsideGrid(topMax, grid.GetLength(1));

        for(int y = botMax; y <= topMax; y ++)
        {
            Tile currTile = grid[coord.x, y];

            if(currTile == null)
            {
                lastIDCounter = 0;
                lastPrefabID = -1;

                continue;
            }

            if(lastPrefabID == -1)
            {
                lastPrefabID = currTile.ID;
                lastIDCounter = 1; // Initialize counter
            }
            else if(lastPrefabID == currTile.ID)
            {
                lastIDCounter ++;
            }
            else
            {
                lastPrefabID = currTile.ID;
                lastIDCounter = 1; // Reset counter to 1 for new tile ID
            }

            if(lastIDCounter == MatchOffset) results.Remove(lastPrefabID);
        }
    }

    public static bool TryGetMostBelowEmpty(this Tile[,] thisGrid, Tile thisTile, out Vector2Int belowTileCoords)
    {
        Vector2Int belowCoords = thisTile.Coords;
        belowTileCoords = belowCoords;
        
        belowCoords.y --;

        if(thisGrid.IsInsideGrid(belowCoords) == false) return false;

        if(thisGrid.Get(belowCoords)) return false;
        
        for(int y = belowCoords.y; y < 0; y --)
        {
            Vector2Int thisCoords = new(thisTile.Coords.x, y);
            
            Tile belowTile = thisGrid.Get(thisCoords);

            if(belowTile == false)
            {
                belowTileCoords = thisCoords;
            }
            else
            {
                break;
            }
        }
        
        return true;
    }

    public static List<Tile> GetMatchesY
    (this Tile[,] thisGrid, Tile tile)
        => GetMatchesY(thisGrid, tile.Coords, tile.ID);

    public static List<Tile> GetMatchesY(this Tile[,] grid, Vector2Int coord, int prefabId)
    {
        Tile thisTile = grid.Get(coord);

        List<Tile> matches = new();

        int botMax = coord.y - MatchOffset;
        int topMax = coord.y + MatchOffset + 1;

        int gridLength = grid.GetLength(1);
        int gridMin = 0;

        if(botMax < gridMin) botMax = gridMin;
        if(topMax > gridLength) topMax = gridLength;
        
        for(int y = botMax; y < topMax; y ++)
        {
            Tile currTile = grid[coord.x, y];
            
            if(currTile.ID == prefabId)
            {
                matches.Add(currTile);
            }
            else if(matches.Contains(thisTile) == false)
            {
                matches.Clear();
            }
            else if(matches.Contains(thisTile))
            {
                break;
            }
        }

        if(matches.Count < 3)
        {
            matches.Clear();
        }
        
        return matches;
    }

    public static List<Tile> GetMatchesX
    (this Tile[,] thisGrid, Tile tile)
        => GetMatchesX(thisGrid, tile.Coords, tile.ID);
    
    public static List<Tile> GetMatchesX(this Tile[,] grid, Vector2Int coord, int prefabId)
    {
        Tile thisTile = grid.Get(coord);

        List<Tile> matches = new();

        int leftMax = coord.x - MatchOffset;
        int rightMax = coord.x + MatchOffset + 1;

        int gridLength = grid.GetLength(0);
        int gridMin = 0;

        if(leftMax < gridMin) leftMax = gridMin;
        if(rightMax > gridLength) rightMax = gridLength;
        
        for(int x = leftMax; x < rightMax; x ++)
        {
            Tile currTile = grid[x, coord.y];

            if(currTile.ID == prefabId)
            {
                matches.Add(currTile);
            }
            else if(matches.Contains(thisTile) == false)
            {
                matches.Clear();
            }
            else if(matches.Contains(thisTile))
            {
                break;
            }
        }

        if(matches.Count < 3)
        {
            matches.Clear();
        }
        
        return matches;
    }
    
    public static List<Tile> GetMatchesXAll
    (this Tile[,] thisGrid, Tile tile)
        => GetMatchesXAll(thisGrid, tile.Coords, tile.ID);
    
    public static List<Tile> GetMatchesXAll(this Tile[,] grid, Vector2Int coord, int prefabId)
    {
        Tile thisTile = grid.Get(coord);

        List<Tile> matches = new();

        for(int x = 0; x < grid.GetLength(0); x ++)
        {
            Tile currTile = grid[x, coord.y];

            if(currTile.ID == prefabId)
            {
                matches.Add(currTile);
            }
            else if(matches.Contains(thisTile) == false)
            {
                matches.Clear();
            }
            else if(matches.Contains(thisTile))
            {
                break;
            }
        }

        if(matches.Count < 3)
        {
            matches.Clear();
        }
        
        return matches;
    }

    public static List<Tile> GetMatchesYAll
    (this Tile[,] thisGrid, Tile tile)
        => GetMatchesYAll(thisGrid, tile.Coords, tile.ID);
    public static List<Tile> GetMatchesYAll(this Tile[,] grid, Vector2Int coord, int prefabId)
    {
        Tile thisTile = grid.Get(coord);

        List<Tile> matches = new();

        for(int y = 0; y < grid.GetLength(1); y ++)
        {
            Tile currTile = grid[coord.x, y];

            if(currTile.ID == prefabId)
            {
                matches.Add(currTile);
            }
            else if(matches.Contains(thisTile) == false)
            {
                matches.Clear();
            }
            else if(matches.Contains(thisTile))
            {
                break;
            }
        }

        if(matches.Count < 3)
        {
            matches.Clear();
        }
        
        return matches;
    }

    private static int ClampInsideGrid
    (int value, int gridSize)
    {
        return Mathf.Clamp(value, 0, gridSize - 1);
    }

    public static bool IsInsideGrid(this Tile[,] grid, int axisCoord, int axisIndex)
    {
        const int min = 0;
        int max = grid.GetLength(axisIndex);

        return axisCoord >= min && axisCoord < max;
    }
    
    public static bool IsInsideGrid(this Tile[,] grid, Vector2Int coord)
    {
        return grid.IsInsideGrid(coord.x, 0) && grid.IsInsideGrid(coord.y, 1);
    }

    public static GridDir GetGridDir(Vector3 input)
    {
        int maxAxis = 0;
        float maxAxisSign = input[0].Sign();
        float lastAxisLengthAbs = input[0].Abs();
        
        for(int axisIndex = 0; axisIndex < 3; axisIndex ++)
        {
            float thisAxisLength = input[axisIndex];
            float thisAxisLengthAbs = thisAxisLength.Abs();

            if(thisAxisLengthAbs > lastAxisLengthAbs)
            {
                lastAxisLengthAbs = thisAxisLengthAbs;
                maxAxis = axisIndex;
                maxAxisSign = thisAxisLength.Sign();
            }
        }

        return GetGridDir((maxAxis + 1) * maxAxisSign.CeilToInt());
    }
    
    public static Vector2Int GetGridDirVector(Vector3 input)
    {
        int maxAxis = 0;
        float maxAxisSign = input[0].Sign();
        float lastAxisLengthAbs = input[0].Abs();
        
        for(int axisIndex = 0; axisIndex < 3; axisIndex ++)
        {
            float thisAxisLength = input[axisIndex];
            float thisAxisLengthAbs = thisAxisLength.Abs();

            if(thisAxisLengthAbs > lastAxisLengthAbs)
            {
                lastAxisLengthAbs = thisAxisLengthAbs;
                maxAxis = axisIndex;
                maxAxisSign = thisAxisLength.Sign();
            }
        }

        return GetGridDir((maxAxis + 1) * maxAxisSign.CeilToInt()).ToVector();
    }
    
    /// <summary>
    /// Convert non-zero axis index with sign.
    /// </summary>
    /// <param name="axisSignIndex">Should not start from zero.</param>
    /// <returns>Grid Dir</returns>
    public static GridDir GetGridDir(int axisSignIndex)
    {
        return axisSignIndex switch
        {
            1 => GridDir.Right,
            2 => GridDir.Up,
            -1 => GridDir.Left,
            -2 => GridDir.Down,
            _ => GridDir.Null
        };
    }

    public static Vector2Int ToVector(this GridDir thisGridDir)
    {
        return thisGridDir switch
        {
            GridDir.Null => Vector2Int.zero,
            GridDir.Left => Vector2Int.left,
            GridDir.Right => Vector2Int.right,
            GridDir.Up => Vector2Int.up,
            GridDir.Down => Vector2Int.down,
            _ => throw new ArgumentOutOfRangeException(nameof(thisGridDir), thisGridDir, null)
        };
    }

    public static Tile Get(this Tile[,] thisGrid, Vector2Int coord)
    {
        return thisGrid[coord.x, coord.y];
    }
    
    public static Tile Set(this Tile[,] thisGrid, Tile tileToSet, Vector2Int coord)
    {
        Tile tileAtCoord = thisGrid.Get(coord);

        thisGrid[coord.x, coord.y] = tileToSet;

        if(tileToSet == false) return tileAtCoord;
        
        ITileGrid tileGrid = tileToSet;

        tileGrid.SetCoord(coord);
        
        return tileAtCoord;
    }

    public static void Swap(this Tile[,] thisGrid, Tile fromTile, Vector2Int toCoords)
    {
        Vector2Int fromCoords = fromTile.Coords;
        
        Tile toTile = thisGrid.Set(fromTile, toCoords);
        thisGrid.Set(toTile, fromCoords);
    }
    
    public static void Swap(this Tile[,] thisGrid, Tile fromTile, Tile toTile)
    {
        Vector2Int fromCoords = fromTile.Coords;
        Vector2Int toCoords = toTile.Coords;
        
        thisGrid.Set(fromTile, toCoords);
        thisGrid.Set(toTile, fromCoords);
    }
    
    public static Vector3 CoordsToWorld(this Tile[,] thisGrid, Transform transform, Vector2Int coords)
    {
        Vector3 localPos = coords.ToVector3XY();

        return transform.position + localPos;
    }
}

public enum GridDir
{
    Null,
    Left,
    Right,
    Up,
    Down
}