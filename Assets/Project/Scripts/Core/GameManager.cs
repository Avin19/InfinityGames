using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }
  public event EventHandler onLevel;

  [Header("MainMenuButton")] 

  [SerializeField] private Button startBtn;
  [SerializeField] private Button quitBtn;
  [SerializeField] private Button settingBtn;

  [Header(" Panel")] 
  [SerializeField] private GameObject mainMenuPanel;

  [SerializeField] private GameObject levelPanel;
  [SerializeField] private GameObject gamePanel;
  [SerializeField] private GameObject settingPanel;
  [SerializeField] private GameObject winPanel;

  [Header("Level")] 
  [SerializeField] private GameObject[] levels;

  private GameObject nodeHolder;
  [SerializeField] private TextMeshProUGUI levelText;
  [SerializeField] private TextMeshProUGUI levelText2;
 [SerializeField] private int currentLevel =0;

 [Header("LevelButton")] [SerializeField]
 private Button[] levelBtn;
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

  private void Start()
  {
    ResettingLevel();
    SetAllPanelsFalse();
    mainMenuPanel.SetActive(true);
    

    currentLevel = (int) PlayerPrefs.GetFloat("CurrentLevel");
    levelText.text = $" Level : {currentLevel+1}";
    levelText2.text = $" Level : {currentLevel+1}";
  }

  private void SetAllPanelsFalse()
  {
    mainMenuPanel.SetActive(false);
    levelPanel.SetActive(false);
    settingPanel.SetActive(false);
    gamePanel.SetActive(false);
    winPanel.SetActive(false);
  }
  public void StartGame()
  {
    SetAllPanelsFalse();
    AudioManager.Instance.ButtonClick();
    levelPanel.SetActive(true);
    LockLevel();
  }

  private void LockLevel()
  {
    for (int i = 0; i< levelBtn.Length; i++)
    {
      if (i <= currentLevel )
      {
        levelBtn[i].GetComponent<Image>().color = Color.white;
      }
      else
      {
        levelBtn[i].GetComponent<Image>().color = Color.red;
      }
    }
  }
  public void LevelSetting(int level)
  {

    if (level <= currentLevel)
    {
      ResettingLevel();
      AudioManager.Instance.ButtonClick();
      SetAllPanelsFalse();
      gamePanel.SetActive(true);
      
      levels[level].SetActive(true);
      nodeHolder = levels[level].GetComponentInChildren<NodeHolder>().gameObject;
      onLevel?.Invoke(this, EventArgs.Empty);
    }

  }

  public void SettingGame()
  {
    AudioManager.Instance.ButtonClick();
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

  public void BackBtnGame()
  {
    AudioManager.Instance.ButtonClick();
    SetAllPanelsFalse();
    ResettingLevel();
    levelPanel.SetActive(true);
  }

  public void LevelBackBtn()
  {
    AudioManager.Instance.ButtonClick();
    SetAllPanelsFalse();
    mainMenuPanel.SetActive(true);
  }



  public GameObject GetNodeHolder()
  {
    return nodeHolder;
  }

  
  public void LevelCompleted()
  {
    AudioManager.Instance.LevelWin();
    SetAllPanelsFalse();
    winPanel.SetActive(true);
    if (currentLevel < levels.Length-1)
    {
      currentLevel++;
    }
    else
    {
      currentLevel = 0;
    }
    levelText.text = $" Level : {currentLevel+1}";
    levelText2.text = $" Level : {currentLevel+1}";
    PlayerPrefs.SetFloat("CurrentLevel",currentLevel);
    Invoke("BackToLevel",1f);
  }

  private void BackToLevel()
  {
    SetAllPanelsFalse();
    LockLevel();
    BackBtnGame();
  }
 
}


