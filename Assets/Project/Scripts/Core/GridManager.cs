using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Size")]
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 7;
    [SerializeField] private float cellSize = 1f;

    [Header("Pipe Prefabs")]
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject endPrefab;
    [SerializeField] private GameObject plusPrefab;

    [Header("Straight Prefabs")]
    [SerializeField] private GameObject straightHorizontalPrefab;
    [SerializeField] private GameObject straightVerticalPrefab;

    [Header("Corner Prefabs")]
    [SerializeField] private GameObject cornerUpRight;
    [SerializeField] private GameObject cornerRightDown;
    [SerializeField] private GameObject cornerDownLeft;
    [SerializeField] private GameObject cornerLeftUp;

    private GameObject nodeHolder;
    private Node[] nodes;
    private bool completed;

    // =========================
    public void GenerateGrid(int level)
    {
        completed = false;
        ClearGrid();

        nodeHolder = new GameObject("NodeHolder");

        List<Vector2Int> path = GeneratePath();
        Dictionary<Vector2Int, PipeDirection> map = BuildPipeMap(path);

        nodes = new Node[map.Count];
        int index = 0;

        foreach (var kvp in map)
        {
            Vector2Int pos = kvp.Key;
            PipeDirection dir = kvp.Value;

            bool isStart = pos == path.First();
            bool isEnd = pos == path.Last();

            GameObject prefab = SelectPrefab(dir, isStart, isEnd);
            Vector3 worldPos = GetWorldPosition(pos);

            GameObject go = Instantiate(
                prefab,
                worldPos,
                Quaternion.identity,
                nodeHolder.transform
            );

            Node node = go.GetComponent<Node>();
            node.onNodeCliked += OnNodeClicked;
            nodes[index++] = node;
        }
    }

    // =========================
    private List<Vector2Int> GeneratePath()
    {
        List<Vector2Int> path = new();

        Vector2Int start = new(0, Random.Range(0, height));
        Vector2Int end = new(width - 1, Random.Range(0, height));

        Vector2Int current = start;
        path.Add(current);

        while (current != end)
        {
            List<Vector2Int> options = new();

            if (current.x < end.x) options.Add(Vector2Int.right);
            if (current.y < end.y) options.Add(Vector2Int.up);
            if (current.y > end.y) options.Add(Vector2Int.down);

            current += options[Random.Range(0, options.Count)];
            path.Add(current);
        }

        return path;
    }

    private Dictionary<Vector2Int, PipeDirection> BuildPipeMap(List<Vector2Int> path)
    {
        Dictionary<Vector2Int, PipeDirection> map = new();

        for (int i = 0; i < path.Count; i++)
        {
            PipeDirection dir = PipeDirection.None;

            if (i > 0) dir |= GetDir(path[i], path[i - 1]);
            if (i < path.Count - 1) dir |= GetDir(path[i], path[i + 1]);

            map[path[i]] = dir;
        }

        return map;
    }

    private PipeDirection GetDir(Vector2Int from, Vector2Int to)
    {
        Vector2Int d = to - from;
        if (d == Vector2Int.up) return PipeDirection.Up;
        if (d == Vector2Int.right) return PipeDirection.Right;
        if (d == Vector2Int.down) return PipeDirection.Down;
        return PipeDirection.Left;
    }

    // =========================
    private GameObject SelectPrefab(PipeDirection dir, bool start, bool end)
    {
        if (start) return startPrefab;
        if (end) return endPrefab;

        // PLUS
        if (dir == (PipeDirection.Up | PipeDirection.Right | PipeDirection.Down | PipeDirection.Left))
            return plusPrefab;

        // STRAIGHTS
        if (dir == (PipeDirection.Left | PipeDirection.Right))
            return straightHorizontalPrefab;

        if (dir == (PipeDirection.Up | PipeDirection.Down))
            return straightVerticalPrefab;

        // CORNERS
        if (dir == (PipeDirection.Up | PipeDirection.Right)) return cornerUpRight;
        if (dir == (PipeDirection.Right | PipeDirection.Down)) return cornerRightDown;
        if (dir == (PipeDirection.Down | PipeDirection.Left)) return cornerDownLeft;
        if (dir == (PipeDirection.Left | PipeDirection.Up)) return cornerLeftUp;

        // Fallback (should never happen)
        return plusPrefab;
    }

    // =========================
    private Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        float offsetX = -(width - 1) * cellSize * 0.5f;
        float offsetY = -(height - 1) * cellSize * 0.5f;

        return new Vector3(
            gridPos.x * cellSize + offsetX,
            gridPos.y * cellSize + offsetY,
            0f
        );
    }

    // =========================
    private void OnNodeClicked(object sender, System.EventArgs e)
    {
        if (completed) return;

        if (nodes.All(n => n.ConnectionStatus()))
        {
            completed = true;
            GameManager.Instance.LevelCompleted();
        }
    }

    private void ClearGrid()
    {
        if (nodeHolder != null)
            Destroy(nodeHolder);
    }
}
