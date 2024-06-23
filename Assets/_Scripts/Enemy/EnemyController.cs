using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float maxHP = 100;
    public float currentHP = 100;

    public bool IsAlive => currentHP > 0;
    bool isTurn = false;

    CharacterSO selectedCharacter;
    AttackSO selectedAttack;

    public void SetTurn(int turnValue)
    {
        isTurn = turnValue == 0? true : false;
    }

    void Start()
    {
        currentHP = maxHP;
        selectedCharacter = null;
    }

    void Update()
    {
        if (!IsAlive)
        {
            Debug.Log($"Enemy died! HP Remaining is: {currentHP}");
            GameManager.Instance.EndGame();
        }
    }

    public IEnumerator SelectCard()
    {
        yield return new WaitUntil(() => GameManager.Instance.playerState == GameManager.PlayerState.CharacterSelect);

        GameManager.Instance.enemyState = GameManager.EnemyState.CharacterSelect;
        int randomSelectWaitTime = UnityEngine.Random.Range(0, 5);
        yield return new WaitForSeconds(randomSelectWaitTime);
        Debug.Log($"Enemy Character Selected");

        selectedCharacter = GameManager.Instance.gameData.enemyCards[UnityEngine.Random.Range(0, 4)];

        GameManager.Instance.enemyState = GameManager.EnemyState.DamageSelect;
        int randomAttackWaitTime = UnityEngine.Random.Range(0, 5);
        yield return new WaitForSeconds(randomAttackWaitTime);

        selectedAttack = selectedCharacter.CharacterAttacks[UnityEngine.Random.Range(0, 4)];
        Debug.Log($"Enemy Attack Selected");
        yield return new WaitUntil(() => GameManager.Instance.playerState == GameManager.PlayerState.DamageSelect);

        GameManager.Instance.ApplyDamageToPlayer(selectedAttack.damage);
        selectedCharacter = null;
        selectedAttack = null;
    }

    public void ApplyDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"Damage Taken By Enemy! HP Remaining is: {currentHP}");
    }
}
