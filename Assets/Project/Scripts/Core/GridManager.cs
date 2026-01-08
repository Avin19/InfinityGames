using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Grid Scaling")]
    [SerializeField] private int baseWidth = 5;
    [SerializeField] private int baseHeight = 6;
    [SerializeField] private int levelsPerIncrease = 3;
    [SerializeField] private float cellSize = 1f;

    [Header("Spawn Animation")]
    [SerializeField] private float spawnDelay = 0.02f;
    [SerializeField] private float spawnScaleSpeed = 8f;

    [Header("Prefabs")]
    [SerializeField] private GameObject startPrefab;
    [SerializeField] private GameObject endPrefab;
    [SerializeField] private GameObject plusPrefab;
    [SerializeField] private GameObject straightH;
    [SerializeField] private GameObject straightV;
    [SerializeField] private GameObject cornerUR;
    [SerializeField] private GameObject cornerRD;
    [SerializeField] private GameObject cornerDL;
    [SerializeField] private GameObject cornerLU;

    [Header("Camera")]
    [SerializeField] private CameraController cameraController;

    private List<Node> nodes = new();
    private GameObject holder;
    private Coroutine spawnCoroutine;

    private int width;
    private int height;

    // =========================
    public void GenerateGrid(int level)
    {
        ClearGrid();

        int inc = level / levelsPerIncrease;
        width = baseWidth + inc;
        height = baseHeight + inc;

        // Enable gameplay camera fit
        cameraController.FitGrid(width, height, cellSize);

        holder = new GameObject("Grid");

        List<Vector2Int> path = GeneratePath();
        Dictionary<Vector2Int, PipeDirection> map = BuildMap(path);

        spawnCoroutine = StartCoroutine(SpawnGridAnimated(map, path));
    }

    // =========================
    private IEnumerator SpawnGridAnimated(
        Dictionary<Vector2Int, PipeDirection> map,
        List<Vector2Int> path
    )
    {
        nodes.Clear();

        foreach (var kv in map)
        {
            // 🔒 SAFETY: grid destroyed while coroutine running
            if (holder == null)
                yield break;

            Vector2Int p = kv.Key;
            PipeDirection d = kv.Value;

            GameObject prefab = SelectPrefab(d, p == path[0], p == path[^1]);
            Vector3 pos = GetWorldPos(p);

            GameObject go = Instantiate(prefab, pos, Quaternion.identity, holder.transform);
            go.transform.localScale = Vector3.zero;

            Node node = go.GetComponent<Node>();
            node.onNodeCliked += OnNodeClicked;
            nodes.Add(node);

            float t = 0f;
            while (t < 1f)
            {
                if (go == null || holder == null)
                    yield break;

                t += Time.deltaTime * spawnScaleSpeed;
                go.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
                yield return null;
            }

            yield return new WaitForSeconds(spawnDelay);
        }

        spawnCoroutine = null;
    }

    // =========================
    private void OnNodeClicked(object sender, System.EventArgs e)
    {
        GameManager.Instance.RegisterMove();

        if (nodes.All(n => n.ConnectionStatus()))
            GameManager.Instance.LevelWin();
    }

    // =========================
    private List<Vector2Int> GeneratePath()
    {
        List<Vector2Int> path = new();

        Vector2Int current = new(0, Random.Range(0, height));
        Vector2Int end = new(width - 1, Random.Range(0, height));

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

    private Dictionary<Vector2Int, PipeDirection> BuildMap(List<Vector2Int> path)
    {
        Dictionary<Vector2Int, PipeDirection> map = new();

        for (int i = 0; i < path.Count; i++)
        {
            PipeDirection d = PipeDirection.None;

            if (i > 0) d |= GetDir(path[i], path[i - 1]);
            if (i < path.Count - 1) d |= GetDir(path[i], path[i + 1]);

            map[path[i]] = d;
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
    private GameObject SelectPrefab(PipeDirection d, bool isStart, bool isEnd)
    {
        if (isStart) return startPrefab;
        if (isEnd) return endPrefab;

        if (d == (PipeDirection.Left | PipeDirection.Right)) return straightH;
        if (d == (PipeDirection.Up | PipeDirection.Down)) return straightV;

        if (d == (PipeDirection.Up | PipeDirection.Right)) return cornerUR;
        if (d == (PipeDirection.Right | PipeDirection.Down)) return cornerRD;
        if (d == (PipeDirection.Down | PipeDirection.Left)) return cornerDL;
        if (d == (PipeDirection.Left | PipeDirection.Up)) return cornerLU;

        return plusPrefab;
    }

    private Vector3 GetWorldPos(Vector2Int p)
    {
        float ox = -(width - 1) * cellSize * 0.5f;
        float oy = -(height - 1) * cellSize * 0.5f;

        return new Vector3(
            p.x * cellSize + ox,
            p.y * cellSize + oy,
            0f
        );
    }

    // =========================
    private void ClearGrid()
    {
        // 🔥 STOP animation coroutine FIRST
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        if (holder != null)
        {
            Destroy(holder);
            holder = null;
        }

        nodes.Clear();
    }
}
