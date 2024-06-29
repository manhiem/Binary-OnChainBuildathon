using UnityEngine;
using TMPro;
using System.Collections;

public class RacingController : MonoBehaviour
{
    public PlayerInput playerInput;
    public TextMeshProUGUI raceTimerText;

    private bool raceStarted = false;
    private float startTime;
    private Coroutine raceCoroutine;
    [HideInInspector] public bool pressRace = false;
    private Transform startPoint;
    private Transform endPoint;
    private int coinsCollected = 0;
    private bool isMaze = false;

    void Update()
    {
        if (raceStarted)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StopCoroutine(raceCoroutine);
                raceTimerText.gameObject.SetActive(false);
                raceStarted = false;
                coinsCollected = 0; // Reset coin count
            }
            if (!isMaze)
            {
                UpdateRaceTimer();
            }
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
            if (!isMaze)
            {
                float raceTime = Time.time - startTime;
                Debug.Log("Race finished! Time taken: " + raceTime + " seconds");
                raceStarted = false;
                raceTimerText.gameObject.SetActive(false);
            }
        }
        else if (other.gameObject.CompareTag("Coin") && isMaze)
        {
            CollectCoin(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!raceStarted && other.gameObject.layer == 6)
        {
            if (pressRace)
            {
                startPoint = Manager.Instance.raceStartPoint;
                endPoint = Manager.Instance.raceEndPoint;
                TeleportToStart();
                raceCoroutine = StartCoroutine(StartRace());
                pressRace = false;
                isMaze = false; // Ensure it's a race
            }
        }
        else if (!raceStarted && other.gameObject.layer == 7)
        {
            if (pressRace)
            {
                startPoint = Manager.Instance.mazeStartPoint;
                endPoint = Manager.Instance.mazeEndPoint;
                TeleportToStart();
                raceCoroutine = StartCoroutine(StartMaze());
                pressRace = false;
                isMaze = true; // Set to maze mode
            }
        }
    }

    IEnumerator StartMaze()
    {
        raceTimerText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f); // Wait for 1 second before starting the countdown
        raceTimerText.text = "Maze starting in 3...";
        yield return new WaitForSeconds(1f);
        raceTimerText.text = "2...";
        yield return new WaitForSeconds(1f);
        raceTimerText.text = "1...";
        yield return new WaitForSeconds(1f);
        raceStarted = true;
        Debug.Log("Go!");
        startTime = Time.time;
    }

    void CollectCoin(GameObject coin)
    {
        coinsCollected++;
        Destroy(coin);

        if (coinsCollected >= 10)
        {
            float mazeTime = Time.time - startTime;
            Debug.Log("Maze finished! Time taken: " + mazeTime + " seconds");
            raceStarted = false;
            raceTimerText.gameObject.SetActive(false);
            coinsCollected = 0; // Reset coin count
        }
    }
}
