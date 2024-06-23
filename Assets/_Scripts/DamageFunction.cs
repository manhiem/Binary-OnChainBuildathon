using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DamageFunction : MonoBehaviour
{
    public bool isPlayer;
    [SerializeField] Image backGroundColor;
    [SerializeField] TextMeshProUGUI attackText;
    [SerializeField] string attackName;

    float damage;

    public void Initialize(AttackSO attackData)
    {
        attackName = attackData.AttackName;
        backGroundColor.color = attackData.color;
        this.damage = attackData.damage;
        attackText.text = attackName;
    }

    public void DamageEnemy()
    {
        if(isPlayer)
        {
            GameManager.Instance.ApplyDamageToEnemy(damage);
            StartCoroutine(GameManager.Instance.playerController.ResetBattleScreen());
        }
        else
        {
            GameManager.Instance.ApplyDamageToPlayer(damage);
        }
    }
}
