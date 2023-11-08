using System;
using System.Collections;
using UnityEngine;

namespace swzwij.HealthData
{
    /// <summary>
    /// A class that represents health-related data for a game character or entity.
    /// It manages health, damage, and related events.
    /// </summary>
    public class HealthData : MonoBehaviour
    {
        #region Private Variables

        /// <summary>
        /// The current health of the entity.
        /// </summary>
        [SerializeField]
        private float _health = 100f;

        /// <summary>
        /// Determines if the entity can over-heal beyond its maximum health.
        /// </summary>
        [SerializeField]
        private bool _canOverHeal;

        /// <summary>
        /// The duration of invincibility after taking damage in seconds.
        /// </summary>
        [SerializeField]
        private float _invincibilityTime = 0.1f;

        /// <summary>
        /// The maximum health of the entity.
        /// </summary>
        private float _maxHealth;

        /// <summary>
        /// Indicates whether the entity is being hit.
        /// </summary>
        private bool _isHit;

        /// <summary>
        /// Indicates whether the entity is dead.
        /// </summary>
        private bool _isDead;

        #endregion


        #region Public Getters

        /// <summary>
        /// The current health of the entity.
        /// </summary>
        public float Health => _health;

        /// <summary>
        /// The maximum health of the entity.
        /// </summary>
        public float MaxHealth => _maxHealth;

        /// <summary>
        /// Indicates whether the entity has reached its maximum health.
        /// </summary>
        public bool HasMaxHealth => _health >= _maxHealth;

        /// <summary>
        /// Indicates whether the entity is dead.
        /// </summary>
        public bool IsDead => _isDead;

        /// <summary>
        /// Indicates whether the entity is being hit.
        /// </summary>
        public bool IsHit => _isHit;

        #endregion


        #region Public Events

        [Header("Events")]

        /// <summary>
        /// Event triggered when the entity's health changes.
        /// </summary>
        public Action<float> OnHealthChanged;

        /// <summary>
        /// Event triggered when health is added to the entity.
        /// </summary>
        public Action<float> OnHealthAdded;

        /// <summary>
        /// Event triggered when the entity takes damage.
        /// </summary>
        public Action<float> OnDamageTaken;

        /// <summary>
        /// Event triggered when the entity dies.
        /// </summary>
        public Action OnDeath;

        /// <summary>
        /// Event triggered when the entity is resurrected.
        /// </summary>
        public Action OnResurrected;

        #endregion


        #region Public Functions

        /// <summary>
        /// Adds health to the entity. 
        /// Handles over-healing if allowed.
        /// </summary>
        /// <param name="healthAmount">The amount of health to add to the entity.</param>
        public void AddHealth(float healthAmount)
        {
            if (_isDead || (!_canOverHeal && HasMaxHealth))
                return;

            _health += healthAmount;

            if (_health > _maxHealth && !_canOverHeal)
                _health = _maxHealth;

            OnHealthChanged?.Invoke(_health);
            OnHealthAdded?.Invoke(healthAmount);
        }

        /// <summary>
        /// Inflicts damage on the entity.
        /// </summary>
        /// <param name="damageAmount">The amount of damage to inflict on the entity.</param>
        public void TakeDamage(float damageAmount)
        {
            if (_isDead || _isHit)
                return;

            StartCoroutine(ActivateInvincibility());

            _health -= damageAmount;

            OnHealthChanged?.Invoke(_health);
            OnDamageTaken?.Invoke(damageAmount);

            if (_health <= 0) Die();
        }

        /// <summary>
        /// Resurrects the entity with the specified health amount.
        /// </summary>
        /// <param name="newHealth">The health amount to resurrect the entity with.</param>
        public void Resurrect(float newHealth)
        {
            _isDead = false;

            AddHealth(newHealth);

            OnHealthChanged?.Invoke(_health);
            OnResurrected?.Invoke();
        }

        /// <summary>
        /// Marks the entity as dead.
        /// </summary>
        public void Kill() => Die();

        /// <summary>
        /// Destroys the GameObject associated with this entity.
        /// </summary>
        public void DestroySelf() => Destroy(gameObject);

        #endregion


        #region Private Functions

        /// <summary>
        /// Initializes health data on the entity's Awake.
        /// </summary>
        private void Awake() => InitializeHealth();

        /// <summary>
        /// Initializes the entity's maximum health value.
        /// </summary>
        private void InitializeHealth() => _maxHealth = _health;

        /// <summary>
        /// Activates invincibility for a brief period after being hit.
        /// </summary>
        private IEnumerator ActivateInvincibility()
        {
            _isHit = true;
            yield return new WaitForSeconds(_invincibilityTime);
            _isHit = false;
        }

        /// <summary>
        /// Handles entity death by setting health to zero and triggering death events.
        /// </summary>
        private void Die()
        {
            _health = 0;
            _isDead = true;

            OnHealthChanged?.Invoke(Health);
            OnDeath?.Invoke();
        }

        #endregion
    }
}