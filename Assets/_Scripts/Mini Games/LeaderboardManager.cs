using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardManager : MonoBehaviour
{
    [SerializeField] private GameObject leaderboardEntryPrefab;
    [SerializeField] private Transform leaderboardContainer;
    [SerializeField] private ScrollRect scrollRect; // Reference to the ScrollRect
    [SerializeField] private Button scrollUpButton; // Reference to the scroll up button
    [SerializeField] private Button scrollDownButton; // Reference to the scroll down button
    [SerializeField] private float scrollSpeed = 0.1f; // Speed of scrolling

    private List<GameObject> leaderboardEntries = new List<GameObject>();

    // Test data: Array of names and times
    private string[] names = { "Player1", "Player2", "Player3", "Player4", "Player5", "Player6", "Player7"};
    private float[] times = { 120.5f, 110.3f, 98.2f, 25.6f, 155.67f, 80f, 111f };

    void Start()
    {
        // Initialize the leaderboard with test data
        UpdateLeaderboard(names, times);

        // Add listeners to buttons
        scrollUpButton.onClick.AddListener(ScrollUp);
        scrollDownButton.onClick.AddListener(ScrollDown);
    }

    public void UpdateLeaderboard(string[] names, float[] times)
    {
        // Clear existing entries
        foreach (GameObject entry in leaderboardEntries)
        {
            Destroy(entry);
        }
        leaderboardEntries.Clear();

        // Create new entries
        for (int i = 0; i < names.Length; i++)
        {
            GameObject entry = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
            TextMeshProUGUI nameText = entry.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI timeText = entry.transform.Find("TimeText").GetComponent<TextMeshProUGUI>();

            nameText.text = names[i];
            timeText.text = FormatTime(times[i]);

            leaderboardEntries.Add(entry);
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        float seconds = time % 60;
        return string.Format("{0:00}:{1:00.00}", minutes, seconds);
    }

    private void ScrollUp()
    {
        StartCoroutine(ScrollCoroutine(Vector2.up));
    }

    private void ScrollDown()
    {
        StartCoroutine(ScrollCoroutine(Vector2.down));
    }

    private IEnumerator<WaitForSeconds> ScrollCoroutine(Vector2 direction)
    {
        while (Input.GetButton("Fire1")) // Assuming "Fire1" is the left mouse button or any other key you set for pressing the button
        {
            scrollRect.verticalNormalizedPosition += direction.y * scrollSpeed * Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
