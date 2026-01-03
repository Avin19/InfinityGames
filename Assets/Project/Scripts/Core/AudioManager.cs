using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
  public static AudioManager Instance { get; private set; }

  [Header("Audio")]
  [SerializeField] private AudioSource audioSource;
  [SerializeField] private AudioClip buttonClickClip;
  [SerializeField] private AudioClip nodeCorrectClip;
  [SerializeField] private AudioClip levelWinClip;

  [Header("UI")]
  [SerializeField] private Slider volumeSlider;

  private void Awake()
  {
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
      return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);
  }

  private void Start()
  {
    if (audioSource == null)
      audioSource = GetComponent<AudioSource>();

    if (volumeSlider != null)
    {
      volumeSlider.value = audioSource.volume;
      volumeSlider.onValueChanged.AddListener(SetVolume);
    }
  }

  private void SetVolume(float value)
  {
    audioSource.volume = value;
  }

  // ======================
  //  PUBLIC SOUND EVENTS
  // ======================
  public void ButtonClick()
  {
    Play(buttonClickClip);
  }

  public void NodeCorrectPosition()
  {
    Play(nodeCorrectClip);
  }

  public void LevelWin()
  {
    Play(levelWinClip);
  }

  // ======================
  //  INTERNAL PLAY METHOD
  // ======================
  private void Play(AudioClip clip)
  {
    if (clip == null || audioSource == null) return;
    audioSource.PlayOneShot(clip);
  }
}
