using UnityEngine;
using System.Collections.Generic;
using ShootingSystem.Local;
using Mirror;

namespace HealthSystem
{
    public class HealthSystem : NetworkBehaviour, IDamagable
    {

        [SerializeField] private CustomSlider _healthSlider;
        [SerializeField] private HealthSystem _armorSystem;
        [SerializeField] private bool _canTakeDamage = true;

        private float _currentHealth;

        void Start()
        {
            _currentHealth = _healthSlider.GetCurrentValue();
        }

        [Server]
        public void DoDamage(float damage)
        {
            if (!_canTakeDamage)
                return;

            if (_armorSystem != null && _armorSystem.GetCurrentHealth() > 0)
            {
                _armorSystem.DoDamage(damage);
                return;
            }
            _currentHealth = _healthSlider.GetCurrentValue() - damage;
            _healthSlider.SetCurrentValue(_currentHealth);

            Debug.Log($"Remaining Health: {_currentHealth}");

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
            }
        }

        [Server]
        public void SetCanTakeDamage(bool value)
        {
            _canTakeDamage = value;
        }

        [Server]
        public float GetCurrentHealth() => _currentHealth;

        [Server]
        public float GetMaxHealth() => _healthSlider.GetMaxValue();

        [Server]
        public float GetHealthPercentage() => _currentHealth / _healthSlider.GetMaxValue();

        [Server]
        private void UpdateHealthBar()
        {
            _healthSlider.SetCurrentValue(_currentHealth);
        }
    }
}