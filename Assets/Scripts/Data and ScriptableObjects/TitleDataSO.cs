using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileData", menuName = "Puzzle/Tile Data")]
public class TileDataSO : ScriptableObject {
    public TileType type;
    public GameObject[] prefabs;
    public Color color;
    public int movementCost;
    // // Add more custom properties if needed
}