using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInfo : MonoBehaviour
{
    [SerializeField] Image characterImage;
    [SerializeField] Image backGroundColor;
    [SerializeField] string characterName;

    CharacterSO characterData;
    [HideInInspector] public PlayerController playerController;

    public void Initialize(CharacterSO data)
    {
        characterImage.sprite = data.CharacterIcon;
        backGroundColor.color = data.CharacterColor;
        this.characterName = data.name;
        characterData = data;
    }

    public void SpawnDamagePrefabs()
    {
        playerController.selectedCharacterData = characterData;
    }
}
