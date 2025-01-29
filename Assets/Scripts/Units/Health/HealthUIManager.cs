using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages creation, recycling, and updating of health UI elements using object pooling
/// </summary>
public class HealthUIManager : MonoBehaviour
{
    [Header("Pool Configuration")]
    [Tooltip("Prefab reference for health UI elements")]
    [SerializeField] private HealthUI _healthUIPrefab;

    [Tooltip("Initial number of UI elements to create in pool")]
    [SerializeField][Min(10)] private int _initialPoolSize = 20;

    [Header("Dependencies")]
    [Tooltip("Container transform for organizing pooled UI elements")]
    [SerializeField] private Transform _uiContainer;

    private Dictionary<HealthSystem, HealthUI> _activeHealthUIs =
        new Dictionary<HealthSystem, HealthUI>();
    private Queue<HealthUI> _inactivePool = new Queue<HealthUI>();
    private Camera _mainCamera;

    #region Singleton Implementation
    public static HealthUIManager Instance { get; private set; }

    private void Awake()
    {
        HandleSingletonInitialization();
        InitializeCameraReference();
        CreateUIContainer();
        WarmObjectPool();
    }

    private void HandleSingletonInitialization()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    #region Pool Management
    private void WarmObjectPool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            CreateAndStorePoolItem();
        }
    }

    private HealthUI CreateAndStorePoolItem()
    {
        var newUI = Instantiate(_healthUIPrefab, _uiContainer);
        newUI.gameObject.SetActive(false);
        _inactivePool.Enqueue(newUI);
        return newUI;
    }

    private void ExpandPool()
    {
        int expansionSize = Mathf.Max(5, _initialPoolSize / 4);
        for (int i = 0; i < expansionSize; i++)
        {
            CreateAndStorePoolItem();
        }
    }
    #endregion

    #region Public Interface
    /// <summary>
    /// Retrieves a health UI element for the specified health system
    /// </summary>
    /// <param name="healthSystem">Target health system needing UI representation</param>
    /// <returns>Configured HealthUI component</returns>
    public HealthUI RequestHealthUI(HealthSystem healthSystem)
    {
        if (healthSystem == null) return null;

        if (_activeHealthUIs.TryGetValue(healthSystem, out var existingUI))
        {
            return existingUI;
        }

        if (_inactivePool.Count == 0)
        {
            ExpandPool();
        }

        var newUI = _inactivePool.Dequeue();
        ConfigureHealthUI(newUI, healthSystem);
        return newUI;
    }

    /// <summary>
    /// Returns a health UI element to the pool when no longer needed
    /// </summary>
    /// <param name="healthSystem">Health system associated with the UI element</param>
    public void ReturnHealthUI(HealthSystem healthSystem)
    {
        if (!_activeHealthUIs.TryGetValue(healthSystem, out var ui)) return;

        ResetUIElement(ui);
        _activeHealthUIs.Remove(healthSystem);
    }
    #endregion

    #region UI Configuration
    private void ConfigureHealthUI(HealthUI ui, HealthSystem healthSystem)
    {
        ui.Initialize(healthSystem, _mainCamera);
        ui.gameObject.SetActive(true);
        _activeHealthUIs[healthSystem] = ui;
    }

    private void ResetUIElement(HealthUI ui)
    {
        ui.gameObject.SetActive(false);
        ui.transform.SetParent(_uiContainer);
        _inactivePool.Enqueue(ui);
    }
    #endregion

    #region Frame Updates
    private void LateUpdate()
    {
        RefreshCameraReference();
        MaintainActiveElements();
        UpdateAllUIElements();
    }

    private void MaintainActiveElements()
    {
        CleanInvalidReferences();
        ValidateCameraDependency();
    }

    private void UpdateAllUIElements()
    {
        foreach (var ui in _activeHealthUIs.Values)
        {
            ui.UpdateUI();
        }
    }
    #endregion

    #region Reference Management
    private void CreateUIContainer()
    {
        if (_uiContainer == null)
        {
            _uiContainer = new GameObject("HealthUIContainer").transform;
        }
    }

    private void InitializeCameraReference()
    {
        _mainCamera = Camera.main;
        ValidateCameraDependency();
    }

    private void RefreshCameraReference()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }
    }

    private void ValidateCameraDependency()
    {
        if (_mainCamera == null)
        {
            Debug.LogError("Main camera reference is missing in HealthUIManager");
        }
    }

    private void CleanInvalidReferences()
    {
        var invalidKeys = _activeHealthUIs.Keys
            .Where(key => key == null || key.gameObject == null)
            .ToList();

        foreach (var key in invalidKeys)
        {
            ReturnHealthUI(key);
        }
    }
    #endregion
}