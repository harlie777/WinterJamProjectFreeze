using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public PuzzleGenerator generator;
    public PuzzleSolver solver;
    public GridManager gridManager;

    [Header("Level Settings")]
    public int currentLevel = 1;
    public int currentDifficulty = 1;
    public int pregenCount = 5;

    private HashSet<long> usedSeeds = new();  // To avoid repeat puzzles
    private Queue<PuzzleData> upcomingPuzzles = new();

    void Start()
    {
        StartCoroutine(PreGeneratePuzzles(pregenCount));
    }

    IEnumerator PreGeneratePuzzles(int count)
    {
        while (upcomingPuzzles.Count < count)
        {
            long seed = Random.Range(int.MinValue, int.MaxValue);

            if (usedSeeds.Contains(seed))
                continue;

            usedSeeds.Add(seed);
            Random.InitState((int)seed);

            PuzzleData puzzle = generator.Generate(currentDifficulty);

            if (solver == null || solver.IsSolvable(puzzle))
            {
                upcomingPuzzles.Enqueue(puzzle);
            }

            yield return null;
        }

        // Start level if not already started
        if (currentLevel == 1 && gridManager.transform.childCount == 0)
        {
            GenerateAndStartLevel();
        }
    }

    void GenerateAndStartLevel()
    {
        if (upcomingPuzzles.Count == 0)
        {
            StartCoroutine(PreGeneratePuzzles(3));
            return;
        }

        PuzzleData puzzle = upcomingPuzzles.Dequeue();

        gridManager.SpawnGrid(puzzle);

        // Keep generating in background
        StartCoroutine(PreGeneratePuzzles(1));
    }

    public void OnPuzzleSolved()
    {
        currentLevel++;
        currentDifficulty++; // Optional: make this scale more gradually
        GenerateAndStartLevel();
    }
}
