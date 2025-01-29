using System.Linq;
using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [System.Serializable]
    public class DamageSettings
    {
        [Tooltip("Base damage amount applied per attack")]
        public float damageAmount = 10f;

        [Tooltip("Minimum time between consecutive damage applications")]
        public float damageCooldown = 1f;

        [Tooltip("Should damage be applied on physical contact?")]
        public bool damageOnCollision = true;

        [Tooltip("Tags of objects that can receive damage")]
        public string[] damageableTags = { "Player", "Enemy" };
    }

    [SerializeField] private DamageSettings _settings;
    [SerializeField] private bool _showDebug = true;

    private float _lastDamageTime;

    private void OnCollisionEnter(Collision collision) => HandleContact(collision.gameObject, "Collision");
    private void OnTriggerEnter(Collider other) => HandleContact(other.gameObject, "Trigger");

    public void DealDamage(GameObject target)
    {
        if (target == null) return;

        if (IsOnCooldown())
        {
            LogCooldownStatus();
            return;
        }

        if (target.TryGetComponent<HealthSystem>(out var healthSystem))
        {
            ApplyDamage(healthSystem);
            LogDamageApplied(target.name);
        }
        else
        {
            LogMissingHealthSystem(target.name);
        }

        UpdateCooldown();
    }

    private void HandleContact(GameObject contactTarget, string contactType)
    {
        if (!_settings.damageOnCollision) return;

        if (_showDebug)
        {
            Debug.Log($"{contactType} detected with {contactTarget.name}");
        }

        if (CanDamage(contactTarget))
        {
            DealDamage(contactTarget);
        }
        else
        {
            LogInvalidTarget(contactTarget.name);
        }
    }

    private bool CanDamage(GameObject target)
    {
        return target != null &&
               _settings.damageableTags.Any(tag => target.CompareTag(tag));
    }

    private bool IsOnCooldown()
    {
        return Time.time - _lastDamageTime < _settings.damageCooldown;
    }

    private void ApplyDamage(HealthSystem healthSystem)
    {
        healthSystem.TakeDamage(_settings.damageAmount);
    }

    private void UpdateCooldown()
    {
        _lastDamageTime = Time.time;
    }

    private void LogCooldownStatus()
    {
        if (!_showDebug) return;
        float remaining = _settings.damageCooldown - (Time.time - _lastDamageTime);
        Debug.Log($"Damage cooldown active. Time remaining: {remaining:F1}s");
    }

    private void LogDamageApplied(string targetName)
    {
        if (!_showDebug) return;
        Debug.Log($"Dealt {_settings.damageAmount} damage to {targetName}");
    }

    private void LogMissingHealthSystem(string targetName)
    {
        if (!_showDebug) return;
        Debug.LogWarning($"No HealthSystem component found on {targetName}");
    }

    private void LogInvalidTarget(string targetName)
    {
        if (!_showDebug) return;
        Debug.Log($"Cannot damage {targetName}. Invalid tag or missing component.");
    }
}