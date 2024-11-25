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
    public class RespawnManager : NetworkSingleton<RespawnManager>
    {
        [SerializeField,Range(0,100f)] private float respawnTime = 10f;
        [SerializeField] private TMP_Text display;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private GameObject respawnScreen;
        [SerializeField] private GameObject respawnButton;
        Clock clock;

        public override RespawnManager GetInstance()
        {
            return this;
        }

        public void ActivateRespawnMenu ()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            clock = new Clock(
            OnClockTick, 
            () =>
            {
                respawnButton.SetActive(true);   
            });
            clock.Start(respawnTime);
            respawnScreen.SetActive(true);
        }

        public void OnRespawn ()
        {
            respawnButton.SetActive(false);
            respawnScreen.SetActive(false);
            RespawnPlayer();
        }

        [Command(requiresAuthority = false)]
        public void RespawnPlayer (NetworkConnectionToClient conn = null)
        {
            ((CustomNetworkManager)NetworkManager.singleton).SpawnPlayerAt(spawnPoint.transform.position, conn);
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
            display.text = $"Time to respawn: {minutes}:{seconds:D2}";
        }
    }
}
    