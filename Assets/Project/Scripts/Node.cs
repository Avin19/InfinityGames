using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Node : MonoBehaviour
{
  private const int MaxRotation = 4;
  private int rotationState;

  public event EventHandler onNodeCliked;

  [Header("Node Type")]
  [SerializeField] private bool isPlusNode;
  [SerializeField] private bool isStraightNode;

  [Header("Special Nodes")]
  [SerializeField] private bool isStartNode;
  [SerializeField] private bool isEndNode;

  [Header("Materials")]
  [SerializeField] private Material glowMaterial;
  [SerializeField] private Material normalMaterial;

  private SpriteRenderer spriteRenderer;
  private bool isCorrect;
  private bool soundPlayed;

  // =========================
  private void Awake()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
  }

  private void OnEnable()
  {
    soundPlayed = false;

    // Start & End should never rotate
    if (!isStartNode && !isEndNode)
    {
      RandomizeRotation();
    }

    Evaluate();
  }

  // =========================
  private void OnMouseDown()
  {
    // Start & End are not interactive
    if (isStartNode || isEndNode)
      return;

    Rotate();
    Evaluate();

    onNodeCliked?.Invoke(this, EventArgs.Empty);
  }

  // =========================
  private void Rotate()
  {
    rotationState = (rotationState + 1) % MaxRotation;
    transform.rotation = Quaternion.Euler(0, 0, rotationState * 90);
  }

  private void RandomizeRotation()
  {
    rotationState = Random.Range(0, MaxRotation);
    transform.rotation = Quaternion.Euler(0, 0, rotationState * 90);
  }

  // =========================
  private void Evaluate()
  {
    // Start & End are always correct
    if (isStartNode || isEndNode)
    {
      SetCorrect(true);
      return;
    }

    // Plus node is always valid
    if (isPlusNode)
    {
      SetCorrect(true);
      return;
    }

    // Straight pipes valid at 0 or 180
    if (isStraightNode)
    {
      SetCorrect(rotationState == 0 || rotationState == 2);
      return;
    }

    // Corner pipes valid only at 0
    SetCorrect(rotationState == 0);
  }

  // =========================
  private void SetCorrect(bool value)
  {
    isCorrect = value;
    spriteRenderer.sharedMaterial = value ? glowMaterial : normalMaterial;

    if (value && !soundPlayed)
    {
      AudioManager.Instance.NodeCorrectPosition();
      soundPlayed = true;
    }
  }

  // =========================
  // CALLED BY GRID MANAGER
  // =========================
  public bool ConnectionStatus()
  {
    // Start & End are always correct
    if (isStartNode || isEndNode)
      return true;

    return isCorrect;
  }
}
