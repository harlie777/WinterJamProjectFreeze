using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Tile", menuName = "Puzzle/Caleb/CustomLevel")]
public class CustomLevel : ScriptableObject
{
    public int levelSize;
    public List<CustomLevelTile> levelTiles;
}
