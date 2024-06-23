using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameData gameData;
    [Header("Panels")]
    [SerializeField] GameObject startPanel;
    public GameObject cardsPanel;
    public GameObject battlePanel;
    [SerializeField] GameObject endResultPanel;

    [Header("Game Components")]
    public PlayerController playerController;
    [SerializeField] EnemyController enemyController;
    public GameObject damageHolder;

    [Header("Cards")]
    public List<GameObject> enemyCards = new List<GameObject>();
    public List<GameObject> playerCards = new List<GameObject>();

    [HideInInspector] public bool CanPlayGame = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        endResultPanel.SetActive(false);
        startPanel.SetActive(true);
    }

    private void Update()
    {
        
    }

    public void StartGame()
    {
        CanPlayGame = true;
        startPanel.SetActive(false);
        InstantiateCards();
    }

    void InstantiateCards()
    {
        int randomTurn = UnityEngine.Random.Range(0, 2);
        enemyController.SetTurn(randomTurn);
        playerController.SetTurn(randomTurn);

        for(int i=0; i< gameData.enemyCards.Count; i++)
        {
            GameObject enemyCard = Instantiate(enemyCards[i], Vector3.zero, enemyCards[i].transform.rotation, enemyController.transform);
            enemyCard.GetComponent<CharacterInfo>().Initialize(gameData.enemyCards[i]);
        }

        for (int i = 0; i < gameData.playerCards.Count; i++)
        {
            Button playerCard = Instantiate(playerCards[i], Vector3.zero, playerCards[i].transform.rotation, playerController.transform).gameObject.GetComponent<Button>();
            playerCard.GetComponent<CharacterInfo>().Initialize(gameData.playerCards[i]);
            playerCard.GetComponent<CharacterInfo>().playerController = playerController;
            // Adds listener to enable and disable
            playerCard.onClick.AddListener(() =>
                playerController.SelectBtnEnable()
                );
        };
    }

    public void EndGame()
    {
        CanPlayGame = false;
        endResultPanel.SetActive(true);
    }

    public void ApplyDamageToPlayer(float damage)
    {
        playerController.ApplyDamage(damage);
    }

    public void ApplyDamageToEnemy(float damage)
    {
        enemyController.ApplyDamage(damage);
    }
}
