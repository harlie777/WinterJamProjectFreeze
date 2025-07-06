using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("References")]
    public Transform gridParent;
    public TileDatabase tileDatabase;
    [SerializeField]
    private GameObject playerPrefab;

    [Header("Grid Settings")]
    public float tileSpacing = 1f;
    public bool showCoordinates = true;
    public Font labelFont;

    [Header("Gizmo Settings")]
    public Color gizmoColor = new Color(0.3f, 0.7f, 1f, 0.2f);
    public int gridGizmoSize = 4;
    [Header("Land Settings")]
    [SerializeField] private GameObject landTilePrefab;
    [SerializeField] private int landBorderSize = 2; // how far around the puzzle to fill

    private PuzzleData currentPuzzle;
    private GameObject playerInstance;

    
    private void SpawnSurroundingLand(PuzzleData puzzle)
    {
        int minX = -landBorderSize;
        int maxX = puzzle.gridSize + landBorderSize;
        int minY = -landBorderSize;
        int maxY = puzzle.gridSize + landBorderSize;

        for (int x = minX; x < maxX; x++)
        {
            for (int y = minY; y < maxY; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (!puzzle.validPositions.Contains(pos))
                {
                    Vector3 worldPos = GridToWorld(pos);
                    Instantiate(landTilePrefab, worldPos, Quaternion.identity, gridParent);
                }
            }
        }
    }
   
    public void SpawnGrid(PuzzleData puzzle)
    {
        ClearGrid();
        currentPuzzle = puzzle;
        gridGizmoSize = currentPuzzle.gridSize;

        SpawnSurroundingLand(puzzle);

        // Spawn tiles
        foreach (var kvp in puzzle.tiles)
        {
            Vector2Int pos = kvp.Key;
            TileType type = kvp.Value;

            TileDataSO tileData = tileDatabase.Get(type);
            if (tileData == null) continue;

            Vector3 worldPos = GridToWorld(pos);
            GameObject obj = Instantiate(tileData.prefab, worldPos, Quaternion.identity, gridParent);

            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
                renderer.material.color = tileData.color;

            if (showCoordinates)
                CreateLabel(worldPos, pos);
        }

        // Spawn special markers
        SpawnMarker(puzzle.playerStart, TileType.PlayerStart);
        SpawnMarker(puzzle.goal, TileType.Goal);

        // Optional: spawn ice blocks again visually
        // foreach (Vector2Int icePos in puzzle.iceBlocks)
        //     SpawnMarker(icePos, TileType.IceBlock);

        // Spawn player
        if (playerInstance != null)
            Destroy(playerInstance);

        Vector3 startPos = GridToWorld(puzzle.playerStart);
        playerInstance = Instantiate(playerPrefab, startPos, Quaternion.identity);

        var controller = playerInstance.GetComponent<PlayerController>();
        if (controller != null)
        {
            controller.Init(puzzle, puzzle.playerStart, this);
        }
    }

    // PUBLIC: Called by a reset button
    public void ResetPuzzle()
    {
        if (currentPuzzle != null)
        {
            SpawnGrid(currentPuzzle);
        }
    }

    private void SpawnMarker(Vector2Int pos, TileType type)
    {
        TileDataSO tileData = tileDatabase.Get(type);
        if (tileData == null) return;

        Vector3 worldPos = GridToWorld(pos);
        GameObject obj = Instantiate(tileData.prefab, worldPos, Quaternion.identity, gridParent);

        var renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = tileData.color;

        if (showCoordinates)
            CreateLabel(worldPos, pos);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * tileSpacing, 0f, gridPos.y * tileSpacing);
    }

    private void CreateLabel(Vector3 worldPos, Vector2Int coords)
    {
        GameObject label = new GameObject($"Label_{coords.x}_{coords.y}");
        label.transform.position = worldPos + new Vector3(0, 0.5f, 0);
        label.transform.SetParent(gridParent);

        TextMesh textMesh = label.AddComponent<TextMesh>();
        textMesh.text = $"({coords.x},{coords.y})";
        textMesh.characterSize = 0.25f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.fontSize = 24;
        if (labelFont != null) textMesh.font = labelFont;
    }

    private void ClearGrid()
    {
        foreach (Transform child in gridParent)
            Destroy(child.gameObject);

        if (playerInstance != null)
        {
            Destroy(playerInstance);
            playerInstance = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || gridParent == null) return;

        Gizmos.color = gizmoColor;

        for (int x = 0; x < gridGizmoSize; x++)
        {
            for (int z = 0; z < gridGizmoSize; z++)
            {
                Vector3 center = new Vector3(x * tileSpacing, 0, z * tileSpacing);
                Gizmos.DrawCube(center, new Vector3(tileSpacing * 0.95f, 0.05f, tileSpacing * 0.95f));
            }
        }
    }
}
