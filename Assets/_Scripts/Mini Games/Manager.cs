using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public static Manager Instance;

    public Transform raceStartPoint;
    public Transform raceEndPoint;

    public Transform mazeStartPoint;
    public Transform mazeEndPoint;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
