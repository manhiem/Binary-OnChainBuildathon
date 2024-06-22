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
        Debug.Log($"Select Button: {selectCharacterBtn.activeSelf}");
    }

    public void ShowBattleScreen()
    {
        GameManager.Instance.battlePanel.SetActive(onCardSelected);
        GameManager.Instance.cardsPanel.SetActive(!onCardSelected);

        if (!onCardSelected)
        {
            for(int i=0; i< GameManager.Instance.damageHolder.transform.childCount; i++)
            {
                Destroy(GameManager.Instance.damageHolder.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            foreach (var damage in playerDamageList)
            {
                GameObject damageObject = Instantiate(damage, Vector3.zero, damage.transform.rotation, GameManager.Instance.damageHolder.transform);
            }
        }

        onCardSelected = false;
        selectCharacterBtn.SetActive(onCardSelected);
        Debug.Log($"Select Button: {selectCharacterBtn.activeSelf}");
    }

    public void ApplyDamage(float damage)
    {
        currentHP -= damage;
        Debug.Log($"Damage Taken By Player! HP Remaining is: {currentHP}");
    }
}
