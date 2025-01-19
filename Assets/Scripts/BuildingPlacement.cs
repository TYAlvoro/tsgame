using UnityEngine;

public class SimpleBuildingPlacement : MonoBehaviour
{
    public GameObject buildingPrefab; // The building to place
    public GameObject placeholderPrefab; // The placeholder to show preview
    public LayerMask terrainMask; // Terrain layer mask
    public LayerMask buildingMask; // Mask for buildings (to avoid collision with placed buildings)

    public float maxSlopeAngle = 30f; // Maximum allowed slope angle for placement
    public float buildingYOffset = 1f; // Offset for building to avoid sinking into terrain

    private GameObject currentPlaceholder; // Current placeholder object
    private Renderer placeholderRenderer; // Cached renderer for placeholder
    private Collider placeholderCollider; // Cached collider for placeholder

    private Terrain activeTerrain; // Cached reference to active terrain
    private float timeSinceLastRaycast = 0f; // Time since last raycast
    public float raycastInterval = 0.1f; // Interval between raycasts to reduce frequency

    void Start()
    {
        activeTerrain = Terrain.activeTerrain; // Cache active terrain reference at start
        currentPlaceholder = Instantiate(placeholderPrefab);
        placeholderRenderer = currentPlaceholder.GetComponent<Renderer>();
        placeholderCollider = currentPlaceholder.GetComponent<Collider>();
        SetPlaceholderActive(false); // Deactivate placeholder at the start
    }

    void Update()
    {
        timeSinceLastRaycast += Time.deltaTime;

        if (timeSinceLastRaycast >= raycastInterval)
        {
            UpdatePlaceholder();
            timeSinceLastRaycast = 0f; // Reset the timer
        }

        if (Input.GetMouseButtonDown(0) && currentPlaceholder.activeSelf) // Left Click
        {
            if (IsPlacementValid())
            {
                PlaceBuilding();
            }
        }
    }

    void UpdatePlaceholder()
    {
        Ray ray = Camera.main != null ? Camera.main.ScreenPointToRay(Input.mousePosition) : default;
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, terrainMask))
        {
            SetPlaceholderActive(true);
            currentPlaceholder.transform.position = hit.point + Vector3.up * buildingYOffset;

            // Check for overlaps to determine valid placement
            placeholderRenderer.material.color = IsPlacementValid() ? Color.green : Color.red;
        }
        else
        {
            SetPlaceholderActive(false);
        }
    }

    bool IsPlacementValid()
    {
        // Use OverlapBox for collision check
        Collider[] colliders = Physics.OverlapBox(
            currentPlaceholder.transform.position,
            placeholderCollider.bounds.extents * 0.9f, // Slightly reduced size
            Quaternion.identity,
            buildingMask // Only check for other buildings within the building layer
        );

        // Check for collisions with other buildings
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != currentPlaceholder)
            {
                return false;
            }
        }

        // Check terrain slope (angle)
        Vector3 terrainNormal = GetTerrainNormal(currentPlaceholder.transform.position);
        float angle = Vector3.Angle(terrainNormal, Vector3.up); // Calculate angle

        return angle <= maxSlopeAngle; // Invalid placement if slope is too steep
    }

    Vector3 GetTerrainNormal(Vector3 position)
    {
        if (activeTerrain != null)
        {
            return activeTerrain.terrainData.GetInterpolatedNormal(
                (position.x - activeTerrain.transform.position.x) / activeTerrain.terrainData.size.x,
                (position.z - activeTerrain.transform.position.z) / activeTerrain.terrainData.size.z
            );
        }
        return Vector3.up; // Default to upward normal if terrain is not available
    }

    void PlaceBuilding()
    {
        Instantiate(buildingPrefab, currentPlaceholder.transform.position, Quaternion.identity);
    }

    // Helper method to manage placeholder visibility
    void SetPlaceholderActive(bool isActive)
    {
        if (currentPlaceholder.activeSelf != isActive)
        {
            currentPlaceholder.SetActive(isActive);
        }
    }
}