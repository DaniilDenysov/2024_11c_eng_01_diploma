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
using SynchronizedClock;

namespace Managers  
{
    public class CustomNetworkManager : NetworkManager
    {
        public CSteamID SteamID { get; set; }
        public GameModeSO gameModeSO;

        [Header("Team distribution settings")]
        [SerializeField] private TeamDistributionMode distributionMode = TeamDistributionMode.RoundRobin;
        private int roundRobinLastIndex = 0;


        private int currentRound = 0;

        public enum TeamDistributionMode
        {
            RoundRobin,
            Equality
        }
        
        /// <summary>
        /// key - guid/name  value - list of nicknames
        /// </summary>
        [SerializeField] private Dictionary<string, List<string>> formedTeams = new Dictionary<string, List<string>>();

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log("Connected to server");
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            if (formedTeams.Count > 0)
            {
                Debug.LogError("Teams are already formed!");
                return;
            }
            if (gameModeSO == null)
            {
                Debug.LogError("Game mode is null!");
                return;
            }
            StartNewRound(0,gameModeSO.RoundTime);
            FormTeams();
        }


        #region TeamHandling

        public bool TryAddToTeam(string nickname, out string teamGuid)
        {
            teamGuid = FindAvailableTeam();

            if (string.IsNullOrEmpty(teamGuid)) return false;

            if (!formedTeams.ContainsKey(teamGuid))
            {
                Debug.LogError($"Team with GUID {teamGuid} does not exist.");
                return false;
            }

            if (formedTeams[teamGuid].Count >= gameModeSO.GetPlayerCountForTeam(teamGuid))
            {
                Debug.LogWarning($"Team {teamGuid} is full.");
                return false;
            }

            formedTeams[teamGuid].Add(nickname);
            return true;
        }


       /* public Color GetTeamColor(string teamGuid)
        {
            return gameModeSO.teams.FirstOrDefault((t) => t.Guid.Equals(teamGuid)).color;
        }*/

        public string FindAvailableTeam()
        {
            if (formedTeams == null || formedTeams.Count == 0)
            {
                throw new InvalidOperationException("No teams are available.");
            }

            if (distributionMode == TeamDistributionMode.RoundRobin)
            {
                var teamKeys = new List<string>(formedTeams.Keys);

                if (roundRobinLastIndex >= teamKeys.Count) roundRobinLastIndex = 0;

                for (; roundRobinLastIndex < teamKeys.Count;)
                {
                    if (gameModeSO.GetPlayerCountForTeam(teamKeys[roundRobinLastIndex]) > formedTeams[teamKeys[roundRobinLastIndex]].Count)
                    {
                        return teamKeys[roundRobinLastIndex++];
                    }
                    roundRobinLastIndex++;
                }
            }
            else if (distributionMode == TeamDistributionMode.Equality)
            {
                string smallestTeam = null;
                int minPlayerCount = int.MaxValue;

                foreach (var team in formedTeams)
                {
                    int currentCount = team.Value.Count;
                    if (currentCount < minPlayerCount)
                    {
                        minPlayerCount = currentCount;
                        smallestTeam = team.Key;
                    }
                }

                return smallestTeam;
            }

            return null;
        }




        private void FormTeams()
        {
            if (gameModeSO == null || gameModeSO.teams == null || gameModeSO.teams.Count == 0)
            {
                Debug.LogError("GameModeSO or teams are not configured properly.");
                return;
            }

            foreach (var team in gameModeSO.teams)
            {
                if (!formedTeams.ContainsKey(team.Guid))
                {
                    formedTeams[team.Guid] = new List<string>();
                }
            }
        }


        #endregion

        #region RoundHandling

        private void StartNewRound (int delay, int roundTime)
        {
            SyncedClock.Instance.StartTimer(roundTime);
        }

        #endregion

        public override void OnStopServer()
        {
            base.OnStopServer();
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            if (conn.identity != null)
            {
                var player = conn.identity.GetComponent<NetworkPlayer>();
                if (player != null)
                {
                    string teamGuid = player.GetTeamGuid();
                    if (!string.IsNullOrEmpty(teamGuid) && formedTeams.ContainsKey(teamGuid))
                    {
                        formedTeams[teamGuid].Remove(player.GetName());
                    }
                }
            }

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
                newPlayerInfo.SetTeamGuid(oldPlayerInfo.GetTeamGuid());
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
