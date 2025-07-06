using UnityEngine;

public class GridTestRunner : MonoBehaviour
{
    public GridManager gridManager;
    public TileDatabase tileDatabase;

    void Start()
    {
        // Create sample puzzle data
        PuzzleData puzzle = new PuzzleData(size: 5);

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

        // Add some custom tiles
        puzzle.SetTile(new Vector2Int(0, 0), TileType.PlayerStart);
        puzzle.SetTile(new Vector2Int(0, 4), TileType.Goal);
        puzzle.SetTile(new Vector2Int(2, 0), TileType.Obstacle);
        puzzle.SetTile(new Vector2Int(2, 3), TileType.IceBlock);
        puzzle.SetTile(new Vector2Int(2, 6), TileType.Empty);
        // puzzle.iceBlocks.Add(new Vector2Int(2, 2));

        puzzle.playerStart = new Vector2Int(0, 0);
        puzzle.goal = new Vector2Int(4, 4);

        // Run the spawn method
        gridManager.SpawnGrid(puzzle);
    }
}