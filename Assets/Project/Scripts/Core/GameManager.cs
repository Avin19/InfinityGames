using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }

  [Header("MainMenuButton")] 

  [SerializeField] private Button startBtn;
  [SerializeField] private Button quitBtn;
  [SerializeField] private Button settingBtn;

  [Header(" Panel")] 
  [SerializeField] private GameObject mainMenuPanel;

  [SerializeField] private GameObject levelPanel;
  [SerializeField] private GameObject gamePanel;
  [SerializeField] private GameObject settingPanel;

  [Header("Level")] [SerializeField] private GameObject[] levels;
  
  private void Awake()
  {
    if (Instance != null)
    {
      Destroy(this.gameObject);
    }

    Instance = this;
    startBtn.onClick.AddListener(() => StartGame());
    settingBtn.onClick.AddListener( () => SettingGame());
    quitBtn.onClick.AddListener( () => Application.Quit());
    
  }

  private void SetAllPanelsFalse()
  {
    mainMenuPanel.SetActive(false);
    levelPanel.SetActive(false);
    settingPanel.SetActive(false);
    gamePanel.SetActive(false);
  }
  public void StartGame()
  {
    SetAllPanelsFalse();
    levelPanel.SetActive(true);
  }

  public void LevelSetting(int level)
  {
    ResettingLevel();
    levelPanel.SetActive(false);
    levels[level].SetActive(true);
  }

  public void SettingGame()
  {
    SetAllPanelsFalse();
    settingPanel.SetActive(true);
  }

  private void ResettingLevel()
  {
    foreach (GameObject l in levels)
    {
      l.SetActive(false);
    }
  }
}


