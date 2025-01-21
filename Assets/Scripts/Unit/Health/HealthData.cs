using UnityEngine;

[CreateAssetMenu(menuName = "Health System/Health Config")]
public class HealthData : ScriptableObject
{
    [Header("Health Settings")]
    public float MaxHealth = 100f;
    public bool DestroyOnDeath = true;
    public float DeathDelay = 0f;

    [Header("UI Settings")]
    public Vector3 UIOffset = new Vector3(0, 2f, 0);
    public float VisibilityDistance = 15f;
}