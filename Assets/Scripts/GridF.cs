using System.Collections.Generic;
using Components;
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

    public static bool HasMatchesRight(this Tile[,] grid, Vector2Int abstractCoord, int prefabId)
    {
        if(grid.IsInsideGrid(abstractCoord))
        {
            
        }

        return default;
    }
    
    private static bool HasMatchRight(this Tile[,] grid, Vector2Int coord, int prefabId)
    {
        int rightMax = coord.x + MatchOffset;
        rightMax = ClampInsideGrid(rightMax, grid.GetLength(0));

        int matchCounter = 0;
        
        for(int x = coord.x; x <= rightMax; x ++)
        {
            if(grid[x, coord.y].ID == prefabId)
            {
                matchCounter ++;
            }
            else
            {
                matchCounter = 0;
            }

            if(matchCounter == 3)
            {
                return true;
            }
        }

        return false;
    }
    
    private static bool HasMatchLeft(this Tile[,] grid, Vector2Int coord, int prefabId)
    {
        int leftMax = coord.x - MatchOffset;
        leftMax = ClampInsideGrid(leftMax, grid.GetLength(0));

        int matchCounter = 0;
        
        for(int x = leftMax; x <= coord.x; x ++)
        {
            if(grid[x, coord.y].ID == prefabId)
            {
                matchCounter ++;
            }
            else
            {
                matchCounter = 0;
            }

            if(matchCounter == 3)
            {
                return true;
            }
        }

        return false;
    }
    private static bool HasMatchTop(this Tile[,] grid, Vector2Int coord, int prefabId)
    {
        int topMax = coord.y + MatchOffset;
        topMax = ClampInsideGrid(topMax, grid.GetLength(1));

        int matchCounter = 0;
        
        for(int y = coord.y; y <= topMax; y ++)
        {
            if(grid[coord.x, y].ID == prefabId)
            {
                matchCounter ++;
            }
            else
            {
                matchCounter = 0;
            }

            if(matchCounter == 3)
            {
                return true;
            }
        }

        return false;
    }
    
    private static bool HasMatchBot(this Tile[,] grid, Vector2Int coord, int prefabId)
    {
        int botMax = coord.y - MatchOffset;
        botMax = ClampInsideGrid(botMax, grid.GetLength(1));

        int matchCounter = 0;
        
        for(int y = botMax; y <= coord.y; y ++)
        {
            if(grid[coord.x, y].ID == prefabId)
            {
                matchCounter ++;
            }
            else
            {
                matchCounter = 0;
            }

            if(matchCounter == 3)
            {
                return true;
            }
        }

        return false;
    }
    private static int ClampInsideGrid
    (int value, int gridSize)
    {
        return Mathf.Clamp(value, 0, gridSize - 1);
    }

    private static bool IsInsideGrid(this Tile[,] grid, int axis, int axisIndex)
    {
        int min = 0;
        int max = grid.GetLength(axisIndex);
        
        return axis >= 0 && axis < max;
    }
    
    private static bool IsInsideGrid(this Tile[,] grid, Vector2Int coord)
    {
        return grid.IsInsideGrid(coord.x, 0) && grid.IsInsideGrid(coord.y, 1);
    }
}