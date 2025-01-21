using UnityEngine;
using UnityEngine.Events;

public static class HealthEventSystem
{
    public static UnityEvent<GameObject, float, float> OnHealthChanged =
        new UnityEvent<GameObject, float, float>();

    public static UnityEvent<GameObject> OnDeath =
        new UnityEvent<GameObject>();
}