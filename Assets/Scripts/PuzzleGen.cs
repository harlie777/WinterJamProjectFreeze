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

public class PuzzleGenerator {
    public static PuzzleData Generate(int difficulty) {
        int gridSize = Mathf.Min(3 + difficulty / 2, 7);
        int numBlocks = Mathf.Min(2 + difficulty / 3, 6);

        PuzzleData puzzle = new(gridSize);

        // Define valid shape (square by default)
        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                var pos = new Vector2Int(x, y);
                puzzle.validPositions.Add(pos);
                puzzle.SetTile(pos, TileType.Empty);
            }
        }

        // Place player and goal
        puzzle.playerStart = new Vector2Int(0, 0);
        puzzle.goal = new Vector2Int(gridSize - 1, gridSize - 1);
        puzzle.SetTile(puzzle.goal, TileType.Goal);
        puzzle.SetTile(puzzle.playerStart, TileType.PlayerStart);

        // Random blocks
        for (int i = 0; i < numBlocks; i++) {
            Vector2Int pos;
            do {
                pos = new Vector2Int(Random.Range(0, gridSize), Random.Range(0, gridSize));
            } while (!puzzle.InBounds(pos) || puzzle.GetTileAt(pos) != TileType.Empty || pos == puzzle.goal || pos == puzzle.playerStart);

            puzzle.SetTile(pos, TileType.IceBlock);
            puzzle.iceBlocks.Add(pos);
        }

        return puzzle;
    }
}

public class PuzzleSolver {
    public static bool IsSolvable(PuzzleData puzzle) {
        PuzzleState start = new(puzzle.playerStart, puzzle.iceBlocks);
        Queue<PuzzleState> queue = new();
        HashSet<PuzzleState> visited = new();

        queue.Enqueue(start);
        visited.Add(start);

        Vector2Int[] directions = new Vector2Int[] {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        while (queue.Count > 0) {
            PuzzleState current = queue.Dequeue();

            if (current.playerPos == puzzle.goal) return true;

            foreach (var dir in directions) {
                PuzzleState next = SimulateMove(current, dir, puzzle);
                if (!visited.Contains(next)) {
                    queue.Enqueue(next);
                    visited.Add(next);
                }
            }
        }

        return false;
    }

    private static PuzzleState SimulateMove(PuzzleState state, Vector2Int dir, PuzzleData puzzle) {
        PuzzleState newState = state.Copy();
        Vector2Int pos = newState.playerPos;

        while (true) {
            Vector2Int nextPos = pos + dir;
            if (!puzzle.InBounds(nextPos)) break;
            var tile = puzzle.GetTileAt(nextPos);
            if (tile == TileType.Obstacle || tile == TileType.IceBlock) break;
            pos = nextPos;
        }

        newState.playerPos = pos;
        return newState;
    }
}
