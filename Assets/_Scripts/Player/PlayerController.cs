using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float maxHP = 100;
    public float currentHP = 100;

    public bool IsAlive => currentHP > 0;

    [SerializeField] private GameObject selectCharacterBtn;
    [SerializeField] List<GameObject> playerDamageList = new List<GameObject>();

    bool onCardSelected = false;
    bool isTurn = false;
    public CharacterSO selectedCharacterData;

    void Start()
    {
        currentHP = maxHP;
    }

    public void SetTurn(int turnValue)
    {
        isTurn = turnValue == 1 ? true : false;
    }

    void Update()
    {
        if (!IsAlive)
        {
            Debug.Log($"Enemy died! HP Remaining is: {currentHP}");
            GameManager.Instance.EndGame();
        }
    }

    public void SelectBtnEnable()
    {
        onCardSelected = !onCardSelected;
        selectCharacterBtn.SetActive(onCardSelected);
    }

    public void ShowBattleScreen()
    {
        StartCoroutine(CallBattleScreen());
    }

    public IEnumerator CallBattleScreen()
    {
        yield return new WaitUntil(() => GameManager.Instance.enemyState == GameManager.EnemyState.DamageSelect);

        GameManager.Instance.canRollTimer = false;

        if (!onCardSelected)
        {
            for (int i = 0; i < GameManager.Instance.damageHolder.transform.childCount; i++)
            {
                Destroy(GameManager.Instance.damageHolder.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            GameManager.Instance.playerState = GameManager.PlayerState.DamageSelect;
            for (int i = 0; i < playerDamageList.Count; i++)
            {
                GameObject damageObject = Instantiate(playerDamageList[i], Vector3.zero, playerDamageList[i].transform.rotation, GameManager.Instance.damageHolder.transform);
                damageObject.GetComponent<DamageFunction>().Initialize(selectedCharacterData.CharacterAttacks[i]);
            }

            selectedCharacterData = null;
        }

        onCardSelected = false;
        selectCharacterBtn.SetActive(onCardSelected);
    }

    public IEnumerator ResetBattleScreen()
    {
        yield return new WaitUntil(() => GameManager.Instance.enemyState == GameManager.EnemyState.RoundEnded);

        Debug.Log($"ResetBattleScreen()");

        for (int i = 0; i < GameManager.Instance.damageHolder.transform.childCount; i++)
        {
            Destroy(GameManager.Instance.damageHolder.transform.GetChild(i).gameObject);
        }

        onCardSelected = false;
        selectCharacterBtn.SetActive(onCardSelected);
    }

    public void ApplyDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"Damage Taken By Player! HP Remaining is: {currentHP}");
    }
}
