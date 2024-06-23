using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    public Transform selectedCardsContainer;
    public Transform otherCardsContainer;
    public GameObject cardPrefab;
    public EquipPanel equipPanel;

    [HideInInspector] public bool isEquipping = false;
    [HideInInspector] public string equippingCardID = string.Empty;

    private void Start()
    {
        List<string> selectedCardIds = CardsSaver.instance.LoadSelectedCards();
        DisplayCards(selectedCardIds);
    }

    public void DisplayCards(List<string> selectedCardIds)
    {
        foreach (Transform child in selectedCardsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in otherCardsContainer)
        {
            Destroy(child.gameObject);
        }

        List<string> otherCardIds = new List<string>(CardsSaver.instance.allCardIds);
        foreach (string cardId in selectedCardIds)
        {
            GameObject card = Instantiate(cardPrefab, selectedCardsContainer);
            CharacterSO character = GameManager.Instance.gameData.AllGameCards.FirstOrDefault(card => card.name == cardId);

            card.GetComponent<Card>().Initialize(character.CharacterIcon, character,this);
            otherCardIds.Remove(cardId);
        }

        foreach (string cardId in otherCardIds)
        {
            GameObject card = Instantiate(cardPrefab, otherCardsContainer);
            CharacterSO character = GameManager.Instance.gameData.AllGameCards.FirstOrDefault(card => card.name == cardId);

            card.GetComponent<Card>().Initialize(character.CharacterIcon, character, this);
        }
    }

    private void OnEnable()
    {
        List<string> selectedCardIds = CardsSaver.instance.LoadSelectedCards();
        DisplayCards(selectedCardIds);
    }

    private void OnDisable()
    {
        foreach (Transform child in selectedCardsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in otherCardsContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
