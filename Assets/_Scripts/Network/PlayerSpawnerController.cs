using System;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerSpawnerController : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private Transform spawnPoints;

    bool first = false;

    public override void Spawned()
    {
        if (Runner.IsServer)
        {
            foreach (var item in Runner.ActivePlayers)
            {
                Debug.LogError("Item!");
                SpawnPlayer(item);
            }
        }
    }

    private void SpawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            var spawnPoint = spawnPoints.transform.position;
            var playerObject = Runner.Spawn(playerNetworkPrefab, spawnPoint, Quaternion.identity, playerRef);

            Runner.SetPlayerObject(playerRef, playerObject);
        }
    }

    private void DespawnPlayer(PlayerRef playerRef)
    {
        if (Runner.IsServer)
        {
            if (Runner.TryGetPlayerObject(playerRef, out var playerNetworkObject))
            {
                Runner.Despawn(playerNetworkObject);
            }

            //Reset player object
            Runner.SetPlayerObject(playerRef, null);
        }
    }

    public void PlayerJoined(PlayerRef player)
    {
        //if (!first)
        //{
        //    first = true;
        //    return;
        //}

        Debug.LogError("Joined!");
        SpawnPlayer(player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        DespawnPlayer(player);
    }
}