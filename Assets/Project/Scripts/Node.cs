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


  private SpriteRenderer spriteRenderer;
  private int correctRotation = 0;
  [SerializeField] private Material glowMaterial;
  [SerializeField] private Material normalMaterial;

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
    RotateNode();
  }

  private void RotateNode()
  {
    rotationState = (rotationState + 1) % maxRotationState;
    transform.rotation = Quaternion.Euler(0, 0, rotationState *90);
 
  }

  private void Update()
  {
    
    if (correctRotation == transform.rotation.z)
    {
      spriteRenderer.sharedMaterial = glowMaterial;
    }
    else
    {
      spriteRenderer.sharedMaterial = normalMaterial;
    }
   
  }
}


