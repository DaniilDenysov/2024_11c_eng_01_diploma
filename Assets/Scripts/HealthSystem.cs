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
        [SerializeField] private HealthSystem _armorSystem;
        [SerializeField] private bool _canTakeDamage = true;
        private HealingSystem healingSystem;



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

        [Command(requiresAuthority = false)] //change to server
        public void DoDamage(float damage, NetworkConnectionToClient conn = null)
        {
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
                var killer = conn.identity.GetComponent<NetworkPlayer>();
                killer.AddKill();
                var victim = netIdentity.GetComponent<NetworkPlayer>();
                victim.AddDeath();
                
                InitiateRespawn(netIdentity.connectionToClient);
                OnPlayerDied(killer.GetName(), KillfeedManager.Instance.GetPhrase(),victim.GetName());
            }
            else
            {
                newHealth = _healthSlider.GetCurrentValue() - damage;

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