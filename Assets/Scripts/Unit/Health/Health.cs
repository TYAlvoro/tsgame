using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private HealthData config;
    private float currentHealth;
    private bool isDead = false;

    public HealthData Config => config;

    private void Awake() => currentHealth = config.MaxHealth;

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0, config.MaxHealth);
        HealthEventSystem.OnHealthChanged.Invoke(gameObject, currentHealth, config.MaxHealth);

        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        HealthEventSystem.OnDeath.Invoke(gameObject);
        if (config.DestroyOnDeath) Destroy(gameObject, config.DeathDelay);
    }

    public void Heal(float amount) => TakeDamage(-amount);
    public float GetCurrentHealth() => currentHealth;
}