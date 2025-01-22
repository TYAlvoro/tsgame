using System.Linq;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [System.Serializable]
    public class DamageSettings
    {
        public float damageAmount = 10f;
        public float damageCooldown = 1f;
        public bool damageOnCollision = true;
        public string[] damagableTags = { "Player", "Enemy" };
    }

    [SerializeField] private DamageSettings _settings;
    private float _lastDamageTime;

    // Для отладки
    [SerializeField] private bool _showDebug = true;

    public void DealDamage(GameObject target)
    {
        if (Time.time - _lastDamageTime < _settings.damageCooldown)
        {
            if (_showDebug) Debug.Log($"Damage cooldown active. Time remaining: {_settings.damageCooldown - (Time.time - _lastDamageTime)}");
            return;
        }

        var healthSystem = target.GetComponent<HealthSystem>();
        if (healthSystem != null)
        {
            healthSystem.TakeDamage(_settings.damageAmount);
            _lastDamageTime = Time.time;
            if (_showDebug) Debug.Log($"Dealt {_settings.damageAmount} damage to {target.name}");
        }
        else
        {
            if (_showDebug) Debug.LogWarning($"No HealthSystem found on {target.name}");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!_settings.damageOnCollision) return;

        if (_showDebug) Debug.Log($"Collision detected with {collision.gameObject.name}");

        if (CanDamage(collision.gameObject))
        {
            DealDamage(collision.gameObject);
        }
        else
        {
            if (_showDebug) Debug.Log($"Cannot damage {collision.gameObject.name}. Invalid tag.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_settings.damageOnCollision) return;

        if (_showDebug) Debug.Log($"Trigger detected with {other.gameObject.name}");

        if (CanDamage(other.gameObject))
        {
            DealDamage(other.gameObject);
        }
        else
        {
            if (_showDebug) Debug.Log($"Cannot damage {other.gameObject.name}. Invalid tag.");
        }
    }

    private bool CanDamage(GameObject target)
    {
        return _settings.damagableTags.Any(tag => target.CompareTag(tag));
    }
}