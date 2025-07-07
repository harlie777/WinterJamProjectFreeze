using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WallScript : MonoBehaviour
{
    public int xCount = 10;
    public int yCount = 10;
    public float tileSize = 1f;
    public float trenchDepth = 1f;
    public float edgeWidth = 1f;

    void Start()
    {
        GenerateTrenchWithLedge();
    }

    void GenerateTrenchWithLedge()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int trenchSegments = (xCount * 2 + yCount * 2);
        int ledgeSegments = trenchSegments + 4; // +4 for corners
        int totalSegments = trenchSegments + ledgeSegments;

        Vector3[] vertices = new Vector3[totalSegments * 4];
        int[] triangles = new int[totalSegments * 6];

        int v = 0;
        int t = 0;

        void AddQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, bool flip = false)
        {
            vertices[v + 0] = p1;
            vertices[v + 1] = p2;
            vertices[v + 2] = p3;
            vertices[v + 3] = p4;

            if (!flip)
            {
                triangles[t++] = v + 0;
                triangles[t++] = v + 2;
                triangles[t++] = v + 3;

                triangles[t++] = v + 0;
                triangles[t++] = v + 3;
                triangles[t++] = v + 1;
            }
            else
            {
                triangles[t++] = v + 0;
                triangles[t++] = v + 3;
                triangles[t++] = v + 2;

                triangles[t++] = v + 0;
                triangles[t++] = v + 1;
                triangles[t++] = v + 3;
            }

            v += 4;
        }

        // === TRENCH WALLS ===
        for (int x = 0; x < xCount; x++) // Bottom
        {
            Vector3 top1 = new Vector3(x * tileSize, 0, 0);
            Vector3 top2 = new Vector3((x + 1) * tileSize, 0, 0);
            Vector3 bot1 = top1 + Vector3.down * trenchDepth;
            Vector3 bot2 = top2 + Vector3.down * trenchDepth;
            AddQuad(top1, top2, bot1, bot2);
        }

        for (int y = 0; y < yCount; y++) // Right
        {
            Vector3 top1 = new Vector3(xCount * tileSize, 0, y * tileSize);
            Vector3 top2 = new Vector3(xCount * tileSize, 0, (y + 1) * tileSize);
            Vector3 bot1 = top1 + Vector3.down * trenchDepth;
            Vector3 bot2 = top2 + Vector3.down * trenchDepth;
            AddQuad(top1, top2, bot1, bot2);
        }

        for (int x = xCount; x > 0; x--) // Top
        {
            Vector3 top1 = new Vector3(x * tileSize, 0, yCount * tileSize);
            Vector3 top2 = new Vector3((x - 1) * tileSize, 0, yCount * tileSize);
            Vector3 bot1 = top1 + Vector3.down * trenchDepth;
            Vector3 bot2 = top2 + Vector3.down * trenchDepth;
            AddQuad(top1, top2, bot1, bot2);
        }

        for (int y = yCount; y > 0; y--) // Left
        {
            Vector3 top1 = new Vector3(0, 0, y * tileSize);
            Vector3 top2 = new Vector3(0, 0, (y - 1) * tileSize);
            Vector3 bot1 = top1 + Vector3.down * trenchDepth;
            Vector3 bot2 = top2 + Vector3.down * trenchDepth;
            AddQuad(top1, top2, bot1, bot2);
        }

        // === LEDGE FILL PANELS (with flipped normals) ===
        for (int x = 0; x < xCount; x++) // Bottom
        {
            Vector3 inner1 = new Vector3(x * tileSize, 0, 0);
            Vector3 inner2 = new Vector3((x + 1) * tileSize, 0, 0);
            Vector3 outer1 = inner1 + new Vector3(0, 0, -edgeWidth);
            Vector3 outer2 = inner2 + new Vector3(0, 0, -edgeWidth);
            AddQuad(inner1, inner2, outer1, outer2, flip: true);
        }

        for (int y = 0; y < yCount; y++) // Right
        {
            Vector3 inner1 = new Vector3(xCount * tileSize, 0, y * tileSize);
            Vector3 inner2 = new Vector3(xCount * tileSize, 0, (y + 1) * tileSize);
            Vector3 outer1 = inner1 + new Vector3(edgeWidth, 0, 0);
            Vector3 outer2 = inner2 + new Vector3(edgeWidth, 0, 0);
            AddQuad(inner1, inner2, outer1, outer2, flip: true);
        }

        for (int x = xCount; x > 0; x--) // Top
        {
            Vector3 inner1 = new Vector3(x * tileSize, 0, yCount * tileSize);
            Vector3 inner2 = new Vector3((x - 1) * tileSize, 0, yCount * tileSize);
            Vector3 outer1 = inner1 + new Vector3(0, 0, edgeWidth);
            Vector3 outer2 = inner2 + new Vector3(0, 0, edgeWidth);
            AddQuad(inner1, inner2, outer1, outer2, flip: true);
        }

        for (int y = yCount; y > 0; y--) // Left
        {
            Vector3 inner1 = new Vector3(0, 0, y * tileSize);
            Vector3 inner2 = new Vector3(0, 0, (y - 1) * tileSize);
            Vector3 outer1 = inner1 + new Vector3(-edgeWidth, 0, 0);
            Vector3 outer2 = inner2 + new Vector3(-edgeWidth, 0, 0);
            AddQuad(inner1, inner2, outer1, outer2, flip: true);
        }

        // === CORNER FILL QUADS ===
        Vector3 bl = new Vector3(0, 0, 0);
        Vector3 br = new Vector3(xCount * tileSize, 0, 0);
        Vector3 tr = new Vector3(xCount * tileSize, 0, yCount * tileSize);
        Vector3 tl = new Vector3(0, 0, yCount * tileSize);

        AddQuad(
            bl + new Vector3(-edgeWidth, 0, 0),
            bl,
            bl + new Vector3(-edgeWidth, 0, -edgeWidth),
            bl + new Vector3(0, 0, -edgeWidth),
            flip: true
        );

        AddQuad(
            br,
            br + new Vector3(edgeWidth, 0, 0),
            br + new Vector3(0, 0, -edgeWidth),
            br + new Vector3(edgeWidth, 0, -edgeWidth),
            flip: true
        );

        AddQuad(
            tr + new Vector3(0, 0, edgeWidth),
            tr + new Vector3(edgeWidth, 0, edgeWidth),
            tr,
            tr + new Vector3(edgeWidth, 0, 0),
            flip: true
        );

        AddQuad(
            tl + new Vector3(-edgeWidth, 0, edgeWidth),
            tl + new Vector3(0, 0, edgeWidth),
            tl + new Vector3(-edgeWidth, 0, 0),
            tl,
            flip: true
        );

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}