using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100; // Maximum health of the object
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth; // Initialize current health
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health stays within bounds

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    void Die()
    {
        // Destroy the object for now; expand later with animations or effects
        Destroy(gameObject);
    }
}
