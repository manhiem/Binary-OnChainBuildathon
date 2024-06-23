using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameData", order = 1)]
public class GameData : ScriptableObject
{
    public List<CharacterSO> defaultPlayerCards;
    public List<CharacterSO> enemyCards;

    public List<CharacterSO> AllGameCards;
}
