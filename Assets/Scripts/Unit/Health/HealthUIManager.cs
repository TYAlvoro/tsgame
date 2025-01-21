using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages healthbar pool and positioning using object pooling pattern
/// </summary>
public class HealthUIManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private HealthUI _healthUIPrefab;
    [SerializeField][Min(10)] private int _initialPoolSize = 20;

    private Dictionary<HealthSystem, HealthUI> _activeUI =
        new Dictionary<HealthSystem, HealthUI>();
    private Queue<HealthUI> _pool = new Queue<HealthUI>();
    private Transform _uiContainer;
    private Camera _mainCamera;

    public static HealthUIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        UpdateMainCameraReference();
        InitializeObjectPool();
        CreateUIContainer();
    }

    private void CreateUIContainer()
    {
        _uiContainer = new GameObject("HealthUIContainer").transform;
    }

    private void InitializeObjectPool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateNewPoolItem();
        }
    }

    private HealthUI CreateNewPoolItem()
    {
        HealthUI ui = Instantiate(_healthUIPrefab, _uiContainer);
        ui.gameObject.SetActive(false);
        ui.transform.SetParent(_uiContainer);
        _pool.Enqueue(ui);
        return ui;
    }

    /// <summary>
    /// Requests healthbar from pool for specific health system
    /// </summary>
    public HealthUI GetHealthUI(HealthSystem healthSystem)
    {
        if (_activeUI.TryGetValue(healthSystem, out HealthUI ui))
            return ui;

        if (_pool.Count == 0)
        {
            ExpandPool();
        }

        HealthUI newUI = _pool.Dequeue();
        newUI.Initialize(healthSystem, _mainCamera);
        _activeUI.Add(healthSystem, newUI);
        return newUI;
    }

    /// <summary>
    /// Returns healthbar to pool when no longer needed
    /// </summary>
    public void ReleaseHealthUI(HealthSystem healthSystem)
    {
        if (!_activeUI.TryGetValue(healthSystem, out HealthUI ui)) return;

        ui.gameObject.SetActive(false);
        ui.transform.SetParent(_uiContainer);
        _pool.Enqueue(ui);
        _activeUI.Remove(healthSystem);
    }

    private void ExpandPool()
    {
        int expandAmount = Mathf.Max(5, _initialPoolSize / 4);
        for (int i = 0; i < expandAmount; i++)
        {
            CreateNewPoolItem();
        }
    }

    private void UpdateMainCameraReference()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogError("[HealthUIManager] Main camera reference is missing!");
        }
    }

    private void LateUpdate()
    {
        UpdateMainCameraReference();
        CleanInvalidReferences();
        UpdateAllHealthbars();
    }

    private void CleanInvalidReferences()
    {
        List<HealthSystem> invalidKeys = new List<HealthSystem>();

        foreach (var pair in _activeUI)
        {
            if (pair.Key == null) invalidKeys.Add(pair.Key);
        }

        foreach (var key in invalidKeys)
        {
            ReleaseHealthUI(key);
        }
    }

    private void UpdateAllHealthbars()
    {
        foreach (var pair in _activeUI)
        {
            pair.Value.UpdateUI();
        }
    }
}