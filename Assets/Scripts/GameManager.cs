// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class GameManager : MonoBehaviour
// {
//     public PuzzleGenerator generator;
//     public PuzzleSolver solver;
//     public GridManager gridManager;

//     int currentLevel = 1;
//     int currentDifficulty = 1;
//     HashSet<long> usedSeeds = new();  // Fast lookup

//     Queue<PuzzleData> upcomingPuzzles = new();

//     void Start()
//     {
//         StartCoroutine(PreGeneratePuzzles(5)); // Fill the queue
//     }

//     // Generates puzzle in advanced to avoid loading time
//     IEnumerator PreGeneratePuzzles(int count)
//     {
//         while (upcomingPuzzles.Count < count)
//         {
//             long seed = Random.Range(int.MinValue, int.MaxValue);

//             // If we have already generated this level skip it
//             if (usedSeeds.Contains(seed))
//                 continue;

//             usedSeeds.Add(seed);
//             Random.InitState((int)seed);

//             PuzzleData puzzle = generator.Generate(currentDifficulty);
//             if (solver.IsSolvable(puzzle))
//             {
//                 upcomingPuzzles.Enqueue(puzzle);
//             }

//             yield return null; // Let Unity breathe
//         }

//         GenerateAndStartLevel(); // Start first level after prefill
//     }

//     void GenerateAndStartLevel()
//     {
//         if (upcomingPuzzles.Count == 0)
//         {
//             StartCoroutine(PreGeneratePuzzles(3));
//             return; // Wait for more puzzles
//         }

//         PuzzleData puzzle = upcomingPuzzles.Dequeue();
//         gridManager.SpawnGrid(puzzle);

//         // Optionally: begin pre-generating the next batch
//         StartCoroutine(PreGeneratePuzzles(1));
//     }

//     public void OnPuzzleSolved()
//     {
//         currentLevel++;
//         currentDifficulty++;
//         GenerateAndStartLevel();
//     }
// }
