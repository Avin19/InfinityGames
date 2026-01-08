using System;
using UnityEngine;

public class Node : MonoBehaviour
{
  public event EventHandler onNodeCliked;

  [SerializeField] private bool isPlus;
  [SerializeField] private bool isStraight;
  [SerializeField] private bool isStart;
  [SerializeField] private bool isEnd;

  [SerializeField] private Material glow;
  [SerializeField] private Material normal;

  private CameraController camController;

  private SpriteRenderer sr;
  private int rot;
  private bool correct;
  private bool played;

  private void Awake()
  {
    camController = Camera.main.GetComponent<CameraController>();

    sr = GetComponent<SpriteRenderer>();
  }

  private void OnEnable()
  {
    if (!isStart && !isEnd)
      RandomRotate();

    Evaluate();
  }

  private void OnMouseDown()
  {
    if (camController != null && !camController.IsSettled)
      return;

    if (isStart || isEnd)
      return;

    rot = (rot + 1) % 4;
    transform.rotation = Quaternion.Euler(0, 0, rot * 90);

    Evaluate();
    onNodeCliked?.Invoke(this, EventArgs.Empty);
  }


  private void RandomRotate()
  {
    rot = UnityEngine.Random.Range(0, 4);
    transform.rotation = Quaternion.Euler(0, 0, rot * 90);
  }

  private void Evaluate()
  {
    if (isStart || isEnd || isPlus)
      Set(true);
    else if (isStraight)
      Set(rot == 0 || rot == 2);
    else
      Set(rot == 0);
  }

  private void Set(bool v)
  {
    correct = v;
    sr.sharedMaterial = v ? glow : normal;

    if (v && !played)
    {
      AudioManager.Instance.NodeCorrectPosition();
      played = true;
    }
  }

  public bool ConnectionStatus() => correct;
}
