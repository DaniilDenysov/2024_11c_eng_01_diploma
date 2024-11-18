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
        [SerializeField] private List<Player> players;
        [SerializeField, Range(1, 4)] private int minimalLobbySize = 1;
        [SerializeField] private int humanWinScore = 1;
        [SerializeField] private GameObject playerLabelPrefab;
        [SerializeField] private string mainScene;
        [SerializeField] private bool isGameInProgress = false;
        public static Action OnClientConnected, OnClientDisconnected;
        [SerializeField] private List<string> allowedIPs = new List<string>();

        #region Server

        public override void Awake()
        {
            base.Awake();
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.maxConnections = 4;
            NetworkServer.RegisterHandler<LobbyConnection>(OnCreateCharacter);
        }

        private void OnRecievedMessage(NetworkConnectionToClient arg1, ServerMessage arg2)
        {
            MessageLogManager.Instance.DisplayMessage(arg2.Title, arg2.Description);
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            //assign to available
            Debug.Log("Connections:"+ NetworkServer.connections.Count);
            if (isGameInProgress && allowedIPs.Contains(conn.address))
            {
                var networkPlayer = GetPlayerForIP(conn.address);
                //refactor with try functions
                if (networkPlayer != null)
                {
                    var playerData = networkPlayer.GetPlayerData();
                    players[players.IndexOf(players.FirstOrDefault((p)=>p.CharacterGUID== playerData.CharacterGUID))] = playerData;
                    NetworkServer.AddPlayerForConnection(conn, networkPlayer.gameObject);
                   /// Debug.Log($"assigned {networkPlayer.GetCharacterGUID()} for connection {conn.connectionId}");
                    return;
                }
                conn.Disconnect();
            }
  
        }

        public NetworkPlayer GetPlayerForIP(string address)
        {
            var playerData = players.FirstOrDefault((p) => p.IP == address);
            if (playerData != null)
            {
                var networkPlayer = NetworkPlayerContainer.Instance.GetItems().FirstOrDefault((p) => p.GetPlayerData().IP == address);
                return networkPlayer;
            }
            return null;
        }

        public override void OnServerSceneChanged(string sceneName)
        {
            base.OnServerSceneChanged(sceneName);
            if (isGameInProgress)
            {
                AssignOwnersForConnections();
            }
        }

        public override void OnServerChangeScene(string newSceneName)
        {
            base.OnServerChangeScene(newSceneName);
        }


        private void AssignOwnersForConnections ()
        {
            Dictionary<string, Player> characterMappings = new Dictionary<string, Player>();
            foreach (var player in players)
            {
                characterMappings.TryAdd(player.CharacterGUID, player);
            }

       /*     foreach (var networkPlayer in NetworkPlayerContainer.Instance.GetItems())
            {
                if (characterMappings.TryGetValue(networkPlayer.GetCharacterGUID(), out Player playerData))
                {
                    networkPlayer.SetPlayer(playerData);
                    NetworkServer.AddPlayerForConnection(NetworkServer.connections[playerData.ConnectionId], networkPlayer.gameObject);
                }
            }*/
            Debug.Log("Assigned ownership!");
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);
            if (!isGameInProgress)
            {
                allowedIPs.Remove(conn.address);
            }
            else
            {
                var networkPlayer = GetPlayerForIP(conn.address);
                if (networkPlayer != null)
                {
                    NetworkServer.RemovePlayerForConnection(conn);
                }
            }
           /* else
            {
                StopServer();                                                                                                                                               
            }*/
        }


        public ServerMessage ErrorMessage (string title,string description)
        {
            var message = new ServerMessage();
            message.Description = description;
            message.Title = title;
            return message;
        }

        public override void OnStopServer()
        {
            SteamMatchmaking.LeaveLobby(SteamID);
            OnClientDisconnected?.Invoke();
            allowedIPs.Clear();
        }

        public override void OnStopHost()
        {
            SteamMatchmaking.LeaveLobby(SteamID);
            OnClientDisconnected?.Invoke();
            allowedIPs.Clear();
        }

        #endregion

        private List<Player> GetPlayersData ()
        {
            return PlayerLabelsContainer.Instance.GetItems().Select((i)=>i.Player).ToList();
        }

        private void OnCreateCharacter(NetworkConnectionToClient conn,LobbyConnection lobbyConnection)
        {
            if (!isGameInProgress)
            {
                GameObject participant = Instantiate(playerLabelPrefab);
                allowedIPs.Add(conn.address);
                PlayerLabel[] connections = PlayerLabelsContainer.Instance.GetItems().ToArray();
                PlayerLabel label;
                if (participant.gameObject.TryGetComponent(out label))
                {
                    var player = new Player();
                    player.ConnectionId = conn.connectionId;
                    player.IP = conn.address;
                    player.IsPartyOwner = connections.Length == 0;
                    label.Player = player;
                }
                NetworkServer.AddPlayerForConnection(conn, participant);
            }
        }

        #region Client

        public bool CanStartGame ()
        {
            var connections = FindObjectsOfType<PlayerLabel>();
            if (connections.Length >= minimalLobbySize && isGameInProgress == false)
            {
                foreach (var player in connections.Select((h) => h.Player).ToList())
                {
                    if (!player.IsReady)
                    {
                        //LocalPlayerLogContainer.Instance.AddLogMessage("Not all players are ready!");
                        GlobalErrorHandler.Instance.DisplayInfoError("Not all players are ready!");
                        return false;
                    }
                    if (string.IsNullOrEmpty(player.CharacterGUID))
                    {
                        //LocalPlayerLogContainer.Instance.AddLogMessage("Not all players selected their character!");
                        GlobalErrorHandler.Instance.DisplayInfoError("Not all players selected their character!");
                        return false;
                    }
                }
                return true;
            }
            else
            {
                //LocalPlayerLogContainer.Instance.AddLogMessage("Not enough players!");
                GlobalErrorHandler.Instance.DisplayInfoError("Not enough players!");
                return false;
            }
        }

        public void StartGame ()
        {
            isGameInProgress = true;
            players = GetPlayersData();
            ServerChangeScene(mainScene);
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            NetworkClient.Send(new LobbyConnection());
            OnClientConnected?.Invoke();
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            if (NetworkClient.active && !NetworkServer.active)
            {
                GlobalErrorHandler.Instance.DisplayInfoError("Disconnected from server!");
            }
            OnClientDisconnected?.Invoke();
        }

        public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
        {
            if (!LoadingManager.Instance.IsLoading()) LoadingManager.Instance.StartLoading();
            Debug.Log("Client change scene");
            base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);
        }

        public override void OnClientSceneChanged()
        {
            base.OnClientSceneChanged();
            Debug.Log("Client changed scene");
            LoadingManager.Instance.EndLoading();
        }

        public override void OnStopClient()
        {
            SteamMatchmaking.LeaveLobby(SteamID);
            OnClientDisconnected?.Invoke();
        }
        #endregion
    }

    public struct LobbyConnection : NetworkMessage
    {
      
    }

    public struct ServerMessage : NetworkMessage
    {
        public string Title, Description;
    }
}
