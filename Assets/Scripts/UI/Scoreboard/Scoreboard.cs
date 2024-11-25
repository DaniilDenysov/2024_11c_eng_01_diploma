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
        [SerializeField] private List<PlayerStats> players = new List<PlayerStats>();
        [SerializeField] private GameObject scoreboardBody;
        [SerializeField] private Transform labelContainer;
        [SerializeField] private ScoreboardLabel scoreboardLabel;

        private void Start()
        {
            var inputActions = new DefaultInput();
            inputActions.Enable();
            inputActions.Player.Scoreboard.performed += OnScoreboardOpened;
            inputActions.Player.Scoreboard.canceled += OnScoreboardClosed;
        }

        private void OnScoreboardClosed(InputAction.CallbackContext obj)
        {
            scoreboardBody.SetActive(false);
            ClearContainer();
        }

        private void OnScoreboardOpened(InputAction.CallbackContext obj)
        {
            Refresh();
            scoreboardBody.SetActive(true);
        }

        public void Refresh()
        {
            foreach (var player in players)
            {
                CreateLabel(player).transform.SetParent(labelContainer.transform);
            }
        }

        private void ClearContainer()
        {
            foreach (ScoreboardLabel child in labelContainer.GetComponentsInChildren<ScoreboardLabel>())
            {
                Destroy(child.gameObject);
            }
        }

        [ClientRpc]
        public void AddKillFor (string nickname)
        {
            var player = players.FirstOrDefault((p) => p.Nickname == nickname);
            if (player != null)
            {
                player.KillCount++;
            }
            else
            {
                player = AddPlayer(nickname);
                player.KillCount++;
            }
        }

        [ClientRpc]
        public void AddDeathFor(string nickname)
        {
            var player = players.FirstOrDefault((p) => p.Nickname == nickname);
            if (player != null)
            {
                player.DeathCount++;
            }
            else
            {
                player = AddPlayer(nickname);
                player.DeathCount++;
            }
        }

        public PlayerStats AddPlayer(string nickname)
        {
            if (players.FirstOrDefault((p) => p.Nickname == nickname) != null) return null;
            var newPlayer = new PlayerStats();
            newPlayer.Nickname = nickname;
            players.Add(newPlayer);
            return newPlayer;
        }

        public GameObject CreateLabel (PlayerStats playerStats)
        {
            var label = Instantiate(scoreboardLabel);
            label.Construct(playerStats.Nickname,playerStats.KillCount,playerStats.AssistCount,playerStats.DeathCount);
            return label.gameObject;
        }

        public override Scoreboard GetInstance()
        {
            return this;
        }
    }
}
