using UnityEngine;
using System.Collections;
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

        private Coroutine activeHealingCoroutine;
        private Coroutine passiveHealingCoroutine;

        #region Basic Class
        private void Start()
        {
            healthSystem = GetComponent<HealthSystem>();
            inputActions = new DefaultInput();
            inputActions.Enable();

            inputActions.Player.Heal.started += context => StartActiveHealing();
            inputActions.Player.Heal.canceled += context => StopActiveHealing();
        }

        private void Update()
        {
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

        private void StartActiveHealing()
        {
            if (isActiveHealing || healthSystem.GetCurrentHealth() >= healthSystem.GetMaxHealth())
                return;

            isActiveHealing = true;
            activeHealingCoroutine = StartCoroutine(ActiveHealingCoroutine());
        }

        private void StopActiveHealing()
        {
            if (!isActiveHealing) return;

            isActiveHealing = false;
            if (activeHealingCoroutine != null)
                StopCoroutine(activeHealingCoroutine);

            StartCoroutine(ShootingCooldownCoroutine());
        }

        private IEnumerator ActiveHealingCoroutine()
        {
            yield return new WaitForSeconds(activeHealDelay);

            while (isActiveHealing && healthSystem.GetCurrentHealth() < healthSystem.GetMaxHealth())
            {
                float newHealth = Mathf.Min(healthSystem.GetCurrentHealth() + activeHealSpeed * Time.deltaTime, healthSystem.GetMaxHealth());
                healthSystem.UpdateHealthBar(newHealth);

                yield return null;
            }

            StopActiveHealing();
        }

        private IEnumerator ShootingCooldownCoroutine()
        {
            inputActions.Player.Shoot.Disable();
            yield return new WaitForSeconds(shootCooldownAfterHeal);
            inputActions.Player.Shoot.Enable();
        }

        #endregion

        #region Passive Healing

        private void StartPassiveHealing()
        {
            if (isPassiveHealing || healthSystem.GetCurrentHealth() >= healthSystem.GetMaxHealth())
                return;

            isPassiveHealing = true;
            passiveHealingCoroutine = StartCoroutine(PassiveHealingCoroutine());
        }

        private void StopPassiveHealing()
        {
            if (!isPassiveHealing) return;

            isPassiveHealing = false;
            if (passiveHealingCoroutine != null)
                StopCoroutine(passiveHealingCoroutine);
        }

        private IEnumerator PassiveHealingCoroutine()
        {
            while (isPassiveHealing && healthSystem.GetCurrentHealth() < healthSystem.GetMaxHealth())
            {
                float newHealth = Mathf.Min(healthSystem.GetCurrentHealth() + passiveHealSpeed * Time.deltaTime, healthSystem.GetMaxHealth());
                healthSystem.UpdateHealthBar(newHealth);

                yield return null;
            }

            isPassiveHealing = false;
        }

        #endregion
    }
}
