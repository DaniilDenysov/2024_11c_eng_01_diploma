using DTOs;
using System.Linq;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UI;
using General;
using Steamworks;
using UnityEngine.Events;
using System.Threading.Tasks;

namespace Managers  
{
    public class CustomNetworkManager : NetworkManager
    {
        public CSteamID SteamID { get; set; }



        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log("Connected to server");
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            
        }

        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            base.OnServerReady(conn);
            SpawnPlayerAt(RespawnManager.Instance.GetRandomSpawnPoint().position, conn);
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void SpawnPlayerAt (Vector3 postition,NetworkConnectionToClient connectionToClient)
        {
           GameObject newPlayer = Instantiate(playerPrefab, postition, Quaternion.identity);
            if (connectionToClient.identity != null) NetworkServer.ReplacePlayerForConnection(connectionToClient, newPlayer, ReplacePlayerOptions.Destroy);
            else NetworkServer.AddPlayerForConnection(connectionToClient, newPlayer);
        }

    }
}

[System.Serializable]
public class PlayerStats
{
    public string Nickname;
    public int KillCount;
    public int AssistCount;
    public int DeathCount;
}
