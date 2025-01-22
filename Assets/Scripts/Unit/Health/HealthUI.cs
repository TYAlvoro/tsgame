using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HealthUI : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("Slider component representing health value")]
    [SerializeField] private Slider _healthSlider;

    [Tooltip("Canvas group for controlling visibility")]
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Scaling Configuration")]
    [Tooltip("Curve defining scale changes based on distance")]
    [SerializeField] private AnimationCurve _scaleCurve = AnimationCurve.Linear(0, 1, 1, 0.5f);

    private HealthSystem _healthSystem;
    private Camera _mainCamera;
    private Transform _target;
    private float _visibilityDistance;
    private float _baseScale;

    #region Initialization
    /// <summary>
    /// Initializes the health UI with necessary components
    /// </summary>
    /// <param name="healthSystem">Reference to the health system component</param>
    /// <param name="mainCamera">Main camera reference for positioning</param>
    public void Initialize(HealthSystem healthSystem, Camera mainCamera)
    {
        _healthSystem = healthSystem;
        _mainCamera = mainCamera;
        _target = healthSystem.transform;

        var healthSettings = _healthSystem.GetSettings();
        ConfigureInitialSettings(healthSettings);
        SubscribeToHealthEvents();

        gameObject.SetActive(true);
    }

    private void ConfigureInitialSettings(HealthSystem.Settings settings)
    {
        transform.localScale = Vector3.one * settings.uiScale;
        _visibilityDistance = settings.uiVisibilityDistance;
        _baseScale = settings.uiScale;
    }

    private void SubscribeToHealthEvents()
    {
        _healthSystem.OnHealthChanged += HandleHealthChanged;
        UpdateHealthDisplay(_healthSystem.CurrentHealth / _healthSystem.GetSettings().maxHealth);
    }
    #endregion

    #region UI Update Methods
    /// <summary>
    /// Updates all UI elements (position, rotation, visibility, scale)
    /// </summary>
    public void UpdateUI()
    {
        if (!IsValidReference()) return;

        UpdateTransform();
        UpdateVisibility();
        UpdateScale();
    }

    private bool IsValidReference()
    {
        return _target != null && _healthSystem != null;
    }

    private void UpdateTransform()
    {
        UpdatePosition();
        UpdateRotation();
    }

    private void UpdatePosition()
    {
        transform.position = _target.position + _healthSystem.GetSettings().uiOffset;
    }

    private void UpdateRotation()
    {
        transform.rotation = _mainCamera.transform.rotation;
    }

    private void UpdateVisibility()
    {
        _canvasGroup.alpha = IsWithinVisibilityRange() ? 1 : 0;
    }

    private void UpdateScale()
    {
        float normalizedDistance = GetNormalizedDistance();
        float scaleModifier = _scaleCurve.Evaluate(normalizedDistance);
        transform.localScale = Vector3.one * _baseScale * scaleModifier;
    }
    #endregion

    #region Helper Calculations
    private bool IsWithinVisibilityRange()
    {
        return CalculateDistance() <= _visibilityDistance;
    }

    private float GetNormalizedDistance()
    {
        return Mathf.Clamp01(CalculateDistance() / _visibilityDistance);
    }

    private float CalculateDistance()
    {
        return Vector3.Distance(_mainCamera.transform.position, _target.position);
    }
    #endregion

    #region Event Handlers
    private void HandleHealthChanged(float normalizedHealth)
    {
        UpdateHealthDisplay(normalizedHealth);
    }

    private void UpdateHealthDisplay(float normalizedHealth)
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = normalizedHealth;
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromHealthEvents();
    }

    private void UnsubscribeFromHealthEvents()
    {
        if (_healthSystem != null)
        {
            _healthSystem.OnHealthChanged -= HandleHealthChanged;
        }
    }
    #endregion
}