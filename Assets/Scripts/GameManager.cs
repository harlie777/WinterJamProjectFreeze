
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("References")]
    public PuzzleGenerator generator;
    public PuzzleSolver solver;
    public GridManager gridManager;

    [Header("Level Settings")]
    public int currentLevel = 1;                 // Tracks how many levels the player has completed
    public int currentDifficulty = 1;            // Used to scale the complexity of the puzzles
    // public int currentDifficultyTarget = 5;      // Minimum difficulty score required to accept a puzzle
    // public int difficultyScalingRate = 2;        // How much the difficulty target increases per level
    public int lastNumOfMovesRequired = 1;


    public int pregenCount = 5;                  // Number of puzzles to pre-generate in advance

    [Header("Board Settings")]
    public MaskShapeType shapeType = MaskShapeType.Square;
    public int baseBoardSize = 3;
    public int boardSizeIncreaseRate = 1;

    [Header("Generation Settings")]
    public int generationAttempts = 100000;      // Max attempts before giving up puzzle generation

    private HashSet<long> usedSeeds = new();     // Ensures unique puzzles
    private Queue<PuzzleData> upcomingPuzzles = new(); // Queue of solvable, appropriately difficult puzzles

    [Header("Player Lives")]
    public int lives = 3;

    void Start()
    {
        Instance = this;
        // StartCoroutine(PreGeneratePuzzles(pregenCount));
        // Always maintain a buffer of 1 puzzle ahead
        for(int i = 0; i < 20; i++)
            StartCoroutine(PreGeneratePuzzles(1, currentDifficulty + i));
    }

    /// <summary>
    /// Tries to pre-generate a number of puzzles and store them in a queue.
    /// </summary>
    IEnumerator PreGeneratePuzzles(int count, int difficulty)
    {
        int attempts = 0;

        while (upcomingPuzzles.Count < count && attempts < generationAttempts)
        {
            long seed = Random.Range(int.MinValue, int.MaxValue);
            if (usedSeeds.Contains(seed))
            {
                attempts++;
                continue;
            }

            usedSeeds.Add(seed);
            Random.InitState((int)seed);



            // // Dynamically determine board size based on level progression
            // int boardSize = baseBoardSize + Mathf.Min(currentLevel * boardSizeIncreaseRate, 16);
            // MaskShapeType shape = shapeType; // Just for testing here 
            // // MaskShapeType shape = ChooseShapeType(currentDifficulty);
            // print($"We are going to try and make a {shape} board");
            // List<Vector2Int> mask = BoardMaskGenerator.Generate(boardSize, shapeType);

            // Generate puzzle with current difficulty and mask shape
            // PuzzleData puzzle = generator.Generate(currentDifficulty, mask);
            PuzzleData puzzle = generator.Generate(difficulty);

            // Analyze puzzle and reject if unsolvable
            if (!solver.AnalyzePuzzle(puzzle))
            {
                attempts++;
                print("Puzzle Found Isn't Solvable");
                continue;
            }

            // Accept only if puzzle meets the current difficulty target
            // if (puzzle.difficultyScore >= currentDifficultyTarget - 20)
            // {

            if(lastNumOfMovesRequired <= puzzle.minMovesToSolve + 2)
                upcomingPuzzles.Enqueue(puzzle);
            // }

            attempts++;
            yield return null; // Let Unity yield control back to the main loop
        }

        // Start level immediately if this is the first one
        if (currentLevel == 1 && !gridManager.HasActiveGrid())
        {
            GenerateAndStartLevel();
        }
    }

    // MaskShapeType ChooseShapeType(int difficulty)
    // {
    //     if (difficulty < 5) return MaskShapeType.Square;
    //     if (difficulty < 10) return MaskShapeType.Plus;
    //     if (difficulty < 20) return MaskShapeType.Ring;
    //     return (MaskShapeType)Random.Range(0, System.Enum.GetValues(typeof(MaskShapeType)).Length);
    // }

    /// <summary>
    /// Starts a new level by spawning a puzzle from the pre-generated queue.
    /// </summary>
    void GenerateAndStartLevel()
    {
        if (upcomingPuzzles.Count == 0)
        {
            StartCoroutine(PreGeneratePuzzles(3, currentDifficulty));
            return;
        }

        PuzzleData puzzle = upcomingPuzzles.Dequeue();
        gridManager.SpawnGrid(puzzle);

        // Always maintain a buffer of 1 puzzle ahead
        for(int i = 0; i < 5; i++)
            StartCoroutine(PreGeneratePuzzles(1, currentDifficulty + i));
    }

    /// <summary>
    /// Called when the player solves a puzzle successfully.
    /// Increases difficulty and starts the next level.
    /// </summary>
    public void OnPuzzleSolved()
    {
        
        currentLevel++;
        GetComponentInChildren<LevelNumberUI>().UpdateLevel(currentLevel);
        currentDifficulty++;
        // currentDifficultyTarget += difficultyScalingRate;
        lastNumOfMovesRequired += 1;

        GenerateAndStartLevel();
    }

    public void PlayerDied(CinemachineVirtualCamera cutSceneCamera, Animator anim)
    {
        if(lives > 1)
        {
            lives--;
            GetComponentInChildren<HealthUI>().UpdateLives(lives);
            gridManager.ResetPuzzle();
        }
        else
        {
            lives--;
            GetComponentInChildren<HealthUI>().UpdateLives(lives);
            Cutscene.instance.StartCutscene(cutSceneCamera, anim);
        }
    }
}




// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class GameManager : MonoBehaviour
// {
//     [Header("References")]
//     public PuzzleGenerator generator;
//     public PuzzleSolver solver;
//     public GridManager gridManager;

//     [Header("Level Settings")]
//     public int currentLevel = 1;
//     public int currentDifficulty = 1;
//     public int currentDifficultyTarget = 5;
//     public int difficultyScalingRate = 2;
//     public int pregenCount = 5;

    
//     //How many levels do we try to generate before giving up
//     public int generationAttempts = 100000;

//     private HashSet<long> usedSeeds = new();
//     private Queue<PuzzleData> upcomingPuzzles = new();

//     void Start()
//     {
//         StartCoroutine(PreGeneratePuzzles(pregenCount));
//     }

//     IEnumerator PreGeneratePuzzles(int count)
//     {
//         int attempts = 0;

//         while (upcomingPuzzles.Count < count && attempts < generationAttempts)
//         {
//             long seed = Random.Range(int.MinValue, int.MaxValue);
//             if (usedSeeds.Contains(seed))
//             {
//                 attempts++;
//                 continue;
//             }

//             usedSeeds.Add(seed);
//             Random.InitState((int)seed);

//             PuzzleData puzzle = generator.Generate(currentDifficulty);

//             if (!solver.AnalyzePuzzle(puzzle))
//             {
//                 attempts++;
//                 continue; // Puzzle unsolvable, skip it
//             }

//             if (puzzle.difficultyScore >= currentDifficultyTarget)
//             {
//                 upcomingPuzzles.Enqueue(puzzle);
//             }

//             attempts++;
//             yield return null;
//         }
//         print(attempts);


//         if (currentLevel == 1 && !gridManager.HasActiveGrid())
//         {
//             GenerateAndStartLevel();
//         }
//     }

//     void GenerateAndStartLevel()
//     {
//         if (upcomingPuzzles.Count == 0)
//         {
//             StartCoroutine(PreGeneratePuzzles(3));
//             return;
//         }

//         PuzzleData puzzle = upcomingPuzzles.Dequeue();
//         gridManager.SpawnGrid(puzzle);

//         StartCoroutine(PreGeneratePuzzles(1));
//     }

//     public void OnPuzzleSolved()
//     {
//         currentLevel++;
//         currentDifficulty++;

//         // Increase difficulty gradually
//         currentDifficultyTarget += 2;   
//         GenerateAndStartLevel();
//     }
// }
