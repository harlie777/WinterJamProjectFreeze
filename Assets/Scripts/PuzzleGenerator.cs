using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PuzzleGenerator : MonoBehaviour
{   
    public PuzzleData Generate(int difficulty)
    {
        int gridSize = Mathf.Min(3 + difficulty / 2, 16);
        int maxBlocks = (gridSize * gridSize) - gridSize;
        int numBlocks = Mathf.Min(2 + difficulty / 3, maxBlocks);

        PuzzleData puzzle = new(gridSize);

        // // Set all tiles to empty and collect valid positions
        // for (int x = 0; x < gridSize; x++)
        // {
        //     for (int y = 0; y < gridSize; y++)
        //     {
        //         var pos = new Vector2Int(x, y);
        //         puzzle.SetTile(pos, TileType.Empty);
        //         puzzle.validPositions.Add(pos);
        //     }
        // }

        int startY = Random.Range(0, gridSize);
        int goalY = Random.Range(0, gridSize);

        puzzle.playerStart = new Vector2Int(0, startY);
        puzzle.goal = new Vector2Int(gridSize - 1, goalY);
        puzzle.SetTile(puzzle.playerStart, TileType.PlayerStart);
        puzzle.SetTile(puzzle.goal, TileType.Goal);

        // Build a list of available positions for obstacles
        List<Vector2Int> candidatePositions = new List<Vector2Int>(puzzle.validPositions);
        candidatePositions.Remove(puzzle.playerStart);
        candidatePositions.Remove(puzzle.goal);

        // Shuffle positions
        for (int i = candidatePositions.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (candidatePositions[i], candidatePositions[j]) = (candidatePositions[j], candidatePositions[i]);
        }

        // Place obstacles
        for (int i = 0; i < Mathf.Min(numBlocks, candidatePositions.Count); i++)
        {
            var pos = candidatePositions[i];
            puzzle.SetTile(pos, TileType.Obstacle);
        }

        return puzzle;
    }
    // public PuzzleData Generate(int difficulty)
    // {
    //     int gridSize = Mathf.Min(3 + difficulty / 2, 20);
    //     int numBlocks = Mathf.Min(2 + difficulty / 3, gridSize * (gridSize - 2));

    //     PuzzleData puzzle = new(gridSize);

    //     for (int x = 0; x < gridSize; x++)
    //     {
    //         for (int y = 0; y < gridSize; y++)
    //         {
    //             var pos = new Vector2Int(x, y);
    //             puzzle.validPositions.Add(pos);
    //             puzzle.SetTile(pos, TileType.Empty);
    //         }
    //     }

    //     puzzle.playerStart = new Vector2Int(0, 0);
    //     puzzle.goal = new Vector2Int(gridSize - 1, gridSize - 1);
    //     puzzle.SetTile(puzzle.goal, TileType.Goal);
    //     puzzle.SetTile(puzzle.playerStart, TileType.PlayerStart);

    //     for (int i = 0; i < numBlocks; i++)
    //     {
    //         Vector2Int pos;
    //         do
    //         {
    //             pos = new Vector2Int(Random.Range(0, gridSize), Random.Range(0, gridSize));
    //         } while (!puzzle.InBounds(pos) || puzzle.GetTileAt(pos) != TileType.Empty || pos == puzzle.goal || pos == puzzle.playerStart);

    //         puzzle.SetTile(pos, TileType.Obstacle);
    //         // puzzle.iceBlocks.Add(pos);
    //     }

    //     return puzzle;
    // }


    public PuzzleData Generate(int difficulty, List<Vector2Int> mask)
    {
        int maxX = int.MinValue;
        int maxY = int.MinValue;
        int minY = int.MaxValue;

        // Manual loop to replace mask.Max(pos => pos.x) and mask.Max/Min(pos => pos.y)
        foreach (var pos in mask)
        {
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y > maxY) maxY = pos.y;
            if (pos.y < minY) minY = pos.y;
        }

        int gridSize = Mathf.Max(maxX, maxY) + 1;
        int numBlocks = Mathf.Min(2 + difficulty / 2, mask.Count / 4);

        PuzzleData puzzle = new(gridSize);

        // Initialize valid positions from mask
        foreach (var pos in mask)
        {
            puzzle.validPositions.Add(pos);
            puzzle.SetTile(pos, TileType.Empty);
        }

        // Start and goal on left/right sides at random Y
        for (int attempt = 0; attempt < 10; attempt++)
        {
            var playerStart = new Vector2Int(0, Random.Range(minY, maxY + 1));
            var goal = new Vector2Int(gridSize - 1, Random.Range(minY, maxY + 1));

            if (puzzle.validPositions.Contains(playerStart) && puzzle.validPositions.Contains(goal))
            {
                puzzle.playerStart = playerStart;
                puzzle.goal = goal;
                puzzle.SetTile(playerStart, TileType.PlayerStart);
                puzzle.SetTile(goal, TileType.Goal);
                break;
            }
        }

        // Place random obstacles
        for (int i = 0; i < numBlocks; i++)
        {
            Vector2Int pos;
            do
            {
                pos = mask[Random.Range(0, mask.Count)];
            } while (
                pos == puzzle.playerStart ||
                pos == puzzle.goal ||
                puzzle.GetTileAt(pos) != TileType.Empty
            );

            puzzle.SetTile(pos, TileType.Obstacle);
        }

        return puzzle;
    }
}


// public class PuzzleCSPGenerator : MonoBehaviour
// {
//     public int maxAttempts = 100;
//     public List<BoardMask> boardMasks; // assign in Inspector

//     public PuzzleData Generate(int difficulty, int requiredDifficultyScore)
//     {
//         int gridSize = Mathf.Min(4 + difficulty / 2, 20);
//         int numObstacles = Mathf.Min(2 + difficulty / 2, gridSize * 2);

//         BoardMask chosenMask = boardMasks[Random.Range(0, boardMasks.Count)];

//         for (int attempt = 0; attempt < maxAttempts; attempt++)
//         {
//             PuzzleData puzzle = new(gridSize);

//             // 1. Apply board mask
//             foreach (var pos in chosenMask.allowedPositions)
//             {
//                 if (pos.x < gridSize && pos.y < gridSize)
//                 {
//                     puzzle.validPositions.Add(pos);
//                     puzzle.SetTile(pos, TileType.Empty);
//                 }
//             }

//             if (puzzle.validPositions.Count < 2) continue;

//             // 2. Pick start & goal at random Y, on left/right edges
//             int startY = Random.Range(0, gridSize);
//             int goalY = Random.Range(0, gridSize);

//             Vector2Int playerStart = new Vector2Int(0, startY);
//             Vector2Int goal = new Vector2Int(gridSize - 1, goalY);

//             if (!puzzle.validPositions.Contains(playerStart) || !puzzle.validPositions.Contains(goal))
//                 continue;

//             puzzle.playerStart = playerStart;
//             puzzle.goal = goal;
//             puzzle.SetTile(playerStart, TileType.PlayerStart);
//             puzzle.SetTile(goal, TileType.Goal);

//             // 3. Place obstacles randomly
//             AddRandomObstacles(puzzle, numObstacles);

//             // 4. Analyze difficulty
//             var solver = FindObjectOfType<PuzzleSolver>();
//             if (solver == null)
//             {
//                 Debug.LogError("Missing PuzzleSolver in scene!");
//                 return null;
//             }

//             solver.AnalyzePuzzle(puzzle);

//             if (puzzle.difficultyScore >= requiredDifficultyScore)
//             {
//                 return puzzle;
//             }
//         }

//         Debug.LogWarning("Puzzle generation failed after max attempts.");
//         return null;
//     }

//     private void AddRandomObstacles(PuzzleData puzzle, int count)
//     {
//         int attempts = 0;
//         while (count > 0 && attempts < 500)
//         {
//             Vector2Int pos = puzzle.validPositions[Random.Range(0, puzzle.validPositions.Count)];
//             if (pos == puzzle.playerStart || pos == puzzle.goal || puzzle.GetTileAt(pos) != TileType.Empty)
//             {
//                 attempts++;
//                 continue;
//             }

//             puzzle.SetTile(pos, TileType.Obstacle);
//             count--;
//         }
//     }
// }