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
        }

        private void OnScoreboardOpened(InputAction.CallbackContext obj)
        {
            scoreboardBody.SetActive(true);
        }

        public void Refresh()
        {
            ClearContainer();
            foreach (var player in FindObjectsOfType(typeof(NetworkPlayer)) as NetworkPlayer [])
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

        public GameObject CreateLabel (NetworkPlayer playerStats)
        {
            var label = Instantiate(scoreboardLabel);
            label.Construct(playerStats.GetName(),playerStats.GetKillCount(),playerStats.GetKillCount(), playerStats.GetDeathCount());
            return label.gameObject;
        }

        public override Scoreboard GetInstance()
        {
            return this;
        }
    }
}
