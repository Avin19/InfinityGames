using System;
using UnityEngine;
using UnityEngine.UI;


public class AudioManager : MonoBehaviour
{
  public static AudioManager Instance { get; private set; }
  private AudioSource _audio;
  [SerializeField] private AudioClip[] audioClips;
  [SerializeField] private Slider _slider;

  private void Awake()
  {
    if (Instance != null)
    {
      Destroy(this.gameObject);
    }

    Instance = this;
    
  }

  private void Start()
  {
    
    _audio = GetComponent<AudioSource>();
    _slider.value = _audio.volume;
  
  }

 

  private void Update()
  {
    _audio.volume = _slider.value;
  }



  public void ButtonClick()
  {
    _audio.PlayOneShot(audioClips[0]);
  }
  public void NodeCorrectPosition()
  {
    _audio.PlayOneShot(audioClips[1]);
  }
}


