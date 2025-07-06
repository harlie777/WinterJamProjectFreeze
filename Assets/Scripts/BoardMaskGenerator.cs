using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MaskShapeType {
    Square,
    Rectangle,
    Diamond,
    Ring,
    Plus,
    Cross,
    Circle,
    RandomBlob
}

public static class BoardMaskGenerator
{
    public static List<Vector2Int> Generate(int size, MaskShapeType shape)
    {
        var positions = new List<Vector2Int>();

        switch (shape)
        {
            case MaskShapeType.Square:
                for (int x = 0; x < size; x++)
                    for (int y = 0; y < size; y++)
                        positions.Add(new Vector2Int(x, y));
                break;

            case MaskShapeType.Rectangle:
                int width = size;
                int height = Mathf.RoundToInt(size * 0.6f);
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        positions.Add(new Vector2Int(x, y));
                break;

            case MaskShapeType.Diamond:
                int center = size / 2;
                for (int x = 0; x < size; x++)
                {
                    for (int y = 0; y < size; y++)
                    {
                        if (Mathf.Abs(x - center) + Mathf.Abs(y - center) <= center)
                            positions.Add(new Vector2Int(x, y));
                    }
                }
                break;

            case MaskShapeType.Plus:
                int barWidth = Mathf.Max(1, size / 4);
                int mid = size / 2;
                for (int x = 0; x < size; x++)
                    for (int y = mid - barWidth; y <= mid + barWidth; y++)
                        positions.Add(new Vector2Int(x, y));
                for (int x = mid - barWidth; x <= mid + barWidth; x++)
                    for (int y = 0; y < size; y++)
                        positions.Add(new Vector2Int(x, y));
                break;

            case MaskShapeType.Ring:
                int inner = Mathf.RoundToInt(size * 0.4f);
                int outer = Mathf.RoundToInt(size * 0.5f);
                Vector2 centerVec = new Vector2(size / 2f, size / 2f);
                for (int x = 0; x < size; x++)
                    for (int y = 0; y < size; y++)
                    {
                        float dist = Vector2.Distance(centerVec, new Vector2(x, y));
                        if (dist >= inner && dist <= outer)
                            positions.Add(new Vector2Int(x, y));
                    }
                break;

            case MaskShapeType.RandomBlob:
                // Use flood fill from center with randomness
                HashSet<Vector2Int> visited = new();
                Queue<Vector2Int> queue = new();
                Vector2Int start = new Vector2Int(size / 2, size / 2);
                visited.Add(start);
                queue.Enqueue(start);

                while (visited.Count < size * size * 0.4f && queue.Count > 0)
                {
                    Vector2Int current = queue.Dequeue();
                    foreach (var dir in Directions)
                    {
                        Vector2Int next = current + dir;
                        if (!visited.Contains(next) && Random.value < 0.6f &&
                            next.x >= 0 && next.x < size && next.y >= 0 && next.y < size)
                        {
                            visited.Add(next);
                            queue.Enqueue(next);
                        }
                    }
                }

                positions.AddRange(visited);
                break;
        }

        return positions;
    }

    private static readonly Vector2Int[] Directions = {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };
}
