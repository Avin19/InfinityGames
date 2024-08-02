using System;
using UnityEngine;

/// <summary>
///  
/// </summary>

public class Node : MonoBehaviour
{
  private int rotationState = 0; // 0,1,2,3 represting 0 ,90 ,180 ,270 degree 
  private const int maxRotationState = 4;

  [SerializeField] private bool[] connectons = new bool[4]; // Top , Right, Bottom , Left

 
  private void OnMouseDown()
  {
    RotateNode();
  }

  private void RotateNode()
  {
    rotationState = (rotationState + 1) % maxRotationState;
    transform.rotation = Quaternion.Euler(0, 0, rotationState *90);
    
  }
}


