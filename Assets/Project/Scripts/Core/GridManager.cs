using System;
using System.Linq;
using UnityEngine;

/// <summary>
///  
/// </summary>

public class GridManager : MonoBehaviour
{

    
    [SerializeField] private GameObject[] nodes;
    private GameObject nodeHolder;
    [SerializeField] private bool[] nodeConnectionStatus;
    private bool notcompleted = true;

    private void Start()
    {
        GameManager.Instance.onLevel += OnLevelChange;
    }

    private void OnLevelChange(object sender, EventArgs e)
    {
        gameObject.SetActive(true);
        nodeHolder = GameManager.Instance.GetNodeHolder();
        SetupLevelInfo();
    }
    

    private void SetupLevelInfo()
    {
        nodes = new GameObject[nodeHolder.transform.childCount];
        nodeConnectionStatus = new bool[nodes.Length];
        for (int i = 0; i< nodeHolder.transform.childCount; i++)
        {
            nodes[i] = nodeHolder.transform.GetChild(i).gameObject;
            nodes[i].GetComponent<Node>().onNodeCliked += (sender, args) => CheckStatus(); 
            nodeConnectionStatus[i] = nodes[i].transform.GetComponent<Node>().ConnectionStatus();
            
        }
        
    }

    private void CheckStatus()
    {
        notcompleted = nodeConnectionStatus.Any(element => element == false);
        if (!notcompleted)
        {
            notcompleted = true;
            if (gameObject.activeSelf)
            {
                GameManager.Instance.LevelCompleted();

            }
            gameObject.SetActive(false);
        }
        
    }

    private void Update()
    {
        
        for (int i = 0; i < nodes.Length; i++)
        {
            nodeConnectionStatus[i] = nodes[i].transform.GetComponent<Node>().ConnectionStatus();
            CheckStatus();
        }

    }
}


