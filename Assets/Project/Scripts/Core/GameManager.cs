using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }

  [Header("UI")]
  [SerializeField] private Button startBtn;
  [SerializeField] private Button nextLevelBtn;
  [SerializeField] private Button quitBtn;

  [SerializeField] private GameObject mainMenuPanel;
  [SerializeField] private GameObject gamePanel;
  [SerializeField] private GameObject winPanel;

  [SerializeField] private TextMeshProUGUI levelText;

  [Header("Managers")]
  [SerializeField] private GridManager gridManager;

  private int level;

  private void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;

    startBtn.onClick.AddListener(StartGame);
    nextLevelBtn.onClick.AddListener(NextLevel);
    quitBtn.onClick.AddListener(Application.Quit);
  }

  private void Start()
  {
    level = PlayerPrefs.GetInt("Level", 1);
    UpdateLevelText();

    mainMenuPanel.SetActive(true);
    gamePanel.SetActive(false);
    winPanel.SetActive(false);
  }

  // =========================
  public void StartGame()
  {
    AudioManager.Instance.ButtonClick();

    mainMenuPanel.SetActive(false);
    winPanel.SetActive(false);
    gamePanel.SetActive(true);

    gridManager.GenerateGrid(level);
  }

  // =========================
  public void LevelCompleted()
  {
    AudioManager.Instance.LevelWin();

    gamePanel.SetActive(false);
    winPanel.SetActive(true);
  }

  // =========================
  public void NextLevel()
  {
    AudioManager.Instance.ButtonClick();

    level++;
    PlayerPrefs.SetInt("Level", level);
    UpdateLevelText();

    winPanel.SetActive(false);
    gamePanel.SetActive(true);

    gridManager.GenerateGrid(level);
  }

  private void UpdateLevelText()
  {
    if (levelText != null)
      levelText.text = $"Level {level}";
  }
}
