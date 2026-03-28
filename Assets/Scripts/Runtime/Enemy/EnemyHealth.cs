using System;
using UnityEngine;
using MobaRoguelike.Core.Abilities;

namespace MobaRoguelike.Runtime.Enemy
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _maxHealth = 100f;

        private float _currentHealth;

        public event Action<float> OnDamaged;
        public event Action OnDeath;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth     => _maxHealth;

        public void Configure(float maxHealth)
        {
            _maxHealth     = maxHealth;
            _currentHealth = maxHealth;
        }

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(float amount, int sourceId)
        {
            if (_currentHealth <= 0f) return;

            _currentHealth = Mathf.Max(0f, _currentHealth - amount);
            OnDamaged?.Invoke(amount);

            if (_currentHealth <= 0f)
            {
                OnDeath?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
