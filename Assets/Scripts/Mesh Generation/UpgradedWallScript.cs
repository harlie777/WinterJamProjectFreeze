using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class UpgradedWallScript : MonoBehaviour
{
    [Header("Grid")]
    public int xCount = 10;
    public int yCount = 10;
    public float tileSize = 1f;

    [Header("Trench Settings")]
    public float trenchDepth = 1f;
    public float edgeWidth = 1f;
    public int cornerSegments = 6;

    [Header("Wavy Ledge")]
    public bool wavyTop = true;
    public float waveStrength = 0.1f;
    public float waveFrequency = 1.5f;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private int v = 0;
    private int t = 0;

    void Start()
    {
        GenerateTrenchWithLedge();
    }

    void GenerateTrenchWithLedge()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        int trenchWalls = (xCount + yCount - 2) * 2; // skip corners
        int curvedCorners = cornerSegments * 4;
        int ledgeWalls = (xCount + yCount) * 2;
        int ledgeCorners = cornerSegments * 4;

        int totalQuads = trenchWalls + curvedCorners + ledgeWalls + ledgeCorners;
        vertices = new Vector3[totalQuads * 4];
        triangles = new int[totalQuads * 6];
        v = 0;
        t = 0;

        float w = xCount * tileSize;
        float h = yCount * tileSize;
        float r = tileSize * 0.5f;

        // === TRENCH WALLS (excluding corners) ===
        for (int x = 1; x < xCount - 1; x++) AddTrenchWall(new Vector3(x * tileSize, 0, 0), new Vector3((x + 1) * tileSize, 0, 0));           // Bottom
        for (int y = 1; y < yCount - 1; y++) AddTrenchWall(new Vector3(w, 0, y * tileSize), new Vector3(w, 0, (y + 1) * tileSize));          // Right
        for (int x = xCount - 1; x > 1; x--) AddTrenchWall(new Vector3(x * tileSize, 0, h), new Vector3((x - 1) * tileSize, 0, h));          // Top
        for (int y = yCount - 1; y > 1; y--) AddTrenchWall(new Vector3(0, 0, y * tileSize), new Vector3(0, 0, (y - 1) * tileSize));          // Left

        // === CURVED INNER TRENCH CORNERS ===
        AddCurvedInnerTrenchCorner(new Vector3(tileSize, 0, tileSize), 180, 270, r);                     // Bottom Left
        AddCurvedInnerTrenchCorner(new Vector3(w - tileSize, 0, tileSize), 270, 360, r);                // Bottom Right
        AddCurvedInnerTrenchCorner(new Vector3(w - tileSize, 0, h - tileSize), 0, 90, r);               // Top Right
        AddCurvedInnerTrenchCorner(new Vector3(tileSize, 0, h - tileSize), 90, 180, r);                 // Top Left

        // === TOP LEDGES (flat) ===
        for (int x = 0; x < xCount; x++) AddTopLedge(new Vector3(x * tileSize, 0, 0), new Vector3((x + 1) * tileSize, 0, 0), Vector3.back);
        for (int y = 0; y < yCount; y++) AddTopLedge(new Vector3(w, 0, y * tileSize), new Vector3(w, 0, (y + 1) * tileSize), Vector3.right);
        for (int x = xCount; x > 0; x--) AddTopLedge(new Vector3(x * tileSize, 0, h), new Vector3((x - 1) * tileSize, 0, h), Vector3.forward);
        for (int y = yCount; y > 0; y--) AddTopLedge(new Vector3(0, 0, y * tileSize), new Vector3(0, 0, (y - 1) * tileSize), Vector3.left);

        // === ROUNDED OUTER LEDGE CORNERS ===
        AddRoundedTopCorner(new Vector3(0, 0, 0), 180, 270);
        AddRoundedTopCorner(new Vector3(w, 0, 0), 270, 360);
        AddRoundedTopCorner(new Vector3(w, 0, h), 0, 90);
        AddRoundedTopCorner(new Vector3(0, 0, h), 90, 180);

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    // === WALL FUNCTIONS ===

    void AddTrenchWall(Vector3 top1, Vector3 top2)
    {
        Vector3 bot1 = top1 + Vector3.down * trenchDepth;
        Vector3 bot2 = top2 + Vector3.down * trenchDepth;
        AddQuad(top1, top2, bot1, bot2, flip: false, wavy: false);
    }

    void AddTopLedge(Vector3 inner1, Vector3 inner2, Vector3 outward)
    {
        Vector3 outer1 = inner1 + outward * edgeWidth;
        Vector3 outer2 = inner2 + outward * edgeWidth;
        AddQuad(inner1, inner2, outer1, outer2, flip: true, wavy: wavyTop);
    }

    void AddCurvedInnerTrenchCorner(Vector3 center, float startAngleDeg, float endAngleDeg, float radius)
    {
        float step = (endAngleDeg - startAngleDeg) / cornerSegments;

        for (int i = 0; i < cornerSegments; i++)
        {
            float a1 = Mathf.Deg2Rad * (startAngleDeg + i * step);
            float a2 = Mathf.Deg2Rad * (startAngleDeg + (i + 1) * step);

            Vector3 top1 = center + new Vector3(Mathf.Cos(a1), 0, Mathf.Sin(a1)) * radius;
            Vector3 top2 = center + new Vector3(Mathf.Cos(a2), 0, Mathf.Sin(a2)) * radius;
            Vector3 bot1 = top1 + Vector3.down * trenchDepth;
            Vector3 bot2 = top2 + Vector3.down * trenchDepth;

            AddQuad(top1, top2, bot1, bot2, flip: false, wavy: false);
        }
    }

    void AddRoundedTopCorner(Vector3 center, float startAngleDeg, float endAngleDeg)
    {
        float step = (endAngleDeg - startAngleDeg) / cornerSegments;

        for (int i = 0; i < cornerSegments; i++)
        {
            float a1 = Mathf.Deg2Rad * (startAngleDeg + i * step);
            float a2 = Mathf.Deg2Rad * (startAngleDeg + (i + 1) * step);

            Vector3 inner = center;
            Vector3 outer1 = inner + new Vector3(Mathf.Cos(a1), 0, Mathf.Sin(a1)) * edgeWidth;
            Vector3 outer2 = inner + new Vector3(Mathf.Cos(a2), 0, Mathf.Sin(a2)) * edgeWidth;

            AddQuad(inner, inner, outer1, outer2, flip: true, wavy: wavyTop);
        }
    }

    void AddQuad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, bool flip, bool wavy)
    {
        if (wavy)
        {
            p1.y += Mathf.Sin((p1.x + p1.z) * waveFrequency) * waveStrength;
            p2.y += Mathf.Sin((p2.x + p2.z) * waveFrequency) * waveStrength;
            p3.y += Mathf.Sin((p3.x + p3.z) * waveFrequency) * waveStrength;
            p4.y += Mathf.Sin((p4.x + p4.z) * waveFrequency) * waveStrength;
        }

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
}