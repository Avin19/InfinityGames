using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private Camera cam;

    [Header("Menu Camera Settings")]
    [SerializeField] private float menuSize = 5f;
    [SerializeField] private Vector3 menuPosition = new Vector3(0, 0, -10);

    [Header("Gameplay Camera Settings")]
    [SerializeField] private float padding = 1.5f;
    [SerializeField] private float smoothSpeed = 6f;

    private float targetSize;
    private Vector3 targetPos;
    private bool gameplayActive;

    public bool IsSettled { get; private set; }

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;

        // Start in MENU state
        ResetToMenu();
    }

    private void LateUpdate()
    {
        if (!gameplayActive)
        {
            // Smoothly return to menu camera
            cam.orthographicSize = Mathf.Lerp(
                cam.orthographicSize,
                menuSize,
                Time.deltaTime * smoothSpeed
            );

            transform.position = Vector3.Lerp(
                transform.position,
                menuPosition,
                Time.deltaTime * smoothSpeed
            );

            return;
        }

        // Gameplay camera auto-fit
        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetSize,
            Time.deltaTime * smoothSpeed
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            Time.deltaTime * smoothSpeed
        );

        IsSettled =
            Mathf.Abs(cam.orthographicSize - targetSize) < 0.01f &&
            Vector3.Distance(transform.position, targetPos) < 0.01f;
    }

    // =========================
    // CALLED WHEN GAME STARTS
    // =========================
    public void FitGrid(int gridWidth, int gridHeight, float cellSize)
    {
        gameplayActive = true;

        float gridWorldWidth = gridWidth * cellSize;
        float gridWorldHeight = gridHeight * cellSize;
        float aspect = (float)Screen.width / Screen.height;

        float sizeByHeight = gridWorldHeight / 2f;
        float sizeByWidth = (gridWorldWidth / aspect) / 2f;

        targetSize = Mathf.Max(sizeByHeight, sizeByWidth) + padding;
        targetPos = new Vector3(0, 0, menuPosition.z);
    }

    // =========================
    // CALLED WHEN EXITING GAME
    // =========================
    public void ResetToMenu()
    {
        gameplayActive = false;
        IsSettled = false;
    }
}
