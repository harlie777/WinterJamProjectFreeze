using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Tile", menuName = "Puzzle/Caleb/CustomLevelTile")]
public class CustomLevelTile : ScriptableObject
{
    public Vector2Int tileLocation;
    public TileType tileType;
}
