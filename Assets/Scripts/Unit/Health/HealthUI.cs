using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float scaleFactor = 0.005f;
    [SerializeField] private Vector2 size = new Vector2(200, 20);

    private RectTransform rectTransform;
    private Transform target;
    private Camera mainCamera;
    private Vector3 positionOffset;
    private float maxViewDistance;

    public void Initialize(Transform targetTransform, Camera camera,
                         Vector3 offset, float visibilityDistance)
    {
        rectTransform = GetComponent<RectTransform>();
        target = targetTransform;
        mainCamera = camera;
        positionOffset = offset;
        maxViewDistance = visibilityDistance;

        ConfigureCanvas();
        RegisterEvents();
        UpdateTransform();
    }

    private void ConfigureCanvas()
    {
        var canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = mainCamera;

        // Критически важные настройки
        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(200, 20); // Ширина x Высота

        transform.localScale = Vector3.one * scaleFactor;
    }

    private void RegisterEvents()
    {
        HealthEventSystem.OnHealthChanged.AddListener(OnHealthUpdate);
        HealthEventSystem.OnDeath.AddListener(OnTargetDeath);
    }

    private void OnHealthUpdate(GameObject obj, float current, float max)
    {
        if (obj != target.gameObject) return;
        healthSlider.value = Mathf.Clamp01(current / max);
    }

    private void OnTargetDeath(GameObject obj)
    {
        if (obj == target.gameObject) Destroy(gameObject);
    }

    private void LateUpdate() => UpdateTransform();

    private void UpdateTransform()
    {
        if (target == null) return;

        transform.position = target.position + positionOffset;
        transform.rotation = mainCamera.transform.rotation;

        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (mainCamera == null) return;

        float distance = Vector3.Distance(
            mainCamera.transform.position,
            target.position
        );

        canvasGroup.alpha = distance <= maxViewDistance ? 1 : 0;
        float distanceScaleModifier = Mathf.Clamp(1 - (distance / maxViewDistance), 0.5f, 1f);
        transform.localScale = Vector3.one * scaleFactor * distanceScaleModifier;
    }

    private void OnDestroy()
    {
        HealthEventSystem.OnHealthChanged.RemoveListener(OnHealthUpdate);
        HealthEventSystem.OnDeath.RemoveListener(OnTargetDeath);
    }
}