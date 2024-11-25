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

        public void SpawnPlayerAt (Vector3 position, NetworkConnectionToClient connectionToClient)
        {
           NetworkServer.ReplacePlayerForConnection(connectionToClient, Instantiate(playerPrefab, position, Quaternion.identity), ReplacePlayerOptions.Destroy);
        }
    }
}
