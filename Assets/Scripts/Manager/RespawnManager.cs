using DesignPatterns.Singleton;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using SynchronizedClock;
using Mirror;
using System.Linq;

namespace Managers
{
    public class RespawnManager : Singleton<RespawnManager>
    {
        [SerializeField,Range(0,100f)] private float respawnTime = 10f;
        [SerializeField] private TMP_Text display;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject respawnScreen;
        Clock clock;

        public override RespawnManager GetInstance()
        {
            return this;
        }

        public void ActivateRespawnMenu ()
        {
            clock = new Clock(OnClockTick, OnClockStopped);
            clock.Start(respawnTime);
            respawnScreen.SetActive(true);
        }

        private void Update()
        {
            if (clock == null) return;
            clock.Tick(Time.deltaTime);
        }

        private void OnClockTick (float currentTime)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            display.text = $"Time left: {minutes}:{seconds:D2}";
        }

        private void OnClockStopped()
        {
            respawnScreen.SetActive(false);
            Respawn();
        }

        private void Respawn()
        {
            var conn = NetworkServer.connections.Values.FirstOrDefault(c => c.identity != null && c.identity.isLocalPlayer);
            if (conn == null)
            {
                Debug.LogError("Could not find the player's connection.");
                return;
            }

            ((CustomNetworkManager)NetworkManager.singleton).SpawnPlayerAt(spawnPoint.transform.position, conn);
        }
    }
}
    