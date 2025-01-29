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

    /// <summary> Current health value </summary>
    public float CurrentHealth { get; private set; }

    /// <summary> Called when health changes (0-1 normalized value) </summary>
    public event System.Action<float> OnHealthChanged;

    /// <summary> Called when health reaches zero </summary>
    public event System.Action OnDeath;

    private HealthUI _healthUI;

    /// <summary>
    /// Returns current settings configuration
    /// </summary>
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
    public void TakeDamage(float damage)
    {
        if (damage <= 0) return; // Prevent negative or zero damage
        if (CurrentHealth <= 0) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        OnHealthChanged?.Invoke(CurrentHealth / _settings.maxHealth);

        if (CurrentHealth <= 0)
        {
            Die();
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