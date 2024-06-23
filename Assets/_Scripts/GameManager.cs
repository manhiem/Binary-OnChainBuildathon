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

    [Header("Timer")]
    [SerializeField] private TextMeshProUGUI timerText;
    private float roundTime = 10f;
    private bool isRoundActive = false;
    public bool canRollTimer = true;

    [HideInInspector] public bool CanPlayGame = false;

    [SerializeField] public PlayerState playerState;
    [SerializeField] public EnemyState enemyState;

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

    public void StartGame()
    {
        CanPlayGame = true;
        startPanel.SetActive(false);
        InstantiateCards();
        StartRound();
    }

    void InstantiateCards()
    {
        int randomTurn = UnityEngine.Random.Range(0, 2);
        enemyController.SetTurn(randomTurn);
        playerController.SetTurn(randomTurn);

        Instance.playerState = PlayerState.CharacterSelect;
        for (int i = 0; i < gameData.enemyCards.Count; i++)
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
        Instance.enemyState = EnemyState.RoundEnded;
        playerController.ApplyDamage(damage);
    }

    public void ApplyDamageToEnemy(float damage)
    {
        Instance.playerState = PlayerState.RoundEnded;
        enemyController.ApplyDamage(damage);

        if (playerController.IsAlive && enemyController.IsAlive)
        {
            Instance.playerState = PlayerState.CharacterSelect;
            StartRound();
        }
    }

    private void StartRound()
    {
        if (CanPlayGame)
        {
            isRoundActive = true;
            StartCoroutine(RoundTimer());
        }
    }

    private IEnumerator RoundTimer()
    {
        float timeRemaining = roundTime;
        canRollTimer = true;
        StartCoroutine(enemyController.SelectCard());
        while (timeRemaining > 0 && canRollTimer)
        {
            timerText.text = timeRemaining.ToString("F1");
            yield return new WaitForSeconds(0.1f);
            timeRemaining -= 0.1f;
        }

        timerText.text = "0.0";
        isRoundActive = false;

        if (CanPlayGame && canRollTimer)
        {
            SelectRandomCards();
            StartRound();
        }
    }

    private void SelectRandomCards()
    {
        if (gameData.enemyCards.Count > 0 && gameData.playerCards.Count > 0)
        {
            int randomEnemyCardIndex = UnityEngine.Random.Range(0, gameData.enemyCards.Count);
            int randomPlayerCardIndex = UnityEngine.Random.Range(0, gameData.playerCards.Count);

            GameObject enemyCard = enemyCards[randomEnemyCardIndex];
            GameObject playerCard = playerCards[randomPlayerCardIndex];

            // Logic to handle the selected cards
            // Example: Apply damage or any other action you want to take
            Debug.Log($"Selected Enemy Card: {enemyCard.name}, Selected Player Card: {playerCard.name}");
        }
    }

    public enum PlayerState
    {
        CharacterSelect,
        DamageSelect,
        RoundEnded
    }

    public enum EnemyState
    {
        CharacterSelect,
        DamageSelect,
        RoundEnded
    }
}
