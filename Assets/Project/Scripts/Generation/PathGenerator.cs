using System.Collections.Generic;
using UnityEngine;

public static class PathGenerator
{
    public static List<Vector2Int> GeneratePath(int width, int height)
    {
        List<Vector2Int> path = new();
        HashSet<Vector2Int> visited = new();

        Vector2Int current = new(0, Random.Range(0, height));
        Vector2Int end = new(width - 1, Random.Range(0, height));

        path.Add(current);
        visited.Add(current);

        while (current != end)
        {
            List<Vector2Int> dirs = new()
            {
                Vector2Int.right,
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left
            };

            dirs.Shuffle();

            bool moved = false;
            foreach (var dir in dirs)
            {
                Vector2Int next = current + dir;

                if (next.x < 0 || next.x >= width ||
                    next.y < 0 || next.y >= height ||
                    visited.Contains(next))
                    continue;

                visited.Add(next);
                path.Add(next);
                current = next;
                moved = true;
                break;
            }

            if (!moved) break;
        }

        return path;
    }

    public static PipeType GetPipeType(Vector2Int prev, Vector2Int curr, Vector2Int next)
    {
        return (curr - prev) == (next - curr)
            ? PipeType.Straight
            : PipeType.Corner;
    }
}
