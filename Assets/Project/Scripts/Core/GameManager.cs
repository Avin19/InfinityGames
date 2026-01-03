using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }

  [Header("Panels")]
  [SerializeField] private GameObject menuPanel;
  [SerializeField] private GameObject gamePanel;
  [SerializeField] private GameObject winPanel;

  [Header("Win Panel UI")]
  [SerializeField] private TextMeshProUGUI resultText;
  [SerializeField] private Button nextBtn;
  [SerializeField] private Button replayBtn;
  [SerializeField] private Button menuBtn;
  [SerializeField] private Button gameplaybtn;   // back from gameplay
  [SerializeField] private Button startBtn;

  [Header("Ad Buttons")]
  [SerializeField] private Button rewardTimeBtn;  // rewarded ad
  [SerializeField] private Button rewardMoveBtn;  // interstitial ad

  [Header("HUD")]
  [SerializeField] private TextMeshProUGUI levelText;
  [SerializeField] private TextMeshProUGUI moveText;
  [SerializeField] private TextMeshProUGUI timerText;

  [Header("Limits")]
  [SerializeField] private int baseMoves = 15;
  [SerializeField] private float baseTime = 30f;
  [SerializeField] private int extraMovesFromAd = 5;
  [SerializeField] private float extraTimeFromAd = 15f;

  [Header("Managers")]
  [SerializeField] private GridManager gridManager;
  [SerializeField] private CameraController cameraController;

  public int CurrentLevel { get; private set; }

  private int remainingMoves;
  private float remainingTime;
  private bool isLevelRunning;

  // =========================
  private void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;

    startBtn.onClick.AddListener(StartGame);
    nextBtn.onClick.AddListener(NextLevel);
    replayBtn.onClick.AddListener(ReplayLevel);
    menuBtn.onClick.AddListener(BackToMenu);
    gameplaybtn.onClick.AddListener(BackToMenu);

    if (rewardTimeBtn != null)
      rewardTimeBtn.onClick.AddListener(WatchAdForExtraTime);

    if (rewardMoveBtn != null)
      rewardMoveBtn.onClick.AddListener(WatchAdForExtraMoves);
  }

  private void Start()
  {
    CurrentLevel = PlayerPrefs.GetInt("Level", 1);
    ShowMenu();
  }

  private void Update()
  {
    if (!isLevelRunning) return;

    remainingTime -= Time.deltaTime;
    timerText.text = $"Time: {Mathf.CeilToInt(remainingTime)}s";

    if (remainingTime <= 0)
    {
      remainingTime = 0;
      LevelLose("TIME UP!");
    }
  }

  // =========================
  private void ShowMenu()
  {
    isLevelRunning = false;
    cameraController.ResetToMenu();

    menuPanel.SetActive(true);
    gamePanel.SetActive(false);
    winPanel.SetActive(false);
  }


  public void StartGame()
  {
    AudioManager.Instance.ButtonClick();
    StartLevel();
  }

  private void StartLevel()
  {
    // scale difficulty slightly with level
    remainingMoves = baseMoves + (CurrentLevel / 3);
    remainingTime = baseTime + (CurrentLevel * 2);

    isLevelRunning = true;

    moveText.text = $"Moves: {remainingMoves}";
    timerText.text = $"Time: {Mathf.CeilToInt(remainingTime)}s";
    levelText.text = $"Level {CurrentLevel}";

    menuPanel.SetActive(false);
    winPanel.SetActive(false);
    gamePanel.SetActive(true);

    gridManager.GenerateGrid(CurrentLevel);
  }

  // =========================
  public void RegisterMove()
  {
    if (!isLevelRunning) return;

    remainingMoves--;
    moveText.text = $"Moves: {remainingMoves}";

    if (remainingMoves <= 0)
    {
      remainingMoves = 0;
      LevelLose("OUT OF MOVES!");
    }
  }

  // =========================
  public void LevelWin()
  {
    if (!isLevelRunning) return;

    isLevelRunning = false;

    AudioManager.Instance.LevelWin();
    PlayerPrefs.SetInt("Level", ++CurrentLevel);

    resultText.text = "LEVEL COMPLETE!";
    nextBtn.gameObject.SetActive(true);

    ShowWinPanel();
  }

  private void LevelLose(string message)
  {
    if (!isLevelRunning) return;

    isLevelRunning = false;

    resultText.text = message;
    nextBtn.gameObject.SetActive(false);

    ShowWinPanel();
  }

  private void ShowWinPanel()
  {
    cameraController.ResetToMenu();

    gamePanel.SetActive(false);
    winPanel.SetActive(true);

    // ad buttons visible on loss
    if (rewardTimeBtn != null)
      rewardTimeBtn.gameObject.SetActive(true);

    if (rewardMoveBtn != null)
      rewardMoveBtn.gameObject.SetActive(true);
  }

  // =========================
  private void NextLevel()
  {
    StartLevel();
  }

  private void ReplayLevel()
  {
    StartLevel();
  }

  private void BackToMenu()
  {
    ShowMenu();
  }

  // =========================
  // ADS
  // =========================
  public void WatchAdForExtraTime()
  {
    AdMobManager.Instance.ShowRewarded(() =>
    {
      remainingTime += extraTimeFromAd;
      ResumeAfterAd();
    });
  }

  public void WatchAdForExtraMoves()
  {
    AdMobManager.Instance.ShowInterstitial();

    remainingMoves += extraMovesFromAd;
    moveText.text = $"Moves: {remainingMoves}";

    ResumeAfterAd();
  }

  private void ResumeAfterAd()
  {
    isLevelRunning = true;
    winPanel.SetActive(false);
    gamePanel.SetActive(true);
  }
}
