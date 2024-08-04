using System;
using UnityEngine;



public class AudioManager : MonoBehaviour
{
  public static AudioManager Instance { get; private set; }
  private AudioSource _audio;
  [SerializeField] private AudioClip[] audioClips;

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


