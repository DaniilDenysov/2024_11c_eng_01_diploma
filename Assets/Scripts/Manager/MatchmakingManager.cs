using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using Steamworks;
using DesignPatterns.Singleton;

namespace Managers
{
    public class MatchmakingManager : Singleton<MatchmakingManager>
    {
        [SerializeField, Range(1, 12)] private int maxPlayersInLobby;


        private Callback<LobbyCreated_t> lobbyCreated;
        private Callback<GameLobbyJoinRequested_t> joinLobbyRequested;
        private Callback<LobbyEnter_t> lobbyEntered;

        private Callback<LobbyMatchList_t> lobbyListCallback;
        private Callback<LobbyDataUpdate_t> lobbyDataUpdatedCallback;

        private void Start()
        {
            if (SteamManager.Initialized)
            {
                Debug.Log(SteamUser.GetSteamID());
                lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
                lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
                lobbyListCallback = Callback<LobbyMatchList_t>.Create(OnLobbyListReceived);
            }
            else
            {
                Debug.LogError("Steam not initialized.");
            }
        }

        #region SteamCallbacks
        public void RequestLobbiesForGame()
        {
            SteamMatchmaking.AddRequestLobbyListStringFilter("game_id", "Mafia", ELobbyComparison.k_ELobbyComparisonEqual);
            SteamMatchmaking.RequestLobbyList();
        }

        private bool IsInLobby()
        {
            if (SteamMatchmaking.GetLobbyOwner(new CSteamID(SteamUser.GetSteamID().m_SteamID)) != CSteamID.Nil)
            {
                return true;
            }
            return false;
        }


        private void OnLobbyListReceived(LobbyMatchList_t result)
        {
            Debug.Log($"Number of lobbies found: {result.m_nLobbiesMatching}");
            if (!IsInLobby())
            {
                if (result.m_nLobbiesMatching > 0)
                {
                    for (int i = 0; i < result.m_nLobbiesMatching; i++)
                    {
                        CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
                        // SteamMatchmaking.RequestLobbyData(lobbyID);
                        if (lobbyID.IsValid() && lobbyID.IsLobby())
                        {
                            SteamMatchmaking.JoinLobby(lobbyID);
                            return;
                        }
                    }
                }
                CreateLobby();
            }
        }

      /*  private void OnLobbyDataUpdated(LobbyDataUpdate_t lobbyData)
        {
            if (lobbyData.m_bSuccess != 1 || !lobbyData.m_ulSteamIDLobby.Equals(lobbyData.m_ulSteamIDMember))
                return;

            CSteamID lobbyID = new CSteamID(lobbyData.m_ulSteamIDLobby);
            string lobbyName = SteamMatchmaking.GetLobbyData(lobbyID, "name");
            string gameID = SteamMatchmaking.GetLobbyData(lobbyID, "game_id");
            string currentMembers = SteamMatchmaking.GetLobbyData(lobbyID, "num_members");
            string maxMembers = SteamMatchmaking.GetLobbyData(lobbyID, "max_members");

            Debug.Log($"Lobby found: {lobbyName} (ID: {lobbyID})");

            if (gameID == "Mafia" && int.TryParse(currentMembers, out int current) && int.TryParse(maxMembers, out int max) && current < max)
            {
                SteamMatchmaking.JoinLobby(lobbyID);
                lobbyJoined = true;
                return;
            }

            if (!lobbyJoined)
            {
                CreateLobby();
            }
        }*/

        private void OnLobbyCreated(LobbyCreated_t param)
        {
            if (param.m_eResult != EResult.k_EResultOK)
            {
                Debug.LogError("Failed to create lobby");
                LoadingManager.Instance.EndLoading();
                return;
            }

            var id = new CSteamID(param.m_ulSteamIDLobby);
            SteamMatchmaking.SetLobbyData(id, "address", SteamUser.GetSteamID().ToString());
            SteamMatchmaking.SetLobbyData(id, "game_id", "Mafia");
            SteamMatchmaking.SetLobbyData(id, "num_members", "1");
            SteamMatchmaking.SetLobbyData(id, "max_members", $"{maxPlayersInLobby}");

            ((CustomNetworkManager)NetworkManager.singleton).SteamID = id;
            NetworkManager.singleton.StartHost();

            Debug.Log("Lobby created successfully");
           // LoadingManager.Instance.EndLoading();
        }

        private void OnLobbyEntered(LobbyEnter_t param)
        {
            if (NetworkServer.active) return;

            var id = new CSteamID(param.m_ulSteamIDLobby);
            NetworkManager.singleton.networkAddress = SteamMatchmaking.GetLobbyData(id, "address");
            ((CustomNetworkManager)NetworkManager.singleton).SteamID = id;
            NetworkManager.singleton.StartClient();

            Debug.Log("Joined lobby successfully");
           // LoadingManager.Instance.EndLoading();
        }
        #endregion

        public void CreateLobby()
        {
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, maxPlayersInLobby);
          //  LoadingManager.Instance.StartLoading();
        }

        public override MatchmakingManager GetInstance()
        {
            return this;
        }
    }
}
