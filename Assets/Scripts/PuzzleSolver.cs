using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSolver : MonoBehaviour
{   
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


    public bool IsSolvable(PuzzleData puzzle)
    {
        PuzzleState start = new(puzzle.playerStart, puzzle.iceBlocks);
        Queue<PuzzleState> queue = new();
        HashSet<PuzzleState> visited = new();

        queue.Enqueue(start);
        visited.Add(start);

        Vector2Int[] directions = new Vector2Int[]
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        while (queue.Count > 0)
        {
            PuzzleState current = queue.Dequeue();

            if (current.playerPos == puzzle.goal)
                return true;

            foreach (var dir in directions)
            {
                PuzzleState next = SimulateMove(current, dir, puzzle);
                if (!visited.Contains(next))
                {
                    queue.Enqueue(next);
                    visited.Add(next);
                }
            }
        }

        return false;
    }
}
