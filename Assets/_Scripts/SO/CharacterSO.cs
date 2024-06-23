using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CharacterData", order = 1)]
public class CharacterSO : ScriptableObject
{
    public Sprite CharacterIcon;
    public Color CharacterColor;

    public List<AttackSO> CharacterAttacks;
}
