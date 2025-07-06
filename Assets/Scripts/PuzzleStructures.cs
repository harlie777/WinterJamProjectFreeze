using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TileType
{
    Empty,
    Obstacle,
    IceBlock,
    Goal,
    PlayerStart,
    Wall
}




// Puzzle States are used by the solver
public class PuzzleState
{
    public Vector2Int playerPos;
    public HashSet<Vector2Int> blockPositions;
    public int moveCount;

    public PuzzleState(Vector2Int pos, HashSet<Vector2Int> blocks, int moves = 0)
    {
        playerPos = pos;
        blockPositions = new HashSet<Vector2Int>(blocks);
        moveCount = moves;
    }

    public PuzzleState Copy()
    {
        return new PuzzleState(playerPos, new HashSet<Vector2Int>(blockPositions), moveCount + 1);
    }

    public override bool Equals(object obj)
    {
        if (obj is not PuzzleState other) return false;
        return playerPos == other.playerPos && blockPositions.SetEquals(other.blockPositions);
    }

    public override int GetHashCode()
    {
        int hash = playerPos.GetHashCode();
        foreach (var pos in blockPositions) hash ^= pos.GetHashCode();
        return hash;
    }
}

public class PuzzleData
{
    public int gridSize;
    public Vector2Int playerStart;
    public Vector2Int goal;
    public Dictionary<Vector2Int, TileType> tiles = new();
    public HashSet<Vector2Int> validPositions = new();
    public HashSet<Vector2Int> iceBlocks = new();
    // public HashSet<Vector2Int> Obstacles = new();

    // Metadata for scoring
    public int minMovesToSolve;
    // public int numDeadEnds;
    // public float avgBranchingFactor;
    // public int difficultyScore;

    // public PuzzleData(int size)
    // {
    //     gridSize = size;
    // }
    public PuzzleData(int size)
    {
        GridTemplates.ApplyTo(this, size);
    }

    public bool InBounds(Vector2Int pos) => validPositions.Contains(pos);

    public TileType GetTileAt(Vector2Int pos)
    {
        return tiles.TryGetValue(pos, out var type) ? type : TileType.Empty;
    }

    public void SetTile(Vector2Int pos, TileType type)
    {
        if (type == TileType.Empty) tiles.Remove(pos);
        else tiles[pos] = type;
    }

//     public void SetGridAsEmpty(int gridSize)
//     {
        
//     }
}


public static class GridTemplates
{
    private static readonly Dictionary<int, (List<Vector2Int> positions, Dictionary<Vector2Int, TileType> tiles)> cachedGrids = new();

    static GridTemplates()
    {
        for (int size = 3; size <= 20; size++)
        {
            var positions = new List<Vector2Int>(size * size);
            var tiles = new Dictionary<Vector2Int, TileType>(size * size);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Vector2Int pos = new(x, y);
                    positions.Add(pos);
                    tiles[pos] = TileType.Empty;
                }
            }

            cachedGrids[size] = (positions, tiles);
        }
    }

    public static void ApplyTo(PuzzleData puzzle, int gridSize)
    {
        if (!cachedGrids.TryGetValue(gridSize, out var template))
        {
            Debug.LogError($"Grid template not found for size {gridSize}");
            return;
        }

        puzzle.gridSize = gridSize;
        puzzle.validPositions = new HashSet<Vector2Int>(template.positions);
        puzzle.tiles = new Dictionary<Vector2Int, TileType>(template.tiles);
    }
}