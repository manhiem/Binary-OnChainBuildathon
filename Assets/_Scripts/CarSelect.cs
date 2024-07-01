using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSelect : MonoBehaviour
{
    public int carID;
    public void OnSelectedCar()
    {
        Manager.Instance.ChangeCar(carID);
        PlayerPrefs.SetInt("Car", carID);

        Manager.Instance.garageCanvas.SetActive(false);
    }
}
