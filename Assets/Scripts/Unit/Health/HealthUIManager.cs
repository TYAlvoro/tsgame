using UnityEngine;
using System.Collections.Generic;

public class HealthUIManager : MonoBehaviour
{
    [SerializeField] private GameObject healthUIPrefab;
    [SerializeField] private float updateInterval = 0.5f;

    private Transform uiContainer;
    private Camera mainCamera;
    private Dictionary<Health, HealthUI> activeUI = new Dictionary<Health, HealthUI>();

    private void Awake()
    {
        mainCamera = Camera.main;
        CreateUIContainer();
        InvokeRepeating(nameof(UpdateUI), 0, updateInterval);
    }

    private void CreateUIContainer()
    {
        uiContainer = new GameObject("HealthUIContainer").transform;
        uiContainer.position = Vector3.zero;
    }

    private void UpdateUI()
    {
        CleanupDestroyedObjects();
        CreateMissingUI();
    }

    private void CleanupDestroyedObjects()
    {
        List<Health> toRemove = new List<Health>();

        foreach (var pair in activeUI)
        {
            if (pair.Key == null || pair.Value == null)
                toRemove.Add(pair.Key);
        }

        foreach (var key in toRemove)
        {
            if (activeUI.TryGetValue(key, out HealthUI ui))
                Destroy(ui.gameObject);

            activeUI.Remove(key);
        }
    }

    private void CreateMissingUI()
    {
        foreach (var health in FindObjectsOfType<Health>())
        {
            if (!activeUI.ContainsKey(health) && health.isActiveAndEnabled)
                CreateHealthUI(health);
        }
    }

    private void CreateHealthUI(Health health)
    {
        var uiInstance = Instantiate(healthUIPrefab, uiContainer);
        var healthUI = uiInstance.GetComponent<HealthUI>();

        healthUI.Initialize(
            health.transform,
            mainCamera,
            health.Config.UIOffset,
            health.Config.VisibilityDistance
        );

        activeUI.Add(health, healthUI);
    }
}