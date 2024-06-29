using UnityEngine;
using UnityEngine.UI;
using Fusion;
using System.Collections;

public class Fuel : NetworkBehaviour
{
    public float maxFuel = 10f;
    public float currentFuel;
    public float depletionRate = 5f; // 10L lasts for 2 minutes = 5L per minute

    public Image fuelBar;
    public Button refuelButton;

    private void Start()
    {
        currentFuel = maxFuel;
        UpdateFuelUI();
    }

    public override void FixedUpdateNetwork()
    {
        // Network synchronization of fuel level
        if (Runner.IsForward)
        {
            DepleteFuel(Runner.DeltaTime * depletionRate / 60f);
        }
    }

    public void DepleteFuel(float amount)
    {
        currentFuel -= amount;
        if (currentFuel <= 0f)
        {
            currentFuel = 0f;
            // Handle out of fuel scenario
        }
        UpdateFuelUI();
    }

    public void RefillFuel(float amount)
    {
        currentFuel += amount;
        if (currentFuel > maxFuel)
        {
            currentFuel = maxFuel;
        }
        UpdateFuelUI();
    }

    private void UpdateFuelUI()
    {
        if (fuelBar != null)
        {
            fuelBar.fillAmount = currentFuel / maxFuel;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Fuel"))
        {
            refuelButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Fuel"))
        {
            refuelButton.gameObject.SetActive(false);
        }
    }

    public void OnRefuelButtonClicked()
    {
        GameObject player = this.gameObject;
        Fuel playerFuel = player.GetComponent<Fuel>();

        if (playerFuel != null)
        {
            float amountToRefill = playerFuel.maxFuel - playerFuel.currentFuel;
            playerFuel.RefillFuel(amountToRefill);
        }
    }
}
