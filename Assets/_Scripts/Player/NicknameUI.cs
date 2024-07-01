using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class NicknameUI : MonoBehaviour
{
    public TMP_InputField nicknameInputField;
    public Button submitButton;

    public Action<string> OnSubmit;

    private void Start()
    {
        submitButton.onClick.AddListener(SubmitNickname);
    }

    private void SubmitNickname()
    {
        string nickname = nicknameInputField.text;
        if (!string.IsNullOrEmpty(nickname))
        {
            string id = $"#{UnityEngine.Random.Range(1000, 10000)}";
            PlayerPrefs.SetString("Nickname", nickname);
            PlayerPrefs.SetInt("Car", 0);
            PlayerPrefs.SetInt("Refill", 0);
            Debug.Log($"API: {id} {nickname} {0}");
            gameObject.SetActive(false); // Hide the UI after submission
        }
    }
}
