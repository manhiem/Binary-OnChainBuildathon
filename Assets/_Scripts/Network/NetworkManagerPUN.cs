using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlayerSpawnerPUN : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private string gameplaySceneName;
    [SerializeField] private Vector3 spawnPoint;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == gameplaySceneName)
        {
            // Spawn player in gameplay scene
            SpawnPlayer();
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // Connect to Photon server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Photon Server");

        // Join a random room (or create one if none exists)
        PhotonNetwork.JoinOrCreateRoom("GameRoom", new RoomOptions { MaxPlayers = 4 }, null);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room");

        // Load gameplay scene
        //SceneManager.LoadScene(gameplaySceneName);
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        // Instantiate player prefab
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPoint, Quaternion.identity);

        // Optionally, set player properties or sync data
        // Example: player.GetComponent<PlayerController>().Initialize();

        Debug.Log("Player spawned.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.LogWarning($"Disconnected from Photon Server: {cause}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log($"{newPlayer.NickName} entered the room.");

        // Optionally, notify UI or handle new player
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"{otherPlayer.NickName} left the room.");

        // Optionally, clean up or notify UI
    }
}
