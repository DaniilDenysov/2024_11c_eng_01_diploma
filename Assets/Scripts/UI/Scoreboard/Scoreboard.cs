using DesignPatterns.Singleton;
using Managers;
using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Score
{
    public class Scoreboard : NetworkSingleton<Scoreboard>
    {
        //change to dictionary
      //  [SerializeField] private List<PlayerStats> players = new List<PlayerStats>();
        [SerializeField] private GameObject scoreboardBody;
        [SerializeField] private Transform labelContainer;
        [SerializeField] private ScoreboardLabel scoreboardLabel;

        [SerializeField] private bool isDirty;

      //  public readonly SyncDictionary<string, PlayerStats> playerStats = new SyncDictionary<string, PlayerStats>();

        public void SetIsDirty()
        {
            if (!scoreboardBody.activeInHierarchy)
            {
                isDirty = true;
            }
            else
            {
                Refresh();
            }
        }

        private void Start()
        {
            var inputActions = new DefaultInput();
            inputActions.Enable();
            inputActions.Player.Scoreboard.performed += OnScoreboardOpened;
            inputActions.Player.Scoreboard.canceled += OnScoreboardClosed;
            //playerStats.OnAdd += OnNewPlayerAdded;
            //playerStats.OnRemove += OnPlayerRemoved;
            //playerStats.OnChange += OnPlayerStatsUpdated;
        }

     /*    [ClientRpc]
        public void OnPlayerStatsUpdated()//(SyncIDictionary<string, PlayerStats>.Operation arg1, string arg2, PlayerStats arg3)
        {
            //if (arg1 == SyncIDictionary<string, PlayerStats>.Operation.OP_CLEAR) return;
            //Debug.Log("Updated");
            Refresh();
        }

        [Server]
        public void AddKill (string nickname)
        {
            if (playerStats.TryGetValue(nickname, out PlayerStats stats))
            {
                stats.KillCount++;
                var newStats = new PlayerStats();
                newStats.Nickname = stats.Nickname;
                newStats.KillCount = stats.KillCount;
                newStats.AssistCount = stats.AssistCount;
                newStats.DeathCount = stats.DeathCount;
                stats = newStats;
            }
        }

        [Server]
        public void AddDeath(string nickname)
        {
            if (playerStats.TryGetValue(nickname, out PlayerStats stats))
            {          
                stats.DeathCount++;
                var newStats = new PlayerStats();
                newStats.Nickname = stats.Nickname;
                newStats.KillCount = stats.KillCount;
                newStats.AssistCount = stats.AssistCount;
                newStats.DeathCount = stats.DeathCount;
                stats = newStats;
            }
        }*/

        public void Refresh ()
        {
            isDirty = false;
            ClearContainer();
            foreach (var player in FindObjectsOfType(typeof(NetworkPlayer),true) as NetworkPlayer [])
            {
                Debug.Log("Refreshed");
                CreateLabel(player).transform.SetParent(labelContainer.transform);
            }
        }


        private void OnScoreboardClosed(InputAction.CallbackContext obj)
        {
            scoreboardBody.SetActive(false);
        }

        private void OnScoreboardOpened(InputAction.CallbackContext obj)
        {
            if (isDirty)
            {
                Refresh();
            }
            scoreboardBody.SetActive(true);
        }


  /*      [Command(requiresAuthority = false)]
        public void AddPlayerCmd (string nickname)
        {
            if (!playerStats.TryGetValue(nickname,out PlayerStats stats))
            {
                stats = new PlayerStats();
                stats.Nickname = nickname;
                playerStats.Add(nickname,stats);
                OnPlayerStatsUpdated();
            }
        }

        [Command(requiresAuthority = false)]
        public void RemovePlayerCmd (string nickname)
        {
            if (playerStats.TryGetValue(nickname, out PlayerStats stats))
            {
                playerStats.Remove(nickname);
            }
        }*/


        private void ClearContainer()
        {
            foreach (ScoreboardLabel child in labelContainer.GetComponentsInChildren<ScoreboardLabel>())
            {
                Destroy(child.gameObject);
            }
        }

        public GameObject CreateLabel (NetworkPlayer networkPlayer)
        {
            var label = Instantiate(scoreboardLabel);
            label.Construct(networkPlayer.GetName(), networkPlayer.GetKills(), networkPlayer.GetAssists(), networkPlayer.GetDeaths());
            return label.gameObject;
        }

        public override Scoreboard GetInstance()
        {
            return this;
        }
    }
}
