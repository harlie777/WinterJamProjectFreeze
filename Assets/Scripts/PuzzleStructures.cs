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

public class PuzzleData {
    public int gridSize;
    public Vector2Int playerStart;
    public Vector2Int goal;
    public Dictionary<Vector2Int, TileType> tiles = new();
    public HashSet<Vector2Int> validPositions = new();
    public HashSet<Vector2Int> iceBlocks = new();
    // public HashSet<Vector2Int> Obstacles = new();

    public PuzzleData(int size)
    {
        gridSize = size;
    }

    public bool InBounds(Vector2Int pos) => validPositions.Contains(pos);

    public TileType GetTileAt(Vector2Int pos) {
        return tiles.TryGetValue(pos, out var type) ? type : TileType.Empty;
    }

    public void SetTile(Vector2Int pos, TileType type) {
        if (type == TileType.Empty) tiles.Remove(pos);
        else tiles[pos] = type;
    }
}
