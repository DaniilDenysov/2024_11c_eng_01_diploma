using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using Mirror;

namespace HealthSystem
{
    public class HealingSystem : NetworkBehaviour
    {
        [Header("Active Healing")]
        [SerializeField] private float activeHealSpeed = 20f;
        [SerializeField] private float activeHealDelay = 0.5f;
        [SerializeField] private float shootCooldownAfterHeal = 0.5f;

        [Header("Passive Healing")]
        [SerializeField] private float passiveHealSpeed = 4f;
        [SerializeField] private float passiveHealDelay = 6f;

        private bool isActiveHealing = false;
        private bool isPassiveHealing = false;
        private float lastDamageTime;

        private HealthSystem healthSystem;
        private DefaultInput inputActions;

        private CancellationTokenSource activeHealingCts;
        private CancellationTokenSource passiveHealingCts;

        #region Basic Class
        private void Start()
        {
            healthSystem = GetComponent<HealthSystem>();
            inputActions = new DefaultInput();
            inputActions.Enable();

            inputActions.Player.Heal.started += context => { if (isLocalPlayer) StartActiveHealing(); };
            inputActions.Player.Heal.canceled += context => { if (isLocalPlayer) StopActiveHealing(); };
        }

        private void OnDisable()
        {
            activeHealingCts?.Cancel();
            passiveHealingCts?.Cancel();
        }

        private void OnDestroy()
        {
            activeHealingCts?.Cancel();
            passiveHealingCts?.Cancel();
        }

        private void Update()
        {
            if (!NetworkServer.active) return;
            if (Time.time > lastDamageTime + passiveHealDelay && !isActiveHealing && !isPassiveHealing)
            {
                StartPassiveHealing();
            }
        }

        public void NotifyDamageTaken()
        {
            lastDamageTime = Time.time;
            StopPassiveHealing();
        }
        #endregion

        #region Active Healing

        [Command]
        private void StartActiveHealing()
        {
            if (isActiveHealing || healthSystem.GetCurrentHealth() >= healthSystem.GetMaxHealth())
                return;

            isActiveHealing = true;
            activeHealingCts = new CancellationTokenSource();
            _ = ActiveHealingAsync(activeHealingCts.Token);
        }

        [Command]
        private void StopActiveHealing()
        {
            if (!isActiveHealing) return;

            isActiveHealing = false;
            activeHealingCts?.Cancel();

            _ = ShootingCooldownAsync();
        }

        private async Task ActiveHealingAsync(CancellationToken token)
        {
            try
            {
                await Task.Delay((int)(activeHealDelay * 1000), token);

                while (!token.IsCancellationRequested && isActiveHealing && healthSystem.GetCurrentHealth() < healthSystem.GetMaxHealth())
                {
                    float newHealth = Mathf.Min(healthSystem.GetCurrentHealth() + activeHealSpeed * Time.deltaTime, healthSystem.GetMaxHealth());
                    healthSystem.UpdateHealthBar(newHealth);

                    await Task.Yield();
                }
            }
            catch (TaskCanceledException)
            {
               
            }

            StopActiveHealing();
        }

        private async Task ShootingCooldownAsync()
        {
            inputActions.Player.Shoot.Disable();
            await Task.Delay((int)(shootCooldownAfterHeal * 1000));
            inputActions.Player.Shoot.Enable();
        }

        #endregion

        #region Passive Healing

        private void StartPassiveHealing()
        {
            if (isPassiveHealing || healthSystem.GetCurrentHealth() >= healthSystem.GetMaxHealth())
                return;

            isPassiveHealing = true;
            passiveHealingCts = new CancellationTokenSource();
            _ = PassiveHealingAsync(passiveHealingCts.Token);
        }

        private void StopPassiveHealing()
        {
            if (!isPassiveHealing) return;

            isPassiveHealing = false;
            passiveHealingCts?.Cancel();
        }

        private async Task PassiveHealingAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested && isPassiveHealing && healthSystem.GetCurrentHealth() < healthSystem.GetMaxHealth())
                {
                    float newHealth = Mathf.Min(healthSystem.GetCurrentHealth() + passiveHealSpeed * Time.deltaTime, healthSystem.GetMaxHealth());
                    healthSystem.UpdateHealthBar(newHealth);

                    await Task.Yield();
                }
            }
            catch (TaskCanceledException)
            {

            }

            isPassiveHealing = false;
        }

        #endregion
    }
}
