using System;
using UnityEngine;
using PlayerStatistique.Runtime;

namespace Health.Runtime
{
    public class Health : MonoBehaviour
    {
        #region Publics

        public bool m_isPlayer = false; 
        
        public event Action OnDamage;
        public event Action OnHeal;
        public event Action<float> OnHPChange;
        public event Action<float> OnHPChangeNormalized;
        public event Action OnDeath;

        
        public float CurrentHP => _currentHealth;
        public float MaxHP => _maxHealth;
        
        #endregion


        #region Unity API

        private void Start()
        {
            ResetHealth();
        }

        public void Initialize(Statistique stats)
        {
            _statistique = stats;
        }

        #endregion


        #region Main API

        public void TakeDamage(float damagePoints)
        {
            if (_isDead) return;

            _currentHealth -= damagePoints;
            HandleHealthChange();
            OnDamage?.Invoke();

            if (_currentHealth > 0f) return;

            _isDead = true;
            OnDeath?.Invoke();
            if (!m_isPlayer) Destroy(gameObject); 
        }

        public void Heal(float healPoints)
        {
            if (_isDead) return;

            _currentHealth += healPoints;
            HandleHealthChange();
            OnHeal?.Invoke();
        }
        
        public void SetMaxHealth(float maxHealth)
        {
            _maxHealth = maxHealth;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
            HandleHealthChange();
        }

        public void ResetHealth()
        {
            _isDead = false;
            _maxHealth = _statistique.m_MaxHp;
            _currentHealth = _maxHealth;
        }

        private void HandleHealthChange()
        {
            _currentHealth = Mathf.Clamp(_currentHealth, 0f, _maxHealth);
            OnHPChange?.Invoke(_currentHealth);
            OnHPChangeNormalized?.Invoke(_currentHealth / _maxHealth);
        }
        
        #endregion


        #region Private and Protected

        [SerializeField] private Statistique _statistique;

        private float _currentHealth;
        private float _maxHealth;
        public bool _isDead;

        #endregion
    }
}