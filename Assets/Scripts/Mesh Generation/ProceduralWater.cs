using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class ProceduralWater : MonoBehaviour
{
    [SerializeField]
    private int gridTileSize;
    [SerializeField]
    private CameraMover camMover;
    



    public void GenerateMesh(int gridCount)
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        camMover.MoveCamera(gridCount, gridTileSize);
        int vertCountX = gridCount + 1;
        int vertCountY = gridCount + 1;

        Vector3[] vertices = new Vector3[vertCountX * vertCountY];
        Vector2[] uvs = new Vector2[vertices.Length];
        int[] triangles = new int[gridCount * gridCount * 6];

        // Create vertices and uvs
        for (int y = 0; y < vertCountY; y++)
        {
            for (int x = 0; x < vertCountX; x++)
            {
                int index = y * vertCountX + x;
                vertices[index] = new Vector3(x * gridTileSize, 0, y * gridTileSize);
                uvs[index] = new Vector2((float)x / gridCount, (float)y / gridCount);
            }
        }

        // Create triangles
        int triIndex = 0;
        for (int y = 0; y < gridCount; y++)
        {
            for (int x = 0; x < gridCount; x++)
            {
                int bottomLeft = y * vertCountX + x;
                int bottomRight = bottomLeft + 1;
                int topLeft = bottomLeft + vertCountX;
                int topRight = topLeft + 1;

                // First triangle
                triangles[triIndex++] = bottomLeft;
                triangles[triIndex++] = topLeft;
                triangles[triIndex++] = topRight;

                // Second triangle
                triangles[triIndex++] = bottomLeft;
                triangles[triIndex++] = topRight;
                triangles[triIndex++] = bottomRight;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
