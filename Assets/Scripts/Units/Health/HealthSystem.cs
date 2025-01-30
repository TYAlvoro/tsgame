using System;
using UnityEngine;

/// <summary>
/// Manages entity health and coordinates with UI system
/// </summary>
public class HealthSystem : MonoBehaviour
{
    [System.Serializable]
    public class Settings
    {
        [Tooltip("Maximum health points")]
        public float maxHealth = 100f;
        [Tooltip("Healthbar position offset from entity")]
        public Vector3 uiOffset = new Vector3(0, 2f, 0);
        [Tooltip("Maximum visibility distance for healthbar")]
        public float uiVisibilityDistance = 15f;
        [Tooltip("Base scale factor for healthbar")]
        public float uiScale = 0.05f;
    }

    [SerializeField] private Settings _settings;
    public float CurrentHealth { get; private set; }
    public event Action<float> OnHealthChanged;
    public event Action OnDeath;
    private HealthUI _healthUI;

    public Settings GetSettings() => _settings;

    private void Start()
    {
        CurrentHealth = _settings.maxHealth;
        InitializeHealthUI();
    }

    private void InitializeHealthUI()
    {
        if (HealthUIManager.Instance == null)
        {
            Debug.LogError("[HealthSystem] HealthUIManager instance not found!");
            return;
        }
        _healthUI = HealthUIManager.Instance.RequestHealthUI(this);
    }

    /// <summary>
    /// Applies damage to the entity
    /// </summary>
    /// <param name="damage">Damage amount</param>
    /// <param name="damageType">Type of damage</param>
    public void TakeDamage(float damage, DamageType damageType)
    {
        if (damage <= 0 || CurrentHealth <= 0) return;

        // Apply resistance or vulnerability based on damage type
        float effectiveDamage = ApplyDamageModifiers(damage, damageType);

        CurrentHealth = Mathf.Max(0, CurrentHealth - effectiveDamage);
        OnHealthChanged?.Invoke(CurrentHealth / _settings.maxHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private float ApplyDamageModifiers(float damage, DamageType damageType)
    {
        // Example: Apply resistance or vulnerability based on damage type
        switch (damageType)
        {
            case DamageType.Physical:
                return damage * 1.0f; // No modifier
            case DamageType.Fire:
                return damage * 1.2f; // Vulnerable to fire
            case DamageType.Magic:
                return damage * 0.8f; // Resistant to magic
            default:
                return damage;
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (HealthUIManager.Instance != null)
        {
            HealthUIManager.Instance.ReturnHealthUI(this);
        }
        OnHealthChanged = null;
        OnDeath = null;
    }
}

/// <summary>
/// Represents different types of damage
/// </summary>
public enum DamageType
{
    Physical,
    Fire,
    Magic,
    Poison
}