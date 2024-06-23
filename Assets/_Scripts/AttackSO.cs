using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AttackData", order = 1)]
public class AttackSO : ScriptableObject
{
    public string AttackName;
    public float damage;
    public Color color;
}
