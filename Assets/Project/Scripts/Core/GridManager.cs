using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
///  Grid Manager
/// 
/// </summary>

public class GridManager : MonoBehaviour
{
  [SerializeField] private Transform nodePrefabs;
  
  [SerializeField] private int gridWidth = 5;
  [SerializeField] private int gridHeight = 5;

  private void Start()
  {
    InitializerGrid();
  }

  private void InitializerGrid()
  {
    for (int x = 0; x < gridWidth; x++)
    {
      for (int y = 0; y < gridHeight; y++)
      {
        Transform nodeObject = Instantiate(nodePrefabs, new Vector3(x, y, 0), Quaternion.identity, transform).transform;
      }
    }
  }
}


