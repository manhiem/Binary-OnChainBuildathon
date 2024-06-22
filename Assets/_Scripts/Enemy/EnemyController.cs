using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float maxHP = 100;
    public float currentHP = 100;

    public bool IsAlive => currentHP > 0;
    bool isTurn = false;

    public void SetTurn(int turnValue)
    {
        isTurn = turnValue == 0? true : false;
    }

    void Start()
    {
        currentHP = maxHP;
    }

    void Update()
    {
        if (!IsAlive)
        {
            Debug.Log($"Enemy died! HP Remaining is: {currentHP}");
            GameManager.Instance.EndGame();
        }
    }

    public void ApplyDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"Damage Taken By Enemy! HP Remaining is: {currentHP}");
    }
}
