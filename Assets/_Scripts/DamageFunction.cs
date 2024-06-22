using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageFunction : MonoBehaviour
{
    public bool isPlayer;

    public void DamageEnemy()
    {
        if(isPlayer)
        {
            GameManager.Instance.ApplyDamageToEnemy(10);
            GameManager.Instance.playerController.ShowBattleScreen();
        }
        else
        {
            GameManager.Instance.ApplyDamageToPlayer(10);
        }
    }
}
