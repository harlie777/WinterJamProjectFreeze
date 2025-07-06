using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleGenerator : MonoBehaviour
{
    public PuzzleData Generate(int difficulty)
    {
        int gridSize = Mathf.Min(3 + difficulty / 2, 12);
        int numBlocks = Mathf.Min(2 + difficulty / 3, 8);

        PuzzleData puzzle = new(gridSize);

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                var pos = new Vector2Int(x, y);
                puzzle.validPositions.Add(pos);
                puzzle.SetTile(pos, TileType.Empty);
            }
        }

        puzzle.playerStart = new Vector2Int(0, 0);
        puzzle.goal = new Vector2Int(gridSize - 1, gridSize - 1);
        puzzle.SetTile(puzzle.goal, TileType.Goal);
        puzzle.SetTile(puzzle.playerStart, TileType.PlayerStart);

        for (int i = 0; i < numBlocks; i++)
        {
            Vector2Int pos;
            do {
                pos = new Vector2Int(Random.Range(0, gridSize), Random.Range(0, gridSize));
            } while (!puzzle.InBounds(pos) || puzzle.GetTileAt(pos) != TileType.Empty || pos == puzzle.goal || pos == puzzle.playerStart);

            puzzle.SetTile(pos, TileType.Obstacle);
            // puzzle.iceBlocks.Add(pos);
        }

        return puzzle;
    }
}