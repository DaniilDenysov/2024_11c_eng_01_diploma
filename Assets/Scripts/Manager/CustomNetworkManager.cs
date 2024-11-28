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
using Score;

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

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);

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
            if (connectionToClient.identity != null)
            {
                var newPlayerInfo = newPlayer.GetComponent<NetworkPlayer>();
                var oldPlayerInfo = connectionToClient.identity.GetComponent<NetworkPlayer>();
                newPlayerInfo.SetNickname(oldPlayerInfo.GetName());
                newPlayerInfo.SetKills(oldPlayerInfo.GetKills());
                newPlayerInfo.SetAssists(oldPlayerInfo.GetAssists());
                newPlayerInfo.SetDeaths(oldPlayerInfo.GetDeaths());
                NetworkServer.ReplacePlayerForConnection(connectionToClient, newPlayer, ReplacePlayerOptions.Destroy);
            }
            else NetworkServer.AddPlayerForConnection(connectionToClient, newPlayer);
        }

    }
}

[System.Serializable]
public class PlayerStats
{
    public string Nickname;
    [SyncVar] public int KillCount;
    [SyncVar] public int AssistCount;
    [SyncVar] public int DeathCount;
}
