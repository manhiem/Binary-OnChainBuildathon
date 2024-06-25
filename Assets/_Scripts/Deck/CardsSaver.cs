using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class CardsSaver : MonoBehaviour
{
    public static CardsSaver instance;
    private const string SelectedCardsKey = "SelectedCards";

    public List<string> allCardIds = new List<string>(); // A list containing the IDs of all cards available
    public List<string> selectedCardIds = new List<string>(); // A list containing the IDs of selected cards

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        selectedCardIds = LoadSelectedCards();
        Debug.Log($"Deck Check: selectedCardIds count {selectedCardIds.Count}");

        if (selectedCardIds.Count == 0)
        {
            InitializeDefaultDeck();
        }
        else
        {
            InitializeAllCardIds();
        }
    }

    private void InitializeDefaultDeck()
    {
        foreach (var character in GameManager.Instance.gameData.defaultPlayerCards)
        {
            selectedCardIds.Add(character.name);
        }

        foreach (var card in GameManager.Instance.gameData.AllGameCards)
        {
            allCardIds.Add(card.name);
        }

        SaveSelectedCards();
    }

    private void InitializeAllCardIds()
    {
        foreach (var character in GameManager.Instance.gameData.AllGameCards)
        {
            allCardIds.Add(character.name);
        }
    }

    public List<CharacterSO> GetCardsByNames(List<string> cardNames)
    {
        return GameManager.Instance.gameData.AllGameCards.Where(card => cardNames.Contains(card.name)).ToList();
    }

    public void SaveSelectedCards()
    {
        // Convert the list of selected card IDs to a single string
        string selectedCardsString = string.Join(",", selectedCardIds);
        PlayerPrefs.SetString(SelectedCardsKey, selectedCardsString);
    }

    public List<string> LoadSelectedCards()
    {
        string selectedCardsString = PlayerPrefs.GetString(SelectedCardsKey, "");
        if (string.IsNullOrEmpty(selectedCardsString))
        {
            return new List<string>();
        }

        List<string> selectedCards = new List<string>(selectedCardsString.Split(','));
        return selectedCards;
    }
}
