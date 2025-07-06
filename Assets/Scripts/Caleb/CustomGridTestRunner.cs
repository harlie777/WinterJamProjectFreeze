using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGridTestRunner : MonoBehaviour
{
    public GridManager gridManager;
    public TileDatabase tileDatabase;
    public CustomLevel level;
    public ProceduralWater proceduralWater;
    void Start()
    {
        // Create sample puzzle data
        PuzzleData puzzle = new PuzzleData(size: level.levelSize);
        proceduralWater.GenerateMesh(level.levelSize);


        // Add valid positions
        for (int x = 0; x < puzzle.gridSize; x++)
        {
            for (int y = 0; y < puzzle.gridSize; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                puzzle.validPositions.Add(pos);
                puzzle.SetTile(pos, TileType.Empty);  // default tile
            }
        }

        for (int i = 0;i < level.levelTiles.Count; i++)
        {
            puzzle.SetTile(level.levelTiles[i].tileLocation, level.levelTiles[i].tileType);
            if(level.levelTiles[i].tileType == TileType.PlayerStart)
            {
                puzzle.playerStart = level.levelTiles[i].tileLocation;
            }
            if(level.levelTiles[i].tileType == TileType.Goal)
            {
                puzzle.goal = level.levelTiles[i].tileLocation;
            }
        }


        // Run the spawn method
        gridManager.SpawnGrid(puzzle);
    }
}