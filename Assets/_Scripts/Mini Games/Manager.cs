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

    public GameObject garageCanvas;
    public GameObject nickNamePanel;

    public PlayerInput playerInput;

    public CarController[] carModels;

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

    public void ChangeCar(int index)
    {
        int currentCar = PlayerPrefs.GetInt("Car");
        if (currentCar == index) return;

        //    foreach (var c in carModels)
        //{
        //    c.gameObject.SetActive(false);
        //}

        Debug.Log($"API: {index}");
        Destroy(playerInput.gameObject);
        GameObject newCar = Instantiate(carModels[index].gameObject, playerInput.gameObject.transform.position, playerInput.gameObject.transform.rotation);
        playerInput = newCar.GetComponent<PlayerInput>();
    }

    public void InitCar(int index)
    {
        Destroy(playerInput.gameObject);
        GameObject newCar = Instantiate(carModels[index].gameObject, playerInput.gameObject.transform.position, playerInput.gameObject.transform.rotation);
        playerInput = newCar.GetComponent<PlayerInput>();
    }
}
