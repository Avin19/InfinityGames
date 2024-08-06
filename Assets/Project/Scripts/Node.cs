using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
///  
/// </summary>

public class Node : MonoBehaviour
{
  private int rotationState = 0; // 0,1,2,3 represting 0 ,90 ,180 ,270 degree 
  private const int maxRotationState = 4;

  public event EventHandler onNodeCliked;
  private SpriteRenderer spriteRenderer;
  private int correctRotation = 0;
  private bool played;
  [SerializeField] private Material glowMaterial;
  [SerializeField] private Material normalMaterial;
  [SerializeField] private bool isPlusNode;
  [SerializeField] private bool isStraightNode;

  private void OnEnable()
  {
    played = false;
    RandomPipeRotation();
  }

  private void Start()
  {
    spriteRenderer = GetComponent<SpriteRenderer>();
    RandomPipeRotation();
  }

  private void RandomPipeRotation()
  {
 
    rotationState = Random.Range(1,3) % maxRotationState;
    transform.rotation = Quaternion.Euler(0, 0, rotationState *90);
  }

  private void OnMouseDown()
  {
    
    if (isPlusNode)
    {
      spriteRenderer.sharedMaterial = glowMaterial;
      if (!played)
      {
        AudioManager.Instance.NodeCorrectPosition();
        played = true;
      }
    }
    else
    {
      
      RotateNode();
    }
    onNodeCliked?.Invoke(this, EventArgs.Empty);
  }

  private void RotateNode()
  {
    rotationState = (rotationState + 1) % maxRotationState;
    transform.rotation = Quaternion.Euler(0, 0, rotationState *90);
 
  }

  private void Update()
  {
    if (isStraightNode)
    {
       StraightNodeCheck();
    }

    if(!isPlusNode && !isStraightNode)
    {
      TurnNodeCheck();
    }
   
  }

  private void StraightNodeCheck()
  {
    if (Mathf.Approximately(correctRotation, transform.rotation.z)  ||  Mathf.Approximately(180, transform.rotation.z))
    {
      spriteRenderer.sharedMaterial = glowMaterial;

      if (!played)
      {
        AudioManager.Instance.NodeCorrectPosition();
        played = true;
      }
    }
    else
    {
      spriteRenderer.sharedMaterial = normalMaterial;
    }
  }

  private void TurnNodeCheck()
  {
    if (Mathf.Approximately(correctRotation, transform.rotation.z))
    {
      spriteRenderer.sharedMaterial = glowMaterial;

      if (!played)
      {
        AudioManager.Instance.NodeCorrectPosition();
        played = true;
      }
    }
    else
    {
      spriteRenderer.sharedMaterial = normalMaterial;
    }
  }

  public bool ConnectionStatus()
  {
    return played;
  }
}


