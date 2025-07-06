using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileDatabase", menuName = "Puzzle/Tile Database")]
public class TileDatabase : ScriptableObject
{
    [Serializable]
    public class TileEntry
    {
        public TileType type;
        public TileDataSO data;
    }

    [SerializeField]
    private List<TileEntry> tiles = new();

    private Dictionary<TileType, TileDataSO> tileMap;

    public void Init()
    {
        // Initialize the map
        tileMap = new Dictionary<TileType, TileDataSO>();

        // Ensure we have an entry for each TileType
        Array enumValues = Enum.GetValues(typeof(TileType));
        foreach (TileType type in enumValues)
        {
            if (!tiles.Exists(entry => entry.type == type))
            {
                tiles.Add(new TileEntry { type = type, data = null });
            }
        }

        // Populate map from list
        foreach (var tile in tiles)
        {
            if (tileMap.ContainsKey(tile.type))
                Debug.LogWarning($"Duplicate tile type in database: {tile.type}");
            else
                tileMap[tile.type] = tile.data;
        }
    }

    public TileDataSO Get(TileType type)
    {
        if (tileMap == null) Init();

        if (!tileMap.TryGetValue(type, out var data) || data == null)
        {
            Debug.LogWarning($"No TileDataSO found for tile type: {type}");
            return null;
        }

        return data;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Automatically populate the list with all enum types when edited in inspector
        Init();
    }
#endif
}