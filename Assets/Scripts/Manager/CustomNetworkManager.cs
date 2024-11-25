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

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void SpawnPlayerAt (Vector3 position, NetworkConnectionToClient connectionToClient)
        {
           GameObject newPlayer = Instantiate(playerPrefab, position, Quaternion.identity);
           NetworkServer.ReplacePlayerForConnection(connectionToClient, newPlayer, ReplacePlayerOptions.Destroy);
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
