using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    
    public void MoveCamera(int tileCount, int tileSize)
    {
        float camOffset = (tileCount * tileSize) / 2;
        transform.position = new Vector3(camOffset - 1, camOffset + 4, -2);
    }
}
