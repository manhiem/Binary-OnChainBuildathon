using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour, INetworkRunnerCallbacks
{
    public event Action OnPlayerJoinedSuccessfully;
    public event Action OnPlayerRunnerConnection;
    [SerializeField]
    private NetworkRunner runnerPrefab;
    private NetworkRunner networkRunnerInstance;
    [SerializeField]
    private LoadingScreenManager loadingScreenManager;

    private void Start()
    {
        StartGame(GameMode.AutoHostOrClient, "TestRoom");
    }
    public async void StartGame(GameMode mode, string roomCode)
    {
        loadingScreenManager.ShowLoadingScreen();
        OnPlayerRunnerConnection?.Invoke();
        if (networkRunnerInstance == null)
        {
            networkRunnerInstance = Instantiate(runnerPrefab);
        }

        networkRunnerInstance.AddCallbacks(this);
        networkRunnerInstance.ProvideInput = true;
        var StartGameArgs = new StartGameArgs()
        {
            GameMode = mode,
            SessionName = roomCode,
            PlayerCount = 4,
            SceneManager = networkRunnerInstance.GetComponent<INetworkSceneManager>(),
        };

        var res = await networkRunnerInstance.StartGame(StartGameArgs);
        if (res.Ok)
        {
            const string SCENE_NAME = "LevelDesign";
            await networkRunnerInstance.LoadScene(SCENE_NAME);
            Debug.Log($"Created room with roomCode as {roomCode}");
        }
        else
        {
            Debug.Log($"Could not start the game due to {res.ErrorMessage}");
        }
    }

    public void ShutDown()
    {
        networkRunnerInstance.Shutdown();
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        Debug.Log($"Connection Failed due to {reason}");
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
        Debug.Log($"On Connect Request");
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
        Debug.Log($"On Custom Authentication Response");
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        Debug.Log($"On Host Migration");
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        Debug.Log($"On Input");
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
        Debug.Log($"On Input Missing");
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        OnPlayerJoinedSuccessfully?.Invoke();
        loadingScreenManager.HideLoadingScreen();
        Debug.Log($"{player.PlayerId} has joined!");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"{player.PlayerId} has left!");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {
        Debug.Log($"On Reliable Data Recieved");
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
        Debug.Log($"Done Loading the Scene");
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
        Debug.Log($"Started Loading the Scene");
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
        Debug.Log($"On Session List Updated");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        SceneManager.LoadScene("Auth");
        Debug.Log($"Server has been shutdown use to {shutdownReason}");
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
        Debug.Log($"On User Simulation Message {message}");
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log($"{runner.name} connected to server");
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
        Debug.Log($"{runner.name} disconnected from server");
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }
}