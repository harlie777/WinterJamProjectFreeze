using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class PuzzleSolver : MonoBehaviour
// {   
//     private static PuzzleState SimulateMove(PuzzleState state, Vector2Int dir, PuzzleData puzzle) {
//         PuzzleState newState = state.Copy();
//         Vector2Int pos = newState.playerPos;

//         while (true) {
//             Vector2Int nextPos = pos + dir;
//             if (!puzzle.InBounds(nextPos)) break;
//             var tile = puzzle.GetTileAt(nextPos);
//             if (tile == TileType.Obstacle || tile == TileType.IceBlock) break;
//             pos = nextPos;
//         }

//         newState.playerPos = pos;
//         return newState;
//     }


//     public bool IsSolvable(PuzzleData puzzle)
//     {
//         PuzzleState start = new(puzzle.playerStart, puzzle.iceBlocks);
//         Queue<PuzzleState> queue = new();
//         HashSet<PuzzleState> visited = new();

//         queue.Enqueue(start);
//         visited.Add(start);

//         Vector2Int[] directions = new Vector2Int[]
//         {
//             Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
//         };

//         while (queue.Count > 0)
//         {
//             PuzzleState current = queue.Dequeue();

//             if (current.playerPos == puzzle.goal)
//                 return true;

//             foreach (var dir in directions)
//             {
//                 PuzzleState next = SimulateMove(current, dir, puzzle);
//                 if (!visited.Contains(next))
//                 {
//                     queue.Enqueue(next);
//                     visited.Add(next);
//                 }
//             }
//         }

//         return false;
//     }
// }


public class PuzzleSolver : MonoBehaviour
{   
    public int? GetMinMovesToSolveAStar(PuzzleData puzzle)
    {
        PuzzleState start = new PuzzleState(puzzle.playerStart, puzzle.iceBlocks);
        HashSet<PuzzleState> visited = new HashSet<PuzzleState>();
        
        // Priority queue with lowest f(n) first
        var pq = new PriorityQueue<(PuzzleState state, int gCost), int>();

        int Heuristic(Vector2Int pos)
        {
            return Mathf.Abs(puzzle.goal.x - pos.x) + Mathf.Abs(puzzle.goal.y - pos.y);
        }

        pq.Enqueue((start, 0), Heuristic(start.playerPos));

        Vector2Int[] directions = new[] {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        while (pq.Count > 0)
        {
            var (current, gCost) = pq.Dequeue();

            if (current.playerPos == puzzle.goal)
                return gCost;

            if (visited.Contains(current))
                continue;

            visited.Add(current);

            foreach (var dir in directions)
            {
                PuzzleState next = SimulateMove(current, dir, puzzle);
                if (!visited.Contains(next))
                {
                    int h = Heuristic(next.playerPos);
                    int f = gCost + 1 + h; // g(n) + h(n)
                    pq.Enqueue((next, gCost + 1), f);
                }
            }
        }

        return null; // unsolvable
    }
    public int? GetMinMovesToSolve(PuzzleData puzzle)
    {
        PuzzleState start = new(puzzle.playerStart, puzzle.iceBlocks);
        Queue<(PuzzleState state, int depth)> queue = new();
        HashSet<PuzzleState> visited = new();

        queue.Enqueue((start, 0));
        visited.Add(start);

        Vector2Int[] directions = new[] {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        while (queue.Count > 0)
        {
            var (current, depth) = queue.Dequeue();

            if (current.playerPos == puzzle.goal)
                return depth;

            foreach (var dir in directions)
            {
                PuzzleState next = SimulateMove(current, dir, puzzle);
                if (!visited.Contains(next))
                {
                    visited.Add(next);
                    queue.Enqueue((next, depth + 1));
                }
            }
        }

        return null; // unsolvable
    }

    // private PuzzleState SimulateMove(PuzzleState state, Vector2Int direction, PuzzleData puzzle)
    // {
    //     Vector2Int pos = state.playerPos;

    //     while (true)
    //     {
    //         Vector2Int next = pos + direction;
    //         if (!puzzle.InBounds(next)) break;

    //         var tile = puzzle.GetTileAt(next);
    //         if (tile == TileType.Obstacle || tile == TileType.Wall || tile == TileType.IceBlock)
    //             break;

    //         pos = next;

    //         if (tile == TileType.Goal)
    //             break;
    //     }

    //     return new PuzzleState(pos, state.iceBlockPositions);
    // }

    private int CountDeadEnds(PuzzleData puzzle)
    {
        int count = 0;
        
        Vector2Int[] directions = new[] {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
            };

        foreach (var pos in puzzle.validPositions)
            {
                int moves = 0;
                foreach (var dir in directions)
                {
                    Vector2Int next = pos + dir;
                    if (puzzle.InBounds(next) && puzzle.GetTileAt(next) == TileType.Empty)
                        moves++;
                }
                if (moves <= 1)
                    count++;
            }
        return count;
    }

    private float CalculateAverageBranching(PuzzleData puzzle)
    {
        int total = 0;
        int tileCount = 0;

        Vector2Int[] directions = new[] {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
            };

        foreach (var pos in puzzle.validPositions)
        {
            int branches = 0;
            foreach (var dir in directions)
            {
                Vector2Int next = pos + dir;
                if (puzzle.InBounds(next) && puzzle.GetTileAt(next) == TileType.Empty)
                    branches++;
            }

            total += branches;
            tileCount++;
        }

        return tileCount > 0 ? (float)total / tileCount : 0f;
    }





    private static PuzzleState SimulateMove(PuzzleState state, Vector2Int dir, PuzzleData puzzle)
    {
        PuzzleState newState = state.Copy();
        Vector2Int pos = newState.playerPos;

        while (true)
        {
            Vector2Int nextPos = pos + dir;
            if (!puzzle.InBounds(nextPos)) break;
            var tile = puzzle.GetTileAt(nextPos);
            if (tile == TileType.Obstacle || tile == TileType.IceBlock) break;
            pos = nextPos;
        }

        newState.playerPos = pos;
        return newState;
    }
    
    /// <summary>
    /// Returns if True if the puzzle is solvable
    /// </summary>
    /// <param name="puzzle"></param>
    /// <returns></returns>
    public bool AnalyzePuzzle(PuzzleData puzzle)
    {
        print("Anaylsing Puzzle");
        int? minMoves = GetMinMovesToSolve(puzzle); //GetMinMovesToSolveAStar(puzzle); //A* Heurstic sometimes get's it wrong but is slightly faster
        if (!minMoves.HasValue)
            return false;

        puzzle.minMovesToSolve = minMoves.Value;
        // puzzle.numDeadEnds = CountDeadEnds(puzzle);
        // puzzle.avgBranchingFactor = CalculateAverageBranching(puzzle);
        // puzzle.difficultyScore = puzzle.minMovesToSolve + puzzle.numDeadEnds + Mathf.RoundToInt(puzzle.avgBranchingFactor);
        

        return true;
    }
}

public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
{
    private List<(TElement Element, TPriority Priority)> heap = new();

    public int Count => heap.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        heap.Add((element, priority));
        HeapifyUp(heap.Count - 1);
    }

    public TElement Dequeue()
    {
        if (heap.Count == 0) throw new InvalidOperationException("The priority queue is empty.");
        
        TElement result = heap[0].Element;
        heap[0] = heap[^1];
        heap.RemoveAt(heap.Count - 1);
        HeapifyDown(0);
        return result;
    }

    private void HeapifyUp(int index)
    {
        while (index > 0)
        {
            int parent = (index - 1) / 2;
            if (heap[index].Priority.CompareTo(heap[parent].Priority) >= 0)
                break;

            (heap[index], heap[parent]) = (heap[parent], heap[index]);
            index = parent;
        }
    }

    private void HeapifyDown(int index)
    {
        int lastIndex = heap.Count - 1;
        while (true)
        {
            int left = 2 * index + 1;
            int right = 2 * index + 2;
            int smallest = index;

            if (left <= lastIndex && heap[left].Priority.CompareTo(heap[smallest].Priority) < 0)
                smallest = left;

            if (right <= lastIndex && heap[right].Priority.CompareTo(heap[smallest].Priority) < 0)
                smallest = right;

            if (smallest == index)
                break;

            (heap[index], heap[smallest]) = (heap[smallest], heap[index]);
            index = smallest;
        }
    }
}