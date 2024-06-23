using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public Image cardImage;
    public CharacterSO cardCharacter;
    DeckManager deckManager;

    public void Initialize(Sprite cardImage, CharacterSO cardName, DeckManager deckManager)
    {
        this.cardImage.sprite = cardImage;
        this.cardCharacter = cardName;
        this.deckManager = deckManager;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(deckManager.isEquipping)
        {
            if(transform.parent == deckManager.selectedCardsContainer && deckManager.equippingCardID != null)
            {
                CardsSaver.instance.selectedCardIds.Add(deckManager.equippingCardID);

                CardsSaver.instance.selectedCardIds.Remove(cardCharacter.name);
                CardsSaver.instance.SaveSelectedCards();

                List<string> selectedCardIds = CardsSaver.instance.LoadSelectedCards();
                deckManager. DisplayCards(selectedCardIds);
            }

            deckManager.isEquipping = false;
            deckManager.equippingCardID = null;
        }
        else
        {
            deckManager.equipPanel.gameObject.SetActive(true);
            deckManager.equipPanel.Initialize(cardImage.sprite, deckManager, cardCharacter);
        }
    }
}
