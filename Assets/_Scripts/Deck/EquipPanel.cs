using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipPanel : MonoBehaviour
{
    public static EquipPanel instance;

    [SerializeField] Image cardImage;

    DeckManager deckManager;
    CharacterSO equippingCharacter;

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

    public void Initialize(Sprite cardSprite, DeckManager deckManager, CharacterSO character)
    {
        cardImage.sprite = cardSprite;
        this.deckManager = deckManager; 
        equippingCharacter = character;
    }

    public void EquipCard()
    {
        deckManager.isEquipping = true;
        deckManager.equippingCardID = equippingCharacter.name;
        this.gameObject.SetActive(false);
    }
}
