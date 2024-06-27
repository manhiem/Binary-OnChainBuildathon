using UnityEngine;
using TMPro;
using System.Collections;

public class RacingController : MonoBehaviour
{
    public PlayerInput playerInput;
    public Transform startPoint;
    public Transform endPoint;
    public TextMeshProUGUI raceTimerText;

    private bool raceStarted = false;
    private float startTime;
    private Coroutine raceCoroutine;
    [HideInInspector] public bool pressRace = false;

    void Update()
    {
        if (raceStarted)
        {
            if(Input.GetKeyDown(KeyCode.Q))
            {
                StopCoroutine(raceCoroutine);
                raceTimerText.gameObject.SetActive(false);
                raceStarted = false;
            }
            UpdateRaceTimer();
        }

    }

    void TeleportToStart()
    {
        if (startPoint != null)
        {
            Transform playerTransform = this.transform;
            playerTransform.position = startPoint.position;
            playerTransform.rotation = startPoint.rotation;
        }
        else
        {
            Debug.LogError("Start point not set.");
        }
    }

    IEnumerator StartRace()
    {
        raceTimerText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f); // Wait for 1 second before starting the countdown
        raceTimerText.text = "Race starting in 3...";
        yield return new WaitForSeconds(1f);
        raceTimerText.text = "2...";
        yield return new WaitForSeconds(1f);
        raceTimerText.text = "1...";
        yield return new WaitForSeconds(1f);
        raceStarted = true;
        Debug.Log("Go!");
        startTime = Time.time;
    }

    void UpdateRaceTimer()
    {
        float elapsedTime = Time.time - startTime;
        raceTimerText.text = "Time: " + elapsedTime.ToString("F2") + " seconds";
    }

    void OnTriggerEnter(Collider other)
    {

        if (raceStarted && other.transform == endPoint)
        {
            float raceTime = Time.time - startTime;
            Debug.Log("Race finished! Time taken: " + raceTime + " seconds");
            raceStarted = false;
            raceTimerText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!raceStarted && other.gameObject.layer == 6)
        {
            if(pressRace)
            {
                TeleportToStart();
                raceCoroutine = StartCoroutine(StartRace());
                pressRace = false;
            }

        }
    }
}
