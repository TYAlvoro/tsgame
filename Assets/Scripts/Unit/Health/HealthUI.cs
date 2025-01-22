using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Visual representation of health status with dynamic scaling
/// </summary>
public class HealthUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Header("Scaling Configuration")]
    [SerializeField]
    private AnimationCurve _scaleCurve = AnimationCurve.Linear(0, 1, 1, 0.5f);

    private HealthSystem _healthSystem;
    private Camera _mainCamera;
    private Transform _target;
    private float _visibilityDistance;
    private float _baseScale;

    /// <summary>
    /// Initializes healthbar with specific entity parameters
    /// </summary>
    public void Initialize(HealthSystem healthSystem, Camera mainCamera)
    {
        _healthSystem = healthSystem;
        _mainCamera = mainCamera;
        _target = healthSystem.transform;

        var settings = _healthSystem.GetSettings();
        transform.localScale = Vector3.one * settings.uiScale;
        _visibilityDistance = settings.uiVisibilityDistance;
        _baseScale = settings.uiScale;

        // Подписываемся на изменение здоровья
        _healthSystem.OnHealthChanged += UpdateHealthValue;

        // Устанавливаем начальное значение
        UpdateHealthValue(_healthSystem.CurrentHealth / _healthSystem.GetSettings().maxHealth);

        gameObject.SetActive(true);
    }

    private void UpdateHealthValue(float normalizedHealth)
    {
        if (_healthSlider != null)
        {
            _healthSlider.value = normalizedHealth;
        }
    }

    /// <summary>
    /// Updates all visual aspects of healthbar
    /// </summary>
    public void UpdateUI()
    {
        if (_target == null || _healthSystem == null) return;

        UpdatePosition();
        UpdateRotation();
        UpdateVisibilityState();
        CalculateDynamicScale();
    }

    private void UpdatePosition()
    {
        transform.position = _target.position + _healthSystem.GetSettings().uiOffset;
    }

    private void UpdateRotation()
    {
        transform.rotation = _mainCamera.transform.rotation;
    }

    private void UpdateVisibilityState()
    {
        float distance = Vector3.Distance(_mainCamera.transform.position, _target.position);
        _canvasGroup.alpha = distance <= _visibilityDistance ? 1 : 0;
    }

    private void CalculateDynamicScale()
    {
        float distance = Vector3.Distance(_mainCamera.transform.position, _target.position);
        float normalizedDistance = Mathf.Clamp01(distance / _visibilityDistance);
        float scaleModifier = _scaleCurve.Evaluate(normalizedDistance);
        transform.localScale = Vector3.one * _baseScale * scaleModifier;
    }

    private void OnDestroy()
    {
        // Отписываемся от события при уничтожении
        if (_healthSystem != null)
        {
            _healthSystem.OnHealthChanged -= UpdateHealthValue;
        }
    }
}