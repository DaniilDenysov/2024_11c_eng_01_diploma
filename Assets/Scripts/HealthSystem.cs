using UnityEngine;
using System.Collections.Generic;
using HealthSystem;
using ShootingSystem.Local;
using Mirror;
using Managers;
using Score;
using Kilfeed;

namespace HealthSystem
{
    public class HealthSystem : NetworkBehaviour, IDamagable
    {
        [SerializeField] private CustomSlider serverDisplay, clientDisplay;
        private CustomSlider _healthSlider;
        [SerializeField] private int assistsStackSize = 2;
        [SerializeField] private HealthSystem _armorSystem;
        [SerializeField] private bool _canTakeDamage = true;
        [SerializeField] private bool _enableTeamKill = false;
        private HealingSystem healingSystem;

        private Queue<NetworkConnectionToClient> damageHistory = new Queue<NetworkConnectionToClient>();


        public override void OnStartServer()
        {
            base.OnStartServer();
            SetSlider();

            healingSystem = GetComponent<HealingSystem>();
            if (healingSystem == null)
            {
                Debug.LogError("HealingSystem is missing from the GameObject!");
            }
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            SetSlider();
        }

        public void SetSlider ()
        {
            if (isLocalPlayer)
            {
                _healthSlider = clientDisplay;
            }
            else
            {
                _healthSlider = serverDisplay;
            }
        }

        [Server]
        public void AddAssist (NetworkConnectionToClient conn)
        {
            if (conn == null) return;
            if (damageHistory.Contains(conn)) return;
            if (damageHistory.Count > assistsStackSize) damageHistory.Peek();
            damageHistory.Enqueue(conn);
        }

        [Server]
        public void AddAssistance(NetworkConnectionToClient killer)
        {
            foreach (var conn in damageHistory)
            {
                if (conn == null)
                {
                    Debug.LogError("Unable to add assistance, connection to client is null!");
                    continue;
                }
                if (conn == killer)
                {
                    Debug.LogWarning("Unable to add assistance for killer!");
                    continue;
                }
                if (conn.identity == null)
                {
                    Debug.LogError("Unable to add assistance, client identity is null!");
                    continue;
                }
                if (!conn.identity.TryGetComponent(out NetworkPlayer networkPlayer))
                {
                    Debug.LogError("Unable to add assistance, network player component not found!");
                    continue;
                }
                networkPlayer.SetAssists(networkPlayer.GetAssists()+1);
            }
        }

        [Command(requiresAuthority = false)] //change to server
        public void DoDamage(float damage, NetworkConnectionToClient conn = null)
        {
            var killer = conn.identity.GetComponent<NetworkPlayer>();
            var victim = netIdentity.GetComponent<NetworkPlayer>();

            if (!_enableTeamKill)
            {
                if (killer.GetTeamGuid().Equals(victim.GetTeamGuid())) return;
            }

            if (!_canTakeDamage)
                return;

            if (_armorSystem != null && _armorSystem.GetCurrentHealth() > 0)
            {
                _armorSystem.DoDamage(damage, conn);
                return;
            }

            float newHealth = 0f;

            if (_healthSlider.GetCurrentValue() - damage <= 0)
            {
                newHealth = 0;

               
                killer.SetKills(killer.GetKills()+1);
                victim.SetDeaths(victim.GetDeaths()+1);
                AddAssistance(conn);
                InitiateRespawn(netIdentity.connectionToClient);
                OnPlayerDied(killer.GetName(), KillfeedManager.Instance.GetPhrase(), victim.GetName());
            }
            else
            {
                newHealth = _healthSlider.GetCurrentValue() - damage;
                AddAssist(conn);
                if (healingSystem != null)
                {
                    healingSystem.NotifyDamageTaken();
                }
            }

            UpdateHealthBar(newHealth);

            Debug.Log($"Remaining Health: {newHealth}");
        }

        [TargetRpc]
        public void InitiateRespawn (NetworkConnectionToClient conn)
        {
            RespawnManager.Instance.ActivateRespawnMenu();
        }

        [ClientRpc]
        public void OnPlayerDied (string killer,string phrase,string victim)
        {
            gameObject.SetActive(false);
            KillfeedManager.Instance.DisplayFeed(killer, phrase, victim);
        }

        [ClientRpc]
        public void SetCanTakeDamage(bool value)
        {
            _canTakeDamage = value;
        }

        [Server]
        public float GetCurrentHealth() => _healthSlider.GetCurrentValue();

        [Server]
        public float GetMaxHealth() => _healthSlider.GetMaxValue();

        [Server]
        public float GetHealthPercentage() => _healthSlider.GetCurrentValue() / _healthSlider.GetMaxValue();


        [ClientRpc]
        public void UpdateHealthBar(float health)
        {
            _healthSlider.SetCurrentValue(health);
        }
    }
}